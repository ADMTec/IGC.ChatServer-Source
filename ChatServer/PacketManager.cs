using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChatServer
{
	internal class PacketManager : IDisposable
	{
		public void Dispose()
		{
			try
			{
				using (this._packetBuilder)
				{
				}
			}
			catch
			{
			}
		}

		public IEnumerable<byte[]> Process(byte[] buffer, int offset, int remReadLen)
		{
			if (this._packetSize == -1)
			{
				if (this._packetBuilder != null)
				{
					using (this._packetBuilder)
					{
					}
				}
				switch (buffer[offset])
				{
				case 193:
				case 195:
					this._packetSize = (int)buffer[offset + 1];
					break;
				case 194:
				case 196:
				{
					byte[] array = new byte[2];
					Array.Copy(buffer, offset + 1, array, 0, array.Length);
					array = array.Reverse<byte>().ToArray<byte>();
					this._packetSize = (int)BitConverter.ToUInt16(array, 0);
					break;
				}
				default:
					yield break;
				}
				this._packetBuilder = new MemoryStream();
			}
			int num = this._packetSize - (int)this._packetBuilder.Position;
			int num2 = (remReadLen > num) ? num : remReadLen;
			this._packetBuilder.Write(buffer, offset, num2);
			offset += num2;
			remReadLen -= num2;
			if (this._packetBuilder.Position == (long)this._packetSize)
			{
				this._packetSize = -1;
				yield return this._packetBuilder.ToArray();
			}
			if (remReadLen <= 0)
			{
				yield break;
			}
			foreach (byte[] array2 in this.Process(buffer, offset, remReadLen))
			{
				yield return array2;
			}
			IEnumerator<byte[]> enumerator = null;
			yield break;
			yield break;
		}

		private MemoryStream _packetBuilder;

		private int _packetSize = -1;
	}
}
