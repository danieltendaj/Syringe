using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Syringe.Core.Services;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Web.Controllers.Attribute;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
	[AuthorizeWhenOAuth]
	public class SystemController : Controller
	{
		private readonly IVariableEncryptor _encryptor;
		private readonly IConfigurationService _configurationClient;

		public SystemController(IVariableEncryptor encryptor, IConfigurationService configurationClient)
		{
			_encryptor = encryptor;
			_configurationClient = configurationClient;
		}

		[HttpGet]
		public ActionResult EncryptData()
		{
			string encryptionKey = _configurationClient.GetSettings().EncryptionKey;

			var model = new EncryptedDataViewModel()
			{
				IsEnabled = !string.IsNullOrEmpty(encryptionKey)
			};

			return View("EncryptData", model);
		}

		[HttpPost]
		public ActionResult EncryptData(string variableValue)
		{
			string encryptedValue = "";

			if (!string.IsNullOrEmpty(variableValue))
			{
				encryptedValue = _encryptor.Encrypt(variableValue);
			}

			var model = new EncryptedDataViewModel()
			{
				IsEnabled = true,
				PlainValue = variableValue ?? string.Empty,
				EncryptedValue = encryptedValue
			};

			return View("EncryptData", model);
		}

		public ActionResult Settings()
		{
			var variables = _configurationClient
				.GetSystemVariables()
				.Select(x => new VariableViewModel
				{
					Name = x.Name,
					Value = x.Value,
					Environment = x.Environment?.Name ?? string.Empty
				});

			return View("Settings", new SystemSettingsViewModel { Variables = variables });
		}
	}
}