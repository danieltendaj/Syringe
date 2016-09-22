using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Syringe.Core.Tests.Results.Repositories;
using Syringe.Core.Tests.Variables.SharedVariables;

namespace Syringe.Service.Controllers
{
    /// <summary>
    /// The work flow is that you have to request an admin key before being able to use these functions.
    /// Just adds an extra level of security to ensure these aren't used by mistake... :-)
    /// </summary>
    [Route("api/admin")]
    public class AdminController : Controller
    {
        private static readonly Guid ValidAdminKey = Guid.NewGuid();
        private readonly ITestFileResultRepository _testFileResultRepository;

        public AdminController(ITestFileResultRepository testFileResultRepository)
        {
            _testFileResultRepository = testFileResultRepository;
        }

        [Route("key")]
        [HttpGet]
        public Guid GetAdminKey()
        {
            return ValidAdminKey;
        }

        [Route("database")]
        [HttpDelete]
        public async Task<bool> WipeDatabase(Guid adminKey)
        {
            if (adminKey == ValidAdminKey)
            {
                await _testFileResultRepository.Wipe();
                return true;
            }

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}