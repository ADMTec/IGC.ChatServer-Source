using System;
using System.Linq;
using System.Timers;

namespace ChatServer
{
	public static class Database
	{
		public static void InitLogsClearTimer()
		{
			if (Configs.MessageLogCleanerSaveLastDays == 0)
			{
				return;
			}
			Database.ClearLogHistory();
			Timer timer = new Timer();
			timer.Interval = 86400000.0;
			timer.AutoReset = true;
			timer.Elapsed += delegate(object o, ElapsedEventArgs args)
			{
				Database.ClearLogHistory();
			};
			timer.Start();
		}

		private static void ClearLogHistory()
		{
			Form_Main.Log.Write("[Database] Message Logs Cleaner Job Run", Logger.LogType.Normal);
			SQL.Execute("DELETE FROM IGC_FriendChat_MessageLog WHERE GETUTCDATE() - [Date] > ?", new object[]
			{
				Configs.MessageLogCleanerSaveLastDays
			});
		}

		public static SQL.SQLResult GetMessageLogs(string Name, string FriendName, int Count)
		{
			return SQL.Select(string.Format("SELECT * FROM (SELECT TOP {0} [ID], [Name], [Text] FROM IGC_FriendChat_MessageLog WHERE (Name = ? AND FriendName = ?) OR (FriendName = ? AND Name = ?) ORDER BY [ID] DESC) AS RESULT ORDER BY ID", Count), new object[]
			{
				Name,
				FriendName,
				Name,
				FriendName
			});
		}

		public static bool IPBanned(string IP)
		{
			if (IP.Contains(':'))
			{
				IP = IP.Split(new char[]
				{
					':'
				})[0];
			}
			bool result;
			using (SQL.SQLResult sqlresult = SQL.Select("IF EXISTS (SELECT 1 FROM IGC_FriendChat_BannedIPs WHERE IP = ?) SELECT 'True' ELSE Select 'False'", new object[]
			{
				IP
			}))
			{
				result = sqlresult.Read<bool>(0, null, 0);
			}
			return result;
		}

		public static bool CharacterBanned(string Name)
		{
			bool result;
			using (SQL.SQLResult sqlresult = SQL.Select("IF EXISTS (SELECT 1 FROM IGC_FriendChat_BannedCharacters WHERE Name = ?) SELECT 'True' ELSE Select 'False'", new object[]
			{
				Name
			}))
			{
				result = sqlresult.Read<bool>(0, null, 0);
			}
			return result;
		}
	}
}
