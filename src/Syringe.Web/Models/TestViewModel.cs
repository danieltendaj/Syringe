using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Web.Mvc;
using Syringe.Core.Tests;

namespace Syringe.Web.Models
{
    public class TestViewModel
    {
        [Required]
        public int Position { get; set; }

        [Display(Name = "Description")]
        [Required]
        public string Description { get; set; }

        [Required]
        public string Url { get; set; }

        [Display(Name = "Post Body")]
        public string PostBody { get; set; }

        [Required]
        public MethodType Method { get; set; }

        [Required]
        [Display(Name = "Expected HTTP code")]
        public HttpStatusCode ExpectedHttpStatusCode { get; set; }

        public List<HeaderItem> Headers { get; set; }
        public List<CapturedVariableItem> CapturedVariables { get; set; }
        public List<AssertionViewModel> Assertions { get; set; }
        public List<VariableViewModel> AvailableVariables { get; set; }

		public string BeforeExecuteScript { get; set; }

		[Required]
        public string Filename { get; set; }

        public TestViewModel()
        {
            Headers = new List<HeaderItem>();
            CapturedVariables = new List<CapturedVariableItem>();
            Assertions = new List<AssertionViewModel>();
            AvailableVariables = new List<VariableViewModel>();
        }
    }
}