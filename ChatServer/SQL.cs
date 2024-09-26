using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Timers;

namespace ChatServer
{
	public static class SQL
	{
		public static void Init()
		{
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
			sqlConnectionStringBuilder.DataSource = Configs.DatabaseAddress + "," + Configs.DatabasePort.ToString();
			sqlConnectionStringBuilder.InitialCatalog = Configs.DatabaseName;
			if (sqlConnectionStringBuilder.InitialCatalog.StartsWith("[") && sqlConnectionStringBuilder.InitialCatalog.EndsWith("]"))
			{
				sqlConnectionStringBuilder.InitialCatalog = sqlConnectionStringBuilder.InitialCatalog.Substring(0, sqlConnectionStringBuilder.InitialCatalog.Length - 1).Substring(1);
			}
			if (Configs.DatabaseAddress.ToLower() == "(local)")
			{
				sqlConnectionStringBuilder.IntegratedSecurity = true;
			}
			else
			{
				sqlConnectionStringBuilder.IntegratedSecurity = false;
				sqlConnectionStringBuilder.Password = Configs.DatabasePassword;
				sqlConnectionStringBuilder.UserID = Configs.DatabaseUser;
			}
			sqlConnectionStringBuilder.MultipleActiveResultSets = true;
			sqlConnectionStringBuilder.AsynchronousProcessing = true;
			SQL.LocalConnectionString = sqlConnectionStringBuilder.ConnectionString;
			Timer tmr_PendingQueries = new Timer();
			tmr_PendingQueries.Interval = 300.0;
			tmr_PendingQueries.Elapsed += delegate(object o, ElapsedEventArgs args)
			{
				while (SQL.PendingQueries.Count > 0)
				{
					SQL.MyQuery sqlQuery;
					if (SQL.PendingQueries.TryDequeue(out sqlQuery))
					{
						SQL.Execute(sqlQuery);
					}
				}
				tmr_PendingQueries.Start();
			};
			tmr_PendingQueries.Start();
		}

		public static void Execute(string query, params object[] args)
		{
			List<SqlParameter> list = new List<SqlParameter>(args.Length);
			for (int i = 0; i < args.Length; i++)
			{
				string text = "@" + i.ToString();
				int startIndex = query.IndexOf('?');
				query = query.Remove(startIndex, 1).Insert(startIndex, text);
				list.Add(new SqlParameter(text, args[i]));
			}
			new StringBuilder().AppendFormat(CultureInfo.GetCultureInfo("en-US").NumberFormat, query, Array.Empty<object>());
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(SQL.LocalConnectionString))
				{
					sqlConnection.Open();
					using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
					{
						sqlCommand.CommandTimeout = 0;
						sqlCommand.Parameters.AddRange(list.ToArray());
						sqlCommand.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				SQL.SQLError(query, ex.Message.Replace("\n", ". "));
			}
		}

		private static void Execute(SQL.MyQuery sqlQuery)
		{
			SQL.Execute(sqlQuery.Command, sqlQuery.Args);
		}

		internal static bool Start()
		{
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(SQL.LocalConnectionString))
				{
					sqlConnection.Open();
				}
			}
			catch (Exception ex)
			{
				Form_Main.Log.Write(string.Format("\t-> [Failed] {0}", ex.Message.Replace("\n", ". ")), Logger.LogType.ERROR);
				return false;
			}
			Form_Main.Log.Write(string.Format("\t-> [Success]", Array.Empty<object>()), Logger.LogType.Normal);
			return true;
		}

		public static void EnqueueQuery(string command, params object[] args)
		{
			SQL.PendingQueries.Enqueue(new SQL.MyQuery(command, args));
		}

		private static void SQLError(string command, string error)
		{
			Form_Main.Log.Write(string.Format("[SQL] [{0}] [{1}]", error, command), Logger.LogType.WARNING);
		}

		public static SQL.SQLResult Select(string query, params object[] args)
		{
			List<SqlParameter> list = new List<SqlParameter>(args.Length);
			for (int i = 0; i < args.Length; i++)
			{
				string text = "@" + i.ToString();
				int startIndex = query.IndexOf('?');
				query = query.Remove(startIndex, 1).Insert(startIndex, text);
				list.Add(new SqlParameter(text, args[i]));
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.GetCultureInfo("en-US").NumberFormat, query, Array.Empty<object>());
			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(SQL.LocalConnectionString))
				{
					sqlConnection.Open();
					using (SqlCommand sqlCommand = new SqlCommand(stringBuilder.ToString(), sqlConnection))
					{
						sqlCommand.CommandTimeout = 0;
						sqlCommand.Parameters.AddRange(list.ToArray());
						using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.Default))
						{
							SQL.SQLResult sqlresult = new SQL.SQLResult();
							sqlresult.Load(sqlDataReader);
							sqlresult.Count = sqlresult.Rows.Count;
							return sqlresult;
						}
					}
				}
			}
			catch (Exception ex)
			{
				SQL.SQLError(query, ex.Message.Replace("\n", ". "));
			}
			return null;
		}

		private static ConcurrentQueue<SQL.MyQuery> PendingQueries = new ConcurrentQueue<SQL.MyQuery>();

		private static string LocalConnectionString;

		public class SQLResult : DataTable
		{
			public int Count { get; set; }

			public T Read<T>(int row = 0, string columnName = null, int number = 0)
			{
				object value;
				if (columnName == null)
				{
					value = base.Rows[row][0];
				}
				else
				{
					value = base.Rows[row][columnName + ((number != 0) ? (checked(1 + number)).ToString() : "")];
				}
				if (typeof(T).IsEnum)
				{
					return (T)((object)Enum.ToObject(typeof(T), value));
				}
				return (T)((object)Convert.ChangeType(value, typeof(T)));
			}

			public object[] ReadAllValuesFromField(string columnName)
			{
				object[] array = new object[this.Count];
				for (int i = 0; i < this.Count; i++)
				{
					array[i] = base.Rows[i][columnName];
				}
				return array;
			}
		}

		private class MyQuery
		{
			public MyQuery(string command, object[] args)
			{
				this.Command = command;
				this.Args = args;
			}

			public string Command;

			public object[] Args;
		}
	}
}
