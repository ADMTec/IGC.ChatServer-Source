using System;
using System.Diagnostics;

namespace Authenticator
{
	public class EventLogManager
	{
		public static void Information(string text, string sourceName = null)
		{
			EventLogManager.Write(text, EventLogEntryType.Information, sourceName);
		}

		public static void Error(string text, string sourceName = null)
		{
			EventLogManager.Write(text, EventLogEntryType.Error, sourceName);
		}

		public static void Warning(string text, string sourceName = null)
		{
			EventLogManager.Write(text, EventLogEntryType.Warning, sourceName);
		}

		private static void Write(string text, EventLogEntryType type, string sourceName = null)
		{
			sourceName = (sourceName ?? EventLogManager.SRC_NAME());
			EventLogManager.validateSource(sourceName);
			try
			{
				EventLog.WriteEntry(sourceName, text, type);
			}
			catch
			{
			}
		}

		private static void validateSource(string sourceName)
		{
			try
			{
				if (!EventLog.SourceExists(sourceName))
				{
					EventLog.CreateEventSource(sourceName, "IGC Tools");
				}
			}
			catch
			{
			}
		}

		private const string LOG_NAME = "IGC Tools";

		public static Func<string> SRC_NAME = () => "Authenticator";
	}
}
