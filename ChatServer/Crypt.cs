using System;

namespace ChatServer
{
	internal class Crypt
	{
		public static void BuxConvert(ref byte[] buf, int size)
		{
			for (int i = 0; i < size; i++)
			{
				byte[] array = buf;
				int num = i;
				array[num] ^= Crypt.bBuxCode[i % 3];
			}
		}

		public static byte[] BuxConvert(byte[] data, int size)
		{
			byte[] array = new byte[data.Length];
			data.CopyTo(array, 0);
			for (int i = 0; i < size; i++)
			{
				byte[] array2 = array;
				int num = i;
				array2[num] ^= Crypt.bBuxCode[i % 3];
			}
			return array;
		}

		public static void DecXor32(ref byte[] Buff, int DataOffset, int Len, int loopVal)
		{
			byte[] array = new byte[]
			{
				171,
				17,
				205,
				254,
				24,
				35,
				197,
				163,
				202,
				51,
				193,
				204,
				102,
				103,
				33,
				243,
				50,
				18,
				21,
				53,
				41,
				byte.MaxValue,
				254,
				29,
				68,
				239,
				205,
				65,
				38,
				60,
				78,
				77
			};
			for (int num = Len; num != DataOffset; num += loopVal)
			{
				byte[] array2 = Buff;
				int num2 = num;
				array2[num2] ^= (byte)(array[num % array.Length] ^ Buff[num - 1]);
			}
		}

		private static byte[] bBuxCode = new byte[]
		{
			252,
			207,
			171
		};
	}
}
