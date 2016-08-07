using System.Collections.Generic;
using Syringe.Core.Http;
using Syringe.Core.Tests;

namespace Syringe.Service.Controllers.Hubs
{
    public class CompletedTaskInfo
    {
        public string ActualUrl { get; set; }
        public int ResultId { get; set; }
        public bool Success { get; set; }
        public HttpResponse HttpResponse { get; set; }
        public int  Position { get; set; }
        public string ExceptionMessage { get; set; }
        public List<Assertion> Verifications { get; set; }
    }
}