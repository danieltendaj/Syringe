using NUnit.Framework;
using Syringe.Core.Tests.Variables.Encryption;

namespace Syringe.Tests.Unit.Core.Tests.Variables.Encryption
{
	public class RijndaelEncryptionTests
	{
		[Test]
		public void encrypt_should_return_value_when_password_is_empty()
		{
			// given
			string plainText = "hands on your head";
			var encryption = new RijndaelEncryption("");

			// when
			string actualValue = encryption.Encrypt(plainText);

			// then
			Assert.That(actualValue, Is.EqualTo(plainText));
		}

		[Test]
		public void decrypt_should_return_value_when_password_is_empty()
		{
			// given
			string plainText = "touch my what?!";
			var encryption = new RijndaelEncryption("");

			// when
			string actualValue = encryption.Decrypt(plainText);

			// then
			Assert.That(actualValue, Is.EqualTo(plainText));
		}

		[Test]
		public void encrypt_should_encrypt_value_using_base64()
		{
			// given
			string plainText = "trap the Zapdos";
			string expectedValue = "fHFAc4zPBd3+pAE6Rf69IQ==";
			var encryption = new RijndaelEncryption("password");

			// when
			string actualValue = encryption.Encrypt(plainText);

			// then
			Assert.That(actualValue, Is.EqualTo(expectedValue));
		}

		[Test]
		public void decrypt_should_decrypt_base64_string()
		{
			// given
			string expectedValue= "trap the Zapdos";
			string encryptedValue = "fHFAc4zPBd3+pAE6Rf69IQ==";
			var encryption = new RijndaelEncryption("password");

			// when
			string actualValue = encryption.Decrypt(encryptedValue);

			// then
			Assert.That(actualValue, Is.EqualTo(expectedValue));
		}

		[Test]
		public void decrypt_return_original_value_for_bad_data()
		{
			// given
			string encryptedValue = "f";
			var encryption = new RijndaelEncryption("password");

			// when
			string actualValue = encryption.Decrypt(encryptedValue);

			// then
			Assert.That(actualValue, Is.EqualTo("f"));
		}
	}
}