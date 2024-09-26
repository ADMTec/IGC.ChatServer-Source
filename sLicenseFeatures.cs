using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public class sLicenseFeatures
{
	public int cb;

	public int NumDays;

	public int NumExec;

	public SystemTime ExpDate;

	public int CountryId;

	public int Runtime;

	public int GlobalMinutes;

	public SystemTime InstallDate;

	public int NetInstances;

	public int EmbedLicenseInfoInKey;

	public int EmbedCreationDate;
}
