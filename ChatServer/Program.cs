using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Authenticator;

namespace ChatServer
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += Dumper.CurrentDomain_UnhandledException;
			TaskScheduler.UnobservedTaskException += Dumper.TaskScheduler_UnobservedTaskException;
			Application.ThreadException += Dumper.Application_ThreadException;
			CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
			Application.CurrentCulture = cultureInfo;
			Thread.CurrentThread.CurrentCulture = cultureInfo;
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form_Main());
		}
	}
}
