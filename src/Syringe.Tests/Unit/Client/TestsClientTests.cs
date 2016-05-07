using System.Net;
using Moq;
using NUnit.Framework;
using RestSharp;
using Syringe.Client;
using Syringe.Client.RestSharpHelpers;
using Syringe.Tests.StubsMocks;

namespace Syringe.Tests.Unit.Client
{
    [TestFixture]
    public class TestsClientTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void CopyTest_should_call_expected_web_method(bool expectedResult)
        {
            // given
            const string expectedSeriveUrl = "some-url-init";
            const int expectedPosition = 123;
            const string expectedFileName = "lalalala.txt";

            var client = new RestClientMock
            {
                RestResponse = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = expectedResult.ToString().ToLower()
                }
            };

            var clientFactory = new Mock<IRestSharpClientFactory>();
            clientFactory
                .Setup(x => x.Create(expectedSeriveUrl))
                .Returns(client);

            // when
            var testsClient = new TestsClient(expectedSeriveUrl, clientFactory.Object);
            bool result = testsClient.CopyTest(expectedPosition, expectedFileName);

            // then
            Assert.That(result, Is.EqualTo(expectedResult));

            IRestRequest request = client.RestRequest;
            Assert.That(request.Method, Is.EqualTo(Method.POST));
            Assert.That(request.Resource, Is.EqualTo(TestsClient.RESOURCE_PATH + "/CopyTest"));
            Assert.That(request.Parameters.Count, Is.EqualTo(2));

            Parameter param1 = request.Parameters[0];
            Assert.That(param1.Name, Is.EqualTo("position"));
            Assert.That(param1.Type, Is.EqualTo(ParameterType.QueryString));
            Assert.That(param1.Value, Is.EqualTo(expectedPosition.ToString()));

            Parameter param2 = request.Parameters[1];
            Assert.That(param2.Name, Is.EqualTo("fileName"));
            Assert.That(param2.Type, Is.EqualTo(ParameterType.QueryString));
            Assert.That(param2.Value, Is.EqualTo(expectedFileName));
        }
    }
}