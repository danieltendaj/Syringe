using NUnit.Framework;
using Syringe.Core.Tests.Variables.Encryption;

namespace Syringe.Tests.Unit.Core.Tests.Variables.Encryption
{
	public class VariableEncryptorTests
	{
		[Test]
		public void should_ignore_value_that_doesnt_start_with_prefix()
		{
			// given
			string value = "shut the door";
			var encryptor = new VariableEncryptor(new RijndaelEncryption("blah"));

			// when
			string actualValue = encryptor.Encrypt(value);

			// then
			Assert.That(actualValue, Is.EqualTo(value));
		}
	}
}