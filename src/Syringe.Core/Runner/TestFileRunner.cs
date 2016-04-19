using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syringe.Core.Http;
using Syringe.Core.Http.Logging;
using Syringe.Core.Logging;
using Syringe.Core.Repositories;
using Syringe.Core.Runner.Assertions;
using Syringe.Core.Tests;
using Syringe.Core.Tests.Results;
using HttpResponse = Syringe.Core.Http.HttpResponse;

namespace Syringe.Core.Runner
{
    public class TestFileRunner : IObservable<TestResult>
    {
        private readonly IHttpClient _httpClient;
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

        public TestFileRunner(IHttpClient httpClient, ITestFileResultRepository repository)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            _httpClient = httpClient;
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

            var testFileResult = new TestFileResult();
            testFileResult.Filename = testFile.Filename;
            testFileResult.StartTime = DateTime.UtcNow;

            // Add all config variables and ones in this <test>
            var variables = new CapturedVariableProvider();
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


            foreach (Test test in tests)
            {
                if (_isStopPending)
                    break;

                try
                {
                    TestResult result = await RunTestAsync(test, variables, verificationsMatcher);
                    AddResult(testFileResult, result);

                    if (result.ResponseTime < minResponseTime)
                        minResponseTime = result.ResponseTime;

                    if (result.ResponseTime > maxResponseTime)
                        maxResponseTime = result.ResponseTime;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An exception occurred running case {0}", test.Position);
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

        internal async Task<TestResult> RunTestAsync(Test test, CapturedVariableProvider variables, AssertionsMatcher assertionMatcher)
        {
            var testResult = new TestResult();
	        testResult.Position = test.Position;
            testResult.SessionId = SessionId;
            testResult.Test = test;

            try
            {
                string resolvedUrl = variables.ReplacePlainTextVariablesIn(test.Url);
                testResult.ActualUrl = resolvedUrl;

                var httpLogWriter = new HttpLogWriter();

				HttpResponse response = await _httpClient.ExecuteRequestAsync(test.Method, resolvedUrl, test.PostBody, test.Headers, httpLogWriter);
                testResult.ResponseTime = response.ResponseTime;
                testResult.HttpResponse = response;
                testResult.HttpLog = httpLogWriter.StringBuilder.ToString();
                testResult.HttpContent = response.Content;

                if (response.StatusCode == test.ExpectedHttpStatusCode)
                {
                    testResult.ResponseCodeSuccess = true;
                    string content = response.ToString();

                    // Put the parseresponse regex values in the current variable set
                    var logger = new SimpleLogger();
					logger.WriteLine("--------------------------");
					logger.WriteLine("Parsing variables");
                    logger.WriteLine("--------------------------");
                    List<Variable> parsedVariables = CapturedVariableProvider.MatchVariables(test.CapturedVariables, content, logger);
                    variables.AddOrUpdateVariables(parsedVariables);
                    if (parsedVariables.Count == 0)
                    {
                        logger.WriteLine("(No variables to parse)");
                    }

                    // Verify assertions

                    testResult.AssertionResults = assertionMatcher.MatchVerifications(test.Assertions, content);
					logger.WriteLine("--------------------------");
					logger.WriteLine("Assertions");
                    logger.WriteLine("--------------------------");
                    if (testResult.AssertionResults.Count > 0)
                    {
                        foreach (Assertion item in testResult.AssertionResults)
                        {
                            logger.Write(item.Log);
                        }
                    }
                    else
                    {
                        logger.WriteLine("(No assertions found)");
                    }

                    // Store the log
                    testResult.Log = logger.GetLog();
                }
                else
                {
                    testResult.ResponseCodeSuccess = false;
                    testResult.Log = "No verifications run - the response code did not match the expected response code.";
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
