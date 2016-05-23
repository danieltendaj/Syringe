using System.Collections.Generic;
using System.Web.Mvc;
using Syringe.Core.Tests;
using Syringe.Web.Models;

namespace Syringe.Web.Mappers
{
    public interface ITestFileMapper
    {
        TestViewModel BuildViewModel(Test test);
        TestViewModel BuildTestViewModel(TestFile testFile, int position);
        IEnumerable<TestViewModel> BuildTests(IEnumerable<Test> tests);
        Test BuildCoreModel(TestViewModel testModel);
        List<VariableViewModel> BuildVariableViewModel(TestFile test);
    }
}