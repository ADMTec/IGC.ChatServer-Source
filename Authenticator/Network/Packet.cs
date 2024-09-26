using System;
using System.Collections.Generic;
using System.Linq;

namespace Authenticator.Network
{
	internal class Packet
	{
		public bool IsValid
		{
			get
			{
				return !this.IsCryptFail && this.Length > -1;
			}
		}

		public Packet(byte[] data)
		{
			Array.Copy(data, 4, data, 0, data.Length - 4);
			Array.Resize<byte>(ref data, data.Length - 4);
			if (data.Length != 2 || data[0] != 255)
			{
				data = Cipher.Decrypt(data);
				if (data == null)
				{
					this.IsCryptFail = true;
					return;
				}
			}
			this.Length = ((data == null || data.Length == 0) ? -1 : data.Length);
			if (this.Length != -1)
			{
				this.Code = data[0];
				Array.Copy(data, 1, this.Data = new byte[data.Length - 1], 0, this.Data.Length);
			}
		}

		public static byte[] Create(byte code, byte[] data)
		{
			List<byte> list = new List<byte>();
			list.Add(code);
			list.AddRange(data);
			list = Cipher.Encrypt(list.ToArray()).ToList<byte>();
			if (list == null)
			{
				return null;
			}
			list.InsertRange(0, BitConverter.GetBytes((uint)list.Count));
			return list.ToArray();
		}

		public readonly int Length = -1;

		public readonly byte Code;

		public readonly byte[] Data;

		public readonly bool IsCryptFail;
	}
}
