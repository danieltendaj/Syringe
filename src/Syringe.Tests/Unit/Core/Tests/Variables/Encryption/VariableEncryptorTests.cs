using NUnit.Framework;
using Syringe.Core.Tests.Variables.Encryption;

namespace Syringe.Tests.Unit.Core.Tests.Variables.Encryption
{
	public class VariableEncryptorTests
	{
		[Test]
		public void should_ignore_value_that_doesnt_start_with_identifier()
		{
			// given
			string value = "shut the door";
			var encryptor = new VariableEncryptor(new RijndaelEncryption("my password"));

			// when
			string actualValue = encryptor.Encrypt(value);

			// then
			Assert.That(actualValue, Is.EqualTo(value));
		}

		[Test]
		public void should_encrypt_when_value_starts_with_encryption_identifier()
		{
			// given
			string plainText = "enc:shut the door";
			string expectedValue = "jcydwHTHkdPHlUZudXKhcw==";

			var encryptor = new VariableEncryptor(new RijndaelEncryption("my password"));

			// when
			string actualValue = encryptor.Encrypt(plainText);

			// then
			Assert.That(actualValue, Is.EqualTo(expectedValue));
		}

		[Test]
		public void should_decrypt_value()
		{
			// given
			string expectedValue = "shut the door";
			string plainText = "jcydwHTHkdPHlUZudXKhcw==";

			var encryptor = new VariableEncryptor(new RijndaelEncryption("my password"));

			// when
			string actualValue = encryptor.Decrypt(plainText);

			// then
			Assert.That(actualValue, Is.EqualTo(expectedValue));
		}
	}
}