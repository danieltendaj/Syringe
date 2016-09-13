using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Syringe.Core.Reader;

namespace Syringe.Tests.Integration.Reader
{
    [TestFixture]
    public class TestCaseFileReaderTests
    {

        protected string GetFilePath(string fileName)
        {
            var manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            var fileResourceName = manifestResourceNames.First(x => x.Contains(fileName));

   

            return manifestResourceNames.First(x => x.Contains(fileName));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Get_should_throw_exception_when_file_not_found()
        {
            // given
            var caseFileReader = new TestCaseFileReader(new TestCaseDirectory());

            // when
            caseFileReader.Get("filenotexist.xml");
        }

        [Test]
        public void Get_file_should_not_be_empty_when_file_found()
        {
            // given
            var mock = new Mock<ITestCaseDirectory>();

            mock.Setup(x => x.GetFullPath("roadkill-login.xml")).Returns(GetFilePath("roadkill-login.xml"));

            var caseFileReader = new TestCaseFileReader(mock.Object);

            // when
            var textReader = caseFileReader.Get("roadkill-login.xml");

            // then
            Assert.NotNull(textReader);
        }
    }
}
