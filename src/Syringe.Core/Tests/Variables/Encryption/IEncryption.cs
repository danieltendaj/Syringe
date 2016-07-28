using System;

namespace Syringe.Core.Tests.Variables.Encryption
{
	public interface IEncryption : IDisposable
	{
		/// <summary>
		/// Takes an encrypted Base64 format string and returns a plain text encrypted value.
		/// </summary>
		string Decrypt(string encryptedValue);

		/// <summary>
		/// Takes a plain text string and returns an encrypted value, in Base64 format.
		/// </summary>
		string Encrypt(string plainValue);
	}
}