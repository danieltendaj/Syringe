using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Syringe.Core.Tests.Variables.Encryption
{
	/// <summary>
	/// Simple AES wrapper for with a randomnly generated IV, and a static salt.
	/// </summary>
	public class AesEncryption : IEncryption
	{
		private static readonly byte[] _salt = new byte[]
		{
		    0x3c, 0x8b, 0x08, 0x00,
            0x35, 0xe9, 0xf7, 0x45,
            0x6e, 0xa8, 0xbb, 0xe4,
            0x6b, 0x4a, 0xd0, 0x0b
		};

		private readonly Aes _aes;

		internal string Password { get; set; } // SecureString this?

		public AesEncryption(string password)
		{
			if (string.IsNullOrEmpty(password))
				return;

            // Some of this setup is for Rijndael - the no padding may not be necessary.
			Password = password;
			_aes = Aes.Create();
			_aes.Padding = PaddingMode.Zeros;
			Rfc2898DeriveBytes pdb = null;

			try
			{
				pdb = new Rfc2898DeriveBytes(password, _salt);
				_aes.Key = pdb.GetBytes(32);
				_aes.IV = pdb.GetBytes(16);
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
			if (_aes == null)
				return plainValue;

			try
			{
				using (var encryptor = _aes.CreateEncryptor())
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
			catch (Exception ex)
			{
                throw new Exception(string.Format("Error encrypting value {0} - {1}", plainValue, ex));
			}
		}

		public string Decrypt(string encryptedValue)
		{
			if (_aes == null)
				return encryptedValue;

			try
			{
				using (var decryptor = _aes.CreateDecryptor())
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
			catch (Exception ex)
			{
			    return encryptedValue;
			}
		}

		public void Dispose()
		{
			_aes?.Dispose();
		}
	}
}