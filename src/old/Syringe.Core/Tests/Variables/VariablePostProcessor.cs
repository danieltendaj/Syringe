using System;
using System.Net;

namespace Syringe.Core.Tests.Variables
{
    public class VariablePostProcessor : IVariablePostProcessor
    {
        public string Process(string value, VariablePostProcessorType postProcessorType)
        {
            switch (postProcessorType)
            {
                case VariablePostProcessorType.HtmlDecode: value = WebUtility.HtmlDecode(value); break;
                case VariablePostProcessorType.HtmlEncode: value = WebUtility.HtmlEncode(value); break;
                case VariablePostProcessorType.UrlDecode: value = WebUtility.UrlDecode(value); break;
                case VariablePostProcessorType.UrlEncode: value = WebUtility.UrlEncode(value); break;
            }

            return value;
        }
    }
}