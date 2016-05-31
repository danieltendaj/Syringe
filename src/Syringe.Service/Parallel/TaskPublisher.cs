using System;
using Microsoft.AspNet.SignalR.Hubs;
using Syringe.Core.Runner.Messaging;
using Syringe.Core.Tests.Results;
using Syringe.Service.Api.Hubs;

namespace Syringe.Service.Parallel
{
    public class TaskPublisher : ITaskPublisher
    {
        private readonly ITaskGroupProvider _taskGroupProvider;
        private readonly IHubConnectionContext<ITaskMonitorHubClient> _hubConnectionContext;

        public TaskPublisher(ITaskGroupProvider taskGroupProvider, IHubConnectionContext<ITaskMonitorHubClient> hubConnectionContext)
        {
            _taskGroupProvider = taskGroupProvider;
            _hubConnectionContext = hubConnectionContext;
        }

        public void Start(int taskId, IObservable<IMessage> resultSource)
        {
            var taskGroup = _taskGroupProvider.GetGroupForTask(taskId);
            resultSource.Subscribe(result => OnMessage(taskGroup, result));
        }

        private void OnMessage(string taskGroup, IMessage result)
        {
            ITaskMonitorHubClient clientGroup = _hubConnectionContext.Group(taskGroup);

            switch (result.MessageType)
            {
                case MessageType.TestCompletionResult: SendCompletedTask(clientGroup, result as TestResultMessage); break;
            }

        }

        private void SendCompletedTask(ITaskMonitorHubClient clientGroup, TestResultMessage message)
        {
            TestResult result = message.TestResult;
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
    }
}