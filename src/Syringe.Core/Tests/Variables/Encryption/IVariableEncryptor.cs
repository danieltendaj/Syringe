namespace Syringe.Core.Tests.Variables.Encryption
{
	public interface IVariableEncryptor
	{
		string Encrypt(string value);
		string Decrypt(string encryptedValue);
	}
}