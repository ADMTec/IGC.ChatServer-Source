using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using ChatServer;

public class Logger
{
	public Logger(ref RichTextBox rtb, int logRefreshTime, string logFilePath = "Data\\Logs\\")
	{
		this.rtb = rtb;
		this.logFilePath = logFilePath;
		this.logRefreshTime = logRefreshTime;
		this.InitLoggerClass();
	}

	public Logger(int logRefreshTime, string logFilePath = "Data\\Logs\\")
	{
		this.logFilePath = logFilePath;
		this.logRefreshTime = logRefreshTime;
		this.InitLoggerClass();
	}

	private void InitLoggerClass()
	{
		if (!this.logFilePath.EndsWith("\\"))
		{
			this.logFilePath += "\\";
		}
		if (this.rtb != null)
		{
			this.LogQueue = new ConcurrentQueue<Logger.LogLine>();
			this.rtb.ScrollToCaret();
			this.tmr_LogRefresh = new System.Timers.Timer();
			this.tmr_LogRefresh.Interval = (double)this.logRefreshTime;
			this.tmr_LogRefresh.AutoReset = false;
			this.tmr_LogRefresh.Elapsed += delegate(object o, ElapsedEventArgs args)
			{
				if (Form_Main.IsClosing)
				{
					return;
				}
				this.WriteLogsQueue();
				this.tmr_LogRefresh.Start();
			};
			this.tmr_LogRefresh.Start();
		}
	}

	public void Write(string text, Logger.LogType type = Logger.LogType.Normal)
	{
		if (Form_Main.IsClosing)
		{
			return;
		}
		this.LogQueue.Enqueue(new Logger.LogLine(text, type));
	}

	private void WriteLogsQueue()
	{
		if (this.LogQueue.Count == 0)
		{
			return;
		}
		while (this.LogQueue.Count > 0)
		{
			Logger.LogLine log;
			if (this.LogQueue.TryDequeue(out log) && log != null)
			{
				this.WriteLogToFile(log);
				this.rtb.Parent.Invoke(new MethodInvoker(delegate()
				{
					if (this.rtb != null)
					{
						string text = string.Format("[{0}] {1} {2}\r", log.logTime.ToLongTimeString(), (log.logType == Logger.LogType.Normal) ? string.Empty : string.Format("[{0}]", log.logType.ToString()), log.text.Replace("\n", " ").Replace("\r", " "));
						int textLength = this.rtb.TextLength;
						this.rtb.AppendText(text);
						if (log.logType != Logger.LogType.Normal)
						{
							this.PaintLine(log.logType, textLength, text.Length - 1);
						}
					}
				}));
			}
		}
	}

	private void PaintLine(Logger.LogType logType, int offset, int textLen)
	{
		this.rtb.SelectionStart = offset;
		this.rtb.SelectionLength = 0;
		this.rtb.SelectionLength = textLen;
		if (logType != Logger.LogType.ERROR)
		{
			if (logType == Logger.LogType.WARNING)
			{
				this.rtb.SelectionColor = Color.Yellow;
			}
		}
		else
		{
			this.rtb.SelectionColor = Color.Red;
		}
		this.rtb.SelectionStart = this.rtb.TextLength;
		this.rtb.SelectionLength = 0;
		this.rtb.SelectionBackColor = this.rtb.BackColor;
		this.rtb.SelectionColor = this.rtb.ForeColor;
	}

	private void WriteLogToFile(Logger.LogLine line)
	{
		if (!Directory.Exists(this.logFilePath))
		{
			Directory.CreateDirectory(this.logFilePath);
		}
		if (!Directory.Exists(this.logFilePath + "Trash\\"))
		{
			Directory.CreateDirectory(this.logFilePath + "Trash\\");
		}
		FileInfo fileInfo = new FileInfo(this.logFilePath + "Trash\\" + line.logTime.ToString("yyyy-MM-dd") + ".log");
		if (fileInfo.Exists)
		{
			while (fileInfo.IsReadOnly)
			{
				Thread.Sleep(200);
				fileInfo.Refresh();
			}
		}
		try
		{
			using (StreamWriter streamWriter = new StreamWriter(fileInfo.FullName, true))
			{
				streamWriter.WriteLine(string.Format("[{0}] {1}", line.logTime.ToLongTimeString(), line.text));
			}
		}
		catch (Exception ex)
		{
			this.WriteLogToFile(new Logger.LogLine(string.Format("[Log Write Error] [{0}]", ex.Message), Logger.LogType.ERROR));
		}
		if (line.logType != Logger.LogType.Normal)
		{
			fileInfo = new FileInfo(this.logFilePath + ((line.logType == Logger.LogType.ERROR) ? "Errors.log" : ((line.logType == Logger.LogType.WARNING) ? "Warnings.log" : "")));
			if (fileInfo.Exists)
			{
				while (fileInfo.IsReadOnly)
				{
					Thread.Sleep(200);
				}
			}
			try
			{
				using (StreamWriter streamWriter2 = new StreamWriter(fileInfo.FullName, true))
				{
					streamWriter2.WriteLine(string.Format("[{0}] {1}", line.logTime.ToString(), line.text));
				}
			}
			catch (Exception ex2)
			{
				this.WriteLogToFile(new Logger.LogLine(string.Format("[Log Write Error] [{0}]", ex2.Message), Logger.LogType.ERROR));
			}
		}
	}

	private System.Timers.Timer tmr_LogRefresh;

	private RichTextBox rtb;

	private ConcurrentQueue<Logger.LogLine> LogQueue;

	private string logFilePath;

	private int logRefreshTime;

	public enum LogType
	{
		Normal,
		ERROR,
		WARNING
	}

	private class LogLine
	{
		public LogLine(string text, Logger.LogType logType)
		{
			this.text = text;
			this.logType = logType;
			this.logTime = DateTime.Now;
		}

		public string text;

		public DateTime logTime;

		public Logger.LogType logType;
	}
}
