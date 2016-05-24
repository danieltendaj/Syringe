using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Syringe.Core.Configuration;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Logging;
using Syringe.Core.Runner.Assertions;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;
using Syringe.Core.Tests.Results.Repositories;
using Syringe.Core.Tests.Scripting;
using Syringe.Core.Tests.Variables;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Core.Runner
{
    public class TestFileRunner : IObservable<TestResult>
    {
        private readonly IHttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private bool _isStopPending;
        private List<TestResult> _currentResults;

        private readonly Dictionary<Guid, TestSessionRunnerSubscriber> _subscribers =
            new Dictionary<Guid, TestSessionRunnerSubscriber>();

        public ITestFileResultRepository Repository { get; set; }
        public Guid SessionId { get; internal set; }

        public IEnumerable<TestResult> CurrentResults
        {
            get
            {
                lock (_currentResults)
                {
                    return _currentResults.AsReadOnly();
                }
            }
        }

        public int TestsRun { get; set; }
        public int TotalTests { get; set; }

        public TestFileRunner(IHttpClient httpClient, ITestFileResultRepository repository, IConfiguration configuration)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            _httpClient = httpClient;
            _configuration = configuration;
            _currentResults = new List<TestResult>();
            Repository = repository;

            SessionId = Guid.NewGuid();
        }

        private void NotifySubscribers(Action<IObserver<TestResult>> observerAction)
        {
            IDictionary<Guid, TestSessionRunnerSubscriber> currentSubscribers;
            lock (_subscribers)
            {
                currentSubscribers = _subscribers.ToDictionary(k => k.Key, v => v.Value);
            }

            foreach (var subscriber in currentSubscribers.Values)
            {
                observerAction(subscriber.Observer);
            }
        }

        private void NotifySubscribersOfAddedResult(TestResult result)
        {
            NotifySubscribers(observer => observer.OnNext(result));
        }

        private void NotifySubscribersOfCompletion()
        {
            NotifySubscribers(observer => observer.OnCompleted());
        }

        public void Stop()
        {
            _isStopPending = true;
        }

        public IDisposable Subscribe(IObserver<TestResult> observer)
        {
            // Notify of the observer of existing results.
            IEnumerable<TestResult> resultsCopy;
            lock (_currentResults)
            {
                resultsCopy = _currentResults.ToArray();
            }

            foreach (TestResult testResult in resultsCopy)
            {
                observer.OnNext(testResult);
            }

            return new TestSessionRunnerSubscriber(observer, _subscribers);
        }

        public async Task<TestFileResult> RunAsync(TestFile testFile)
        {
            _isStopPending = false;
            lock (_currentResults)
            {
                _currentResults = new List<TestResult>();
            }

            var testFileResult = new TestFileResult
            {
                Filename = testFile.Filename,
                StartTime = DateTime.UtcNow,
                Environment = testFile.Environment
            };

            // Add all config variables and ones in this <test>
            var variables = new CapturedVariableProvider(testFile.Environment);
            variables.AddOrUpdateVariables(testFile.Variables);

            var verificationsMatcher = new AssertionsMatcher(variables);

            // Ensure we loop atleast once:
            List<Test> tests = testFile.Tests.ToList();

            TimeSpan minResponseTime = TimeSpan.MaxValue;
            TimeSpan maxResponseTime = TimeSpan.MinValue;
            int totalTestsRun = 0;
            TestsRun = 0;
            TotalTests = tests.Count;
            bool shouldSave = true;


            for (int i = 0; i < tests.Count; i++)
            {
                if (_isStopPending)
                {
                    break;
                }

                try
                {
                    TestResult result = await RunTestAsync(tests.ElementAt(i), i, variables, verificationsMatcher);
                    AddResult(testFileResult, result);

                    if (result.ResponseTime < minResponseTime)
                    {
                        minResponseTime = result.ResponseTime;
                    }

                    if (result.ResponseTime > maxResponseTime)
                    {
                        maxResponseTime = result.ResponseTime;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An exception occurred running index {0}", i);
                    ReportError(ex);
                }
                finally
                {
                    totalTestsRun++;
                    TestsRun++;
                }

                if (_isStopPending)
                {
                    shouldSave = false;
                    break;
                }
            }

            testFileResult.EndTime = DateTime.UtcNow;
            testFileResult.TotalRunTime = testFileResult.EndTime - testFileResult.StartTime;
            testFileResult.TotalTestsRun = totalTestsRun;
            testFileResult.MinResponseTime = minResponseTime;
            testFileResult.MaxResponseTime = maxResponseTime;

            NotifySubscribersOfCompletion();

            if (shouldSave)
            {
                await Repository.AddAsync(testFileResult);
            }

            return testFileResult;
        }

        private void AddResult(TestFileResult session, TestResult result)
        {
            session.TestResults.Add(result);
            lock (_currentResults)
            {
                _currentResults.Add(result);
            }
            NotifySubscribersOfAddedResult(result);
        }

        public void ReportError(Exception exception)
        {
            NotifySubscribers(observer => observer.OnError(exception));
        }

        internal async Task<TestResult> RunTestAsync(Test test, int position, CapturedVariableProvider variables, AssertionsMatcher assertionMatcher)
        {
            var testResult = new TestResult
            {
                Position = position,
                SessionId = SessionId,
                Test = test
            };

            try
            {
                string resolvedUrl = variables.ReplacePlainTextVariablesIn(test.Url);
                testResult.ActualUrl = resolvedUrl;

                string postBody = variables.ReplacePlainTextVariablesIn(test.PostBody);
                foreach (HeaderItem header in test.Headers)
                {
                    header.Value = variables.ReplacePlainTextVariablesIn(header.Value);
                }

                IRestRequest request = _httpClient.CreateRestRequest(test.Method, resolvedUrl, postBody, test.Headers);
                var logger = new SimpleLogger();

                // Scripting part
                if (!string.IsNullOrEmpty(test.BeforeExecuteScript))
                {
                    logger.WriteLine("Evaluating C# script");

                    try
                    {
                        var evaluator = new TestFileScriptEvaluator(_configuration);
                        evaluator.EvaluateBeforeExecute(test, request);

                        request = evaluator.RequestGlobals.Request;
                        test = evaluator.RequestGlobals.Test;
                        logger.WriteIndentedLine("Compilation successful.");

                    }
                    catch (Exception ex)
                    {
                        testResult.ScriptCompilationSuccess = false;
                        testResult.ExceptionMessage = "The script failed to compile - see the log file for a stack trace.";
                        logger.WriteIndentedLine("Compilation failed: {0}", ex);
                    }
                }

                var httpLogWriter = new HttpLogWriter();
                HttpResponse response = await _httpClient.ExecuteRequestAsync(request, httpLogWriter);
                testResult.ResponseTime = response.ResponseTime;
                testResult.HttpResponse = response;
                testResult.HttpLog = httpLogWriter.StringBuilder.ToString();
                testResult.HttpContent = response.Content;

                if (response.StatusCode == test.ExpectedHttpStatusCode)
                {
                    testResult.ResponseCodeSuccess = true;
                    string content = response.ToString();

                    // Put the captured variables regex values in the current variable set
                    foreach (var capturedVariable in test.CapturedVariables)
                    {
                        capturedVariable.Regex = variables.ReplacePlainTextVariablesIn(capturedVariable.Regex);
                    }

                    List<Variable> parsedVariables = CapturedVariableProvider.MatchVariables(test.CapturedVariables, content, logger);
                    variables.AddOrUpdateVariables(parsedVariables);
                    logger.WriteLine("{0} captured variable(s) parsed.", parsedVariables.Count);

                    // Verify assertions
                    testResult.AssertionResults = assertionMatcher.MatchVerifications(test.Assertions, content);
                    logger.WriteLine("Verifying {0} assertion(s)", testResult.AssertionResults.Count);
                    foreach (Assertion item in testResult.AssertionResults)
                    {
                        logger.Write(item.Log);
                    }

                    // Store the log
                    testResult.Log = logger.GetLog();
                }
                else
                {
                    testResult.ResponseCodeSuccess = false;
                    testResult.Log = $"No verifications run - the response code {response.StatusCode} did not match the expected response code {test.ExpectedHttpStatusCode}.";
                }

            }
            catch (Exception ex)
            {
                testResult.Log = "An exception occured: " + ex;
                testResult.ResponseCodeSuccess = false;
                testResult.ExceptionMessage = ex.Message;
            }

            return testResult;
        }

        private sealed class TestSessionRunnerSubscriber : IDisposable
        {
            private readonly Guid _key;
            private readonly Dictionary<Guid, TestSessionRunnerSubscriber> _subscriptionList;

            public TestSessionRunnerSubscriber(IObserver<TestResult> observer,
                Dictionary<Guid, TestSessionRunnerSubscriber> subscriptionList)
            {
                Observer = observer;
                _subscriptionList = subscriptionList;
                _key = Guid.NewGuid();

                lock (subscriptionList)
                {
                    subscriptionList.Add(_key, this);
                }
            }

            public IObserver<TestResult> Observer { get; private set; }

            public void Dispose()
            {
                lock (_subscriptionList)
                {
                    _subscriptionList.Remove(_key);
                }
            }
        }
    }
}
