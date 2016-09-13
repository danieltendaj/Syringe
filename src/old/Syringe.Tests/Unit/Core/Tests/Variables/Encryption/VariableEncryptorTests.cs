using NUnit.Framework;
using Syringe.Core.Tests.Variables.Encryption;

namespace Syringe.Tests.Unit.Core.Tests.Variables.Encryption
{
	public class VariableEncryptorTests
	{
		[Test]
		public void decrypt_should_ignore_value_that_doesnt_start_with_identifier()
		{
			// given
			string value = "jcydwHTHkdPHlUZudXKhcw==";
			var encryptor = new VariableEncryptor(new AesEncryption("my password"));

			// when
			string actualValue = encryptor.Decrypt(value);

			// then
			Assert.That(actualValue, Is.EqualTo(value));
		}

		[Test]
		public void should_encrypt_value_and_add_prefix()
		{
			// given
			string plainText = "shut the door";
			string expectedValue = "enc:jcydwHTHkdPHlUZudXKhcw==";

			var encryptor = new VariableEncryptor(new AesEncryption("my password"));

			// when
			string actualValue = encryptor.Encrypt(plainText);

			// then
			Assert.That(actualValue, Is.EqualTo(expectedValue));
		}

		[Test]
		public void should_decrypt_value_when_value_starts_with_encryption_identifier()
		{
			// given
			string expectedValue = "shut the door";
			string plainText = "enc:jcydwHTHkdPHlUZudXKhcw==";

			var encryptor = new VariableEncryptor(new AesEncryption("my password"));

			// when
			string actualValue = encryptor.Decrypt(plainText);

			// then
			Assert.That(actualValue, Is.EqualTo(expectedValue));
		}
	}
}