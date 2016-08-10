using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;
using Syringe.Core.Runner.Messaging;
using Syringe.Core.Tests.Results;
using Syringe.Service.Controllers.Hubs;

namespace Syringe.Service.Parallel
{
    public class TaskPublisher : ITaskPublisher
    {
        private readonly ITaskGroupProvider _taskGroupProvider;
        private readonly IHubConnectionContext<ITaskMonitorHubClient> _hubConnectionContext;
        private readonly Dictionary<Type, Action<ITaskMonitorHubClient, IMessage>> _messageMappings;

        public TaskPublisher(ITaskGroupProvider taskGroupProvider,
            IHubConnectionContext<ITaskMonitorHubClient> hubConnectionContext)
        {
            _taskGroupProvider = taskGroupProvider;
            _hubConnectionContext = hubConnectionContext;

            _messageMappings = new Dictionary<Type, Action<ITaskMonitorHubClient, IMessage>>
            {
                { typeof (TestResultMessage), SendCompletedTask },
                { typeof (TestFileGuidMessage), SendTestFileGuid }
            };
        }

        public void Start(int taskId, IObservable<IMessage> resultSource)
        {
            var taskGroup = _taskGroupProvider.GetGroupForTask(taskId);
            resultSource.Subscribe(result => OnMessage(taskGroup, result));
        }

        private void OnMessage(string taskGroup, IMessage result)
        {
            ITaskMonitorHubClient clientGroup = _hubConnectionContext.Group(taskGroup);
            _messageMappings[result.GetType()](clientGroup, result);
        }

        private void SendCompletedTask(ITaskMonitorHubClient clientGroup, IMessage message)
        {
            TestResult result = ((TestResultMessage)message).TestResult;
            var info = new CompletedTaskInfo
            {
                ActualUrl = result.ActualUrl,
                HttpResponse = result.HttpResponse,
                Success = result.Success,
                ResultId = result.Position,
                Position = result.Position,
                ExceptionMessage = result.ExceptionMessage,
                Verifications = result.AssertionResults
            };

            clientGroup.OnTaskCompleted(info);
        }

        private void SendTestFileGuid(ITaskMonitorHubClient clientGroup, IMessage message)
        {
            var testFileGuidMessage = (TestFileGuidMessage)message;
            clientGroup.OnTestFileGuid(testFileGuidMessage.ResultId.ToString());
        }
    }
}