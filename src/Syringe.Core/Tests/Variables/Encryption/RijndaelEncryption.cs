using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Syringe.Core.Logging;

namespace Syringe.Core.Tests.Variables.Encryption
{
	/// <summary>
	/// Simple Rijndael wrapper for with a randomnly generated IV.
	/// </summary>
	public class RijndaelEncryption : IEncryption
	{
		private static readonly byte[] SALT = new byte[] { 0x3c, 0x8b, 0x08, 0x00, 0x35, 0xe9, 0xf7, 0x45, 0x6e, 0xa8, 0xbb, 0xe4, 0x6b, 0x4a, 0xd0, 0x0b };

		private readonly Rijndael _rijndael;

		public RijndaelEncryption(string password)
		{
			if (string.IsNullOrEmpty(password))
				return;

			_rijndael = Rijndael.Create();
			_rijndael.Padding = PaddingMode.Zeros;
			Rfc2898DeriveBytes pdb = null;

			try
			{
				pdb = new Rfc2898DeriveBytes(password, SALT);
				_rijndael.Key = pdb.GetBytes(32);
				_rijndael.IV = pdb.GetBytes(16);
			}
			finally
			{
				IDisposable disp = pdb as IDisposable;

				if (disp != null)
				{
					disp.Dispose();
				}
			}
		}

		public string Encrypt(string plainValue)
		{
			if (_rijndael == null)
				return plainValue;

			using (var encryptor = _rijndael.CreateEncryptor())
			using (var stream = new MemoryStream())
			using (var crypto = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(plainValue);

				crypto.Write(bytes, 0, bytes.Length);
				crypto.FlushFinalBlock();
				stream.Position = 0;
				var encrypted = new byte[stream.Length];
				stream.Read(encrypted, 0, encrypted.Length);

				return Convert.ToBase64String(encrypted);
			}
		}

		public string Decrypt(string encryptedValue)
		{
			if (_rijndael == null)
				return encryptedValue;

			try
			{
				using (var decryptor = _rijndael.CreateDecryptor())
				using (var stream = new MemoryStream())
				using (var crypto = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
				{
					byte[] bytes = Convert.FromBase64String(encryptedValue);

					crypto.Write(bytes, 0, bytes.Length);
					crypto.FlushFinalBlock();
					stream.Position = 0;
					var decryptedBytes = new byte[stream.Length];
					stream.Read(decryptedBytes, 0, decryptedBytes.Length);

					return Encoding.UTF8.GetString(decryptedBytes).TrimEnd('\0');
				}
			}
			catch (FormatException ex)
			{
				Log.Error(ex, "Error decrypting value {0}", encryptedValue);
				return encryptedValue;
			}
		}

		public void Dispose()
		{
			_rijndael?.Dispose();
		}
	}
}