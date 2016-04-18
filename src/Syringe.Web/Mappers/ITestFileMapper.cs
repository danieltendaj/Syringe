using System.Collections.Generic;
using Syringe.Core.Tests;
using Syringe.Web.Models;

namespace Syringe.Web.Mappers
{
    public interface ITestFileMapper
    {
        TestViewModel BuildViewModel(Test test);
        IEnumerable<TestViewModel> BuildTests(IEnumerable<Test> tests);
        Test BuildCoreModel(TestViewModel testModel);
        List<VariableViewModel> BuildVariableViewModel(TestFile test);
    }
}