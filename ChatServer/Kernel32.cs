using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ChatServer
{
	internal static class Kernel32
	{
		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern bool GetEnvironmentVariable(string lpName, StringBuilder lpBuffer, int nSize);
	}
}
