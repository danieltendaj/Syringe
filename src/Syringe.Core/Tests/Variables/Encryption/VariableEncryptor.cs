namespace Syringe.Core.Tests.Variables.Encryption
{
	public class VariableEncryptor : IVariableEncryptor
	{
		private readonly IEncryption _encryption;

		public static string ValuePrefix
		{
			get { return "enc:"; }
		}

		public VariableEncryptor(IEncryption encryption)
		{
			_encryption = encryption;
		}

		public string Encrypt(string value)
		{
			if (string.IsNullOrEmpty(value) || !value.StartsWith(ValuePrefix) || value == ValuePrefix)
				return value;

			value = value.Substring(ValuePrefix.Length);

			return _encryption.Encrypt(value);
		}

		public string Decrypt(string encryptedValue)
		{
			return "";
		}
	}
}