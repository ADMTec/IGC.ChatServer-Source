using System;
using System.Text;
using System.Windows.Forms;
using Authenticator.Models;
using Authenticator.Network;
using ToolsAuthServer.Models;

namespace Authenticator
{
	public static class Main
	{
		public static void Initiate(Type type1, Type type2, ToolIDs id, Action<bool, byte[]> callback, params string[] addVals)
		{
			Protocol.Initialize();
			Client.Initialize();
			if (id == ToolIDs.General || id == ToolIDs.Launcher)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				int num = 0;
				if (WinlicenseSDK.WLRegGetStatus(ref num) != 1)
				{
					try
					{
						Form form = Activator.CreateInstance(type2) as Form;
						(form as INoLicenseForm).AuthenticationError = new ErrorCodes?(ErrorCodes.PUBLIC_LicenseNotFound);
						Application.Run(form);
					}
					catch
					{
					}
					Environment.Exit(0);
					return;
				}
			}
			for (int i = 0; i < addVals.Length; i++)
			{
				Protocol.AddVal += string.Format("{0,-15}", addVals[i]);
			}
			Protocol.ID = id;
			Protocol.Type1 = type1;
			Protocol.Type2 = type2;
			if (callback != null)
			{
				Protocol.Callback = callback;
			}
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			bool authServerChange = commandLineArgs.Length == 2 && commandLineArgs[1] == "-switch";
			Client.Connect(34338 + ((id != ToolIDs.Essential) ? 1 : 0), authServerChange, 1);
		}
	}
}
