using System;
using System.Collections.Generic;
using System.IO;

namespace Authenticator.Network
{
	internal class PacketManager
	{
		internal static PacketManager.Result Add(byte[] data, out bool isContinueRec, ref Queue<byte[]> processedPackets)
		{
			isContinueRec = false;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					if (PacketManager._data.Count > 0)
					{
						long num = (long)((ulong)PacketManager._expectedlen - (ulong)((long)(PacketManager._data.Count - 4)));
						int num2 = (int)((num > (long)data.Length) ? ((long)data.Length) : num);
						PacketManager._data.AddRange(binaryReader.ReadBytes(num2));
						if (num - (long)num2 == 0L)
						{
							object lockerObj = PacketManager._lockerObj;
							lock (lockerObj)
							{
								byte[] item = PacketManager._data.ToArray();
								PacketManager._data.Clear();
								processedPackets.Enqueue(item);
								goto IL_B7;
							}
						}
						isContinueRec = true;
						IL_B7:
						if (num2 < data.Length)
						{
							return PacketManager.Add(binaryReader.ReadBytes((int)(memoryStream.Length - memoryStream.Position)), out isContinueRec, ref processedPackets);
						}
					}
					else
					{
						if (data.Length < 5)
						{
							isContinueRec = true;
							return PacketManager.Result.OK;
						}
						PacketManager._expectedlen = binaryReader.ReadUInt32();
						if (PacketManager._expectedlen > 8192u)
						{
							PacketManager._data.Clear();
							return PacketManager.Result.MaxLenLimit;
						}
						PacketManager._data.AddRange(BitConverter.GetBytes(PacketManager._expectedlen));
						return PacketManager.Add(binaryReader.ReadBytes((int)(memoryStream.Length - memoryStream.Position)), out isContinueRec, ref processedPackets);
					}
				}
			}
			return PacketManager.Result.OK;
		}

		private const int MAX_RECEIVE_SIZE = 8192;

		private static uint _expectedlen;

		private static List<byte> _data = new List<byte>();

		private static object _lockerObj = new object();

		internal enum Result
		{
			OK,
			MaxLenLimit
		}
	}
}
