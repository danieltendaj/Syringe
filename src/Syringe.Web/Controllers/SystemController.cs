using System.Linq;
using System.Web.Mvc;
using Syringe.Core.Configuration;
using Syringe.Core.Services;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
    [AuthorizeWhenOAuth]
    public class SystemController : Controller
    {
        private readonly IVariableEncryptor _encryptor;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationService _configurationClient;

        public SystemController(IVariableEncryptor encryptor, IConfiguration configuration, IConfigurationService configurationClient)
        {
            _encryptor = encryptor;
            _configuration = configuration;
            _configurationClient = configurationClient;
        }
        
        [HttpGet]
        public ActionResult EncryptData()
        {
            var model = new EncryptedDataViewModel()
            {
                IsEnabled = !string.IsNullOrEmpty(_configuration.EncryptionKey)
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

            return View(new SystemSettingsViewModel { Variables = variables });
        }
    }
}