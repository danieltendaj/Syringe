using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syringe.Core.Security;
using Syringe.Core.Services;
using Syringe.Core.Tasks;
using Syringe.Core.Tests;
using Syringe.Web.Controllers.Attribute;

namespace Syringe.Web.Controllers
{
	[AuthorizeWhenOAuth]
	public class JsonController : Controller
	{
		private readonly ITasksService _tasksClient;
		private readonly ITestService _testsClient;
		private readonly IUserContext _userContext;

		public JsonController(ITasksService tasksService, ITestService testsClient, IUserContext userContext)
		{
			_tasksClient = tasksService;
			_testsClient = testsClient;
			_userContext = userContext;
		}

		public ActionResult Run(string filename)
		{
			var taskRequest = new TaskRequest()
			{
				Filename = filename,
				Username = _userContext.FullName,
			};
			int taskId = _tasksClient.Start(taskRequest);

			return Json(new { taskId = taskId });
		}

		public ActionResult PollTaskStatus(int taskId)
		{
			var details = _tasksClient.GetTask(taskId);

			var taskProgressModel = new TaskProgressViewMode()
			{
				TaskId = taskId,
				CurrentItem = details.CurrentIndex,
				TotalTests = details.TotalTests,
				IsFinished = details.Status == "RanToCompletion",
				ResultGuid = details.Results.FirstOrDefault()?.SessionId
			};

			return Json(taskProgressModel);
		}

		public ActionResult GetTests(string filename)
		{
			TestFile testFile = _testsClient.GetTestFile(filename);
			return Content(JsonConvert.SerializeObject(testFile), "application/json");
		}
	}
}