using System;
using System.Security.Cryptography;
using System.Text;

namespace Authenticator
{
	internal static class Cipher
	{
		private static string _key
		{
			get
			{
				return "3ZKHAkzfJa2H2SFk";
			}
		}

		private static byte[] _iv
		{
			get
			{
				return Encoding.UTF8.GetBytes("6VuJCthyu7B7YksK");
			}
		}

		private static byte[] crypt(byte[] data, bool encrypt)
		{
			byte[] result;
			try
			{
				using (Aes aes = Aes.Create())
				{
					aes.Padding = PaddingMode.PKCS7;
					aes.Mode = CipherMode.CBC;
					using (PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(Cipher._key, null))
					{
						using (ICryptoTransform cryptoTransform = encrypt ? aes.CreateEncryptor(passwordDeriveBytes.GetBytes(32), Cipher._iv) : aes.CreateDecryptor(passwordDeriveBytes.GetBytes(32), Cipher._iv))
						{
							result = cryptoTransform.TransformFinalBlock(data, 0, data.Length);
						}
					}
				}
			}
			catch (Exception ex)
			{
				EventLogManager.Error(ex.Message, null);
				result = null;
			}
			return result;
		}

		internal static byte[] Decrypt(byte[] data)
		{
			return Cipher.crypt(data, false);
		}

		internal static byte[] Encrypt(byte[] data)
		{
			return Cipher.crypt(data, true);
		}
	}
}
