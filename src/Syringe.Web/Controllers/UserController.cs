using System.Web.Mvc;
using Syringe.Core.Configuration;
using Syringe.Core.Tests.Variables.Encryption;
using Syringe.Web.Models;

namespace Syringe.Web.Controllers
{
    [AuthorizeWhenOAuth]
    public class UserController : Controller
    {
	    private readonly IVariableEncryptor _encryptor;
	    private readonly IConfiguration _configuration;

	    public UserController(IVariableEncryptor encryptor, IConfiguration configuration)
	    {
		    _encryptor = encryptor;
		    _configuration = configuration;
	    }

	    [HttpGet]
        public ActionResult EncryptData()
        {
			var model = new EncryptedDataViewModel()
			{
				IsEnabled = !string.IsNullOrEmpty(_configuration.EncryptionKey)
			};

			return View(model);
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
				IsEnabled = !string.IsNullOrEmpty(_configuration.EncryptionKey),
				PlainValue = variableValue,
				EncryptedValue = encryptedValue
			};

            return View("EncryptData", model);
        }
    }
}