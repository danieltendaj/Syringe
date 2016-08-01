namespace Syringe.Core.Tests.Variables.Encryption
{
	public class VariableEncryptor : IVariableEncryptor
	{
		private readonly IEncryption _encryption;

		public static string ValuePrefix => "enc:";

		public VariableEncryptor(IEncryption encryption)
		{
			_encryption = encryption;
		}

		/// <summary>
		/// Encrypts the value, optionally adding "enc:" to the encrypted string to 
		/// signal that it's an encrypted string to Syringe.
		/// </summary>
		public string Encrypt(string value, bool includePrefixInOutput = true)
		{
			string encryptedValue = _encryption.Encrypt(value);

			if (includePrefixInOutput)
				encryptedValue = ValuePrefix + encryptedValue;

			return encryptedValue;
		}

		/// <summary>
		/// Decrypts a value if it begins with the "enc:" prefix; otherwise returns the value.
		/// </summary>
		public string Decrypt(string encryptedValue)
		{
			if (string.IsNullOrEmpty(encryptedValue) || !encryptedValue.StartsWith(ValuePrefix) || encryptedValue == ValuePrefix)
				return encryptedValue;

			encryptedValue = encryptedValue.Substring(ValuePrefix.Length);

			return _encryption.Decrypt(encryptedValue);
		}
	}
}