using Syringe.Core.Tests.Variables.Encryption;

namespace Syringe.Tests.StubsMocks
{
	public class VariableEncryptorStub : IVariableEncryptor
	{
		public string Encrypt(string value, bool includePrefixInOutput = true)
		{
			return value;
		}

		public string Decrypt(string encryptedValue)
		{
			return encryptedValue;
		}
	}
}