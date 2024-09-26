using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Network;

namespace ChatServer
{
	internal class IOCP
	{
		public static Task<bool> Start()
		{
			return Task.Run(() =>
                {
					return true;
                }
				);
		}

		public static void Stop()
		{
			IOCP.ChatServerIOCP chatServer = IOCP.ChatServer;
			if (chatServer != null)
			{
				chatServer.Stop();
			}
			IOCP.ExDBIOCP exDB = IOCP.ExDB;
			if (exDB == null)
			{
				return;
			}
			exDB.Disconnect();
		}

		public static IOCP.ExDBIOCP ExDB;

		public static IOCP.ChatServerIOCP ChatServer;

		public class ExDBIOCP : IOCPManager
		{
			public bool Connect(bool isReconnect = false)
			{
				Tuple<bool, int> tuple = base.Connect(new IPEndPoint(IPAddress.Parse(Configs.ExDB_IP), Configs.ExDB_Port));
				if (!tuple.Item1)
				{
					return isReconnect && this.reconnect();
				}
				this._index = new int?(tuple.Item2);
				return true;
			}

			public void Disconnect()
			{
				int? index = this._index;
				if (index == null)
				{
					return;
				}
				this._isReconnect = false;
				this.Disconnect(this._index.Value);
			}

			public void Send(byte[] data)
			{
				int? index = this._index;
				if (index == null)
				{
					return;
				}
				base.Send(this._index.Value, data);
			}

			public override void ProcessMessage(string info, bool isError = false)
			{
				Form_Main.Log.Write("[ExDB] " + info, isError ? Logger.LogType.ERROR : Logger.LogType.Normal);
			}

			public override void ProcessReceive(int index, byte[] buffer, int len)
			{
				if (this._packetManager == null)
				{
					this._packetManager = new PacketManager();
				}
				foreach (byte[] buffer2 in this._packetManager.Process(buffer, 0, len))
				{
					ExDBProtocol.Core(buffer2);
				}
			}

			public override void Disconnect(int index)
			{
				using (this._packetManager)
				{
				}
				this._packetManager = null;
				base.Disconnect(index);
				this.reconnect();
			}

			private bool reconnect()
			{
				if (!this._isReconnect)
				{
					return true;
				}
				Form_Main.Log.Write(string.Format("[ExDB] [IOCP] Reconnecting in 10 seconds", Configs.ExDB_IP, Configs.ExDB_Port), Logger.LogType.Normal);
				Thread.Sleep(TimeSpan.FromSeconds(10.0));
				return this.Connect(false) || this.reconnect();
			}

			private int? _index;

			private PacketManager _packetManager;

			private bool _isReconnect = true;
		}

		public class ChatServerIOCP : IOCPManager
		{
			public ChatServerIOCP() : base(Configs.Port)
			{
				this._packets = new ConcurrentDictionary<int, PacketManager>();
				this._connectionsIndexes = new ConcurrentDictionary<string, List<int>>();
			}

			internal override void OnConnectionAcceptBlocking(IOCPConnection connection, string error)
			{
				if (error != null)
				{
					base.OnConnectionAcceptBlocking(connection, error);
					return;
				}
				if (Database.IPBanned(connection.IP))
				{
					Form_Main.Log.Write("[ChatServer] [IOCP] [Connection Refused] IP " + connection.IP + " is banned", Logger.LogType.Normal);
					using (connection.TcpClient)
					{
						try
						{
							connection.TcpClient.Close();
						}
						catch
						{
						}
					}
					return;
				}
				this._connectionsIndexes.GetOrAdd(connection.IP, new List<int>()).Add(connection.Index);
				base.OnConnectionAcceptBlocking(connection, null);
			}

			public override void ProcessMessage(string info, bool isError = false)
			{
				Form_Main.Log.Write("[ChatServer] " + info, isError ? Logger.LogType.ERROR : Logger.LogType.Normal);
			}

			public override void ProcessReceive(int index, byte[] buffer, int len)
			{
				IEnumerable<byte[]> pkgBytes = this._packets.GetOrAdd(index, new PacketManager()).Process(buffer, 0, len);
				foreach(byte[] array in pkgBytes)
                {
					for(int i=0;i<pkgBytes.Count();i++)
                    {
						byte[] tempArray = pkgBytes.ElementAt(i);
						Crypt.DecXor32(ref tempArray, ((array[0] != 193) ? 1 : 0) + 2, array.Length - 1, -1);
					}
                }
			}

			public override void Disconnect(int index)
			{
				IOCPConnection iocpconnection;
				List<int> list;
				if (this._connections.TryGetValue(index, out iocpconnection) && this._connectionsIndexes.TryGetValue(iocpconnection.IP, out list) && list.Contains(index))
				{
					list.Remove(index);
				}
				base.Disconnect(index);
				PacketManager packetManager;
				if (this._packets.TryRemove(index, out packetManager))
				{
					using (packetManager)
					{
					}
				}
				ChatManager.HandleDisconnect(index);
			}

			public List<int> FindAll(string ip)
			{
				List<int> source;
				if (!this._connectionsIndexes.TryGetValue(ip, out source))
				{
					return null;
				}
				return source.ToList<int>();
			}

			private ConcurrentDictionary<int, PacketManager> _packets;

			private ConcurrentDictionary<string, List<int>> _connectionsIndexes;
		}
	}
}
