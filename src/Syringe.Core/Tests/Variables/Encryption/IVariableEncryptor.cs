namespace Syringe.Core.Tests.Variables.Encryption
{
	public interface IVariableEncryptor
	{
		string Encrypt(string value, bool includePrefixInOutput = true);
		string Decrypt(string encryptedValue);
	}
}