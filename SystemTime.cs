﻿using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public class SystemTime
{
	public short wYear;

	public short wMonth;

	public short wDayOfWeek;

	public short wDay;

	public short wHour;

	public short wMinute;

	public short wSecond;

	public short wMilliseconds;
}
