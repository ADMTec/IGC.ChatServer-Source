using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;

namespace ChatServer
{
	internal static class Configs
	{
		public static int Port { get; private set; }

		public static int ExDB_Port { get; private set; }

		public static string ExDB_IP { get; private set; }

		public static string DatabaseName { get; private set; }

		public static string DatabaseAddress { get; private set; }

		public static int DatabasePort { get; private set; }

		public static string DatabaseUser { get; private set; }

		public static string DatabasePassword { get; private set; }

		public static int ChatHistorySendCount { get; private set; }

		public static int MessageLogCleanerSaveLastDays { get; private set; }

		public static bool LogMessagesToDatabase { get; private set; }

		public static string LanguageFileName { get; private set; }

		public static Encoding LanguageEncoding { get; private set; }

		public static void Read()
		{
			if (!File.Exists("config.ini"))
			{
				MessageBox.Show("'config.ini' file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Environment.Exit(0);
				return;
			}
			IniData iniData = new FileIniDataParser().ReadFile("config.ini");
			Configs.Port = int.Parse(iniData["Network"]["Port"] ?? "55980");
			Configs.ExDB_Port = int.Parse(iniData["Network"]["ExDB_Port"] ?? "55906");
			Configs.ExDB_IP = (iniData["Network"]["ExDB_IP"] ?? "127.0.0.1");
			Configs.DatabaseName = (iniData["Database"]["Name"] ?? "MuOnline");
			Configs.DatabaseAddress = (iniData["Database"]["Address"] ?? "(local)");
			Configs.DatabasePort = int.Parse(iniData["Database"]["Port"] ?? "1433");
			Configs.DatabaseUser = (iniData["Database"]["User"] ?? string.Empty);
			Configs.DatabasePassword = (iniData["Database"]["Password"] ?? string.Empty);
			Configs.ChatHistorySendCount = int.Parse(iniData["General"]["ChatHistorySendCount"] ?? " 3");
			Configs.MessageLogCleanerSaveLastDays = int.Parse(iniData["General"]["MessageLogCleanerSaveLastDays"] ?? "10");
			Configs.LogMessagesToDatabase = bool.Parse(iniData["General"]["LogMessagesToDatabase"] ?? "True");
			Configs.LanguageFileName = (iniData["General"]["LanguageFileName"] ?? "English.ini");
			Configs.LanguageEncoding = Encoding.GetEncoding(int.Parse(iniData["General"]["LanguageCodepage"] ?? "65001"));
		}
	}
}
