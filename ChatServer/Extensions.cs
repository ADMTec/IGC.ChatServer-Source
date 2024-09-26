using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ChatServer
{
	public static class Extensions
	{
		public static string ToStringEx(this byte[] array, bool nullTerminated = true)
		{
			int count = array.Length;
			if (nullTerminated)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == 0)
					{
						count = i;
						break;
					}
				}
			}
			return Configs.LanguageEncoding.GetString(array, 0, count);
		}

		public static T Cast<T>(this byte[] buffer)
		{
			GCHandle gchandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			T result = (T)((object)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(T)));
			gchandle.Free();
			return result;
		}

		public static byte[] Cast<T>(this T type)
		{
			byte[] array = new byte[Marshal.SizeOf<T>(type)];
			GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			Marshal.StructureToPtr<T>(type, gchandle.AddrOfPinnedObject(), false);
			gchandle.Free();
			return array;
		}

		public static char[] ToFixedArray(this string val, int fixedLen, bool isForceZeroTerminated = false)
		{
			if (val.Length > fixedLen)
			{
				val.Substring(0, fixedLen);
			}
			val = val.PadRight(fixedLen, '\0');
			if (isForceZeroTerminated)
			{
				val = string.Format("{0}\0", val.Substring(0, fixedLen - 1));
			}
			return val.ToArray<char>();
		}

		public static byte[] ToFixedArrayEx(this string val, int fixedLen)
		{
			Configs.LanguageEncoding.GetByteCount(val);
			byte[] bytes = Configs.LanguageEncoding.GetBytes(val);
			Array.Resize<byte>(ref bytes, fixedLen);
			return bytes;
		}
	}
}
