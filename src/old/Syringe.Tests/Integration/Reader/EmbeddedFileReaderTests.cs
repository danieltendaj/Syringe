using System;
using NUnit.Framework;
using Syringe.Core.Reader;

namespace Syringe.Tests.Integration.Reader
{
    [TestFixture]
    public class EmbeddedFileReaderTests
    {
        private const string namespacePath = "Syringe.Tests.Integration.Xml.";

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Get_should_throw_exception_when_file_not_found()
        {
            // given
            var embededFileReader = new EmbeddedFileReader(namespacePath);

            // when
            embededFileReader.Get("filenotexist.xml");
        }

        [Test]
        public void Get_file_should_not_be_empty_when_file_found()
        {
            // given
            var embededFileReader = new EmbeddedFileReader(namespacePath);

            // when
            var textReader = embededFileReader.Get("roadkill-login.xml");

            // then
            Assert.NotNull(textReader);
        }
    }
}
