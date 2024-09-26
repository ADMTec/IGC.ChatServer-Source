using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

internal class Dumper
{
	public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		try
		{
			Exception ex = (Exception)e.ExceptionObject;
			Dumper.WriteNormalLog("(_UnhandledException) " + ex.Message.Replace("\r", string.Empty).Replace("\n", ". ") + "\n" + ex.StackTrace);
		}
		finally
		{
			MessageBox.Show("Critical Error Occured. Please submit the file 'Data\\Error.log' to the Developer. You may try your last operation again.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
	{
		try
		{
			Dumper.WriteNormalLog("(_UnobservedTaskException) " + e.Exception.Message.Replace("\r", string.Empty).Replace("\n", ". ") + "\n" + e.Exception.StackTrace);
		}
		finally
		{
			MessageBox.Show("Critical Error Occured. Please submit the file 'Data\\Error.log' to the Developer. You may try your last operation again.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
	{
		try
		{
			Dumper.WriteNormalLog("(_ThreadException) " + e.Exception.Message.Replace("\r", string.Empty).Replace("\n", ". ") + "\n" + e.Exception.StackTrace);
		}
		finally
		{
			MessageBox.Show("Critical Error Occured. Please submit the file 'Data\\Error.log' to the Developer. You may try your last operation again.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private static void WriteNormalLog(string Text)
	{
		while (Dumper.WritingLog)
		{
			Thread.Sleep(100);
		}
		Dumper.WritingLog = true;
		byte[] bytes = Encoding.Unicode.GetBytes(Text);
		byte[] array = new byte[]
		{
			170,
			188,
			203,
			43,
			205
		};
		for (int i = 0; i < bytes.Length; i++)
		{
			byte[] array2 = bytes;
			int num = i;
			array2[num] ^= array[i % 5];
		}
		Text = Encoding.Unicode.GetString(bytes);
		try
		{
			using (StreamWriter streamWriter = new StreamWriter("CrashLog.log", true))
			{
				streamWriter.WriteLine(string.Format("[{0}]\t{1}", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), Text));
			}
		}
		catch
		{
		}
		Dumper.WritingLog = false;
	}

	private static bool WritingLog;
}
