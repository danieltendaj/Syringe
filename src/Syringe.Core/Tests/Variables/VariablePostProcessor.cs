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
            }

            return value;
        }
    }
}