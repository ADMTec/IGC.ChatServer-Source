using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Timers;

namespace Utilities.Network
{
	public abstract class IOCPManager : IOCPListener, IDisposable
	{
		public string GetIP(int index)
		{
			IOCPConnection iocpconnection;
			if (!this._connections.TryGetValue(index, out iocpconnection))
			{
				return null;
			}
			return iocpconnection.IP;
		}

		public abstract void ProcessReceive(int index, byte[] buffer, int len);

		public abstract void ProcessMessage(string info, bool isError = false);

		public IOCPManager()
		{
			this._connections = new ConcurrentDictionary<int, IOCPConnection>();
		}

		public IOCPManager(int port) : base(port)
		{
			this._connections = new ConcurrentDictionary<int, IOCPConnection>();
			this._inactiveCleaner.AutoReset = true;
			this._inactiveCleaner.Interval = TimeSpan.FromSeconds(10.0).TotalMilliseconds;
			this._inactiveCleaner.Elapsed += this.clearInactive;
			this._inactiveCleaner.Start();
		}

		private void clearInactive(object sender, ElapsedEventArgs e)
		{
			this._connections.Values.ToList<IOCPConnection>().FindAll((IOCPConnection connection) => connection.LastActive.AddSeconds(30.0) < DateTime.Now).ForEach(delegate(IOCPConnection connection)
			{
				this.ProcessMessage(string.Format("[IOCP] [{0}] Connection Inactive", connection.Index), false);
				this.Disconnect(connection.Index);
			});
		}

		public bool Start(int backlog = -1)
		{
			string arg;
			if (!base.Start(out arg, backlog))
			{
				this.ProcessMessage(string.Format("[IOCP] Start TCP {0} Exception; {1}", this._port, arg), true);
				return false;
			}
			this.ProcessMessage(string.Format("[IOCP] Started TCP {0}", this._port), false);
			base.AcceptAsync(new Action<IOCPConnection, string>(this.OnConnectionAcceptBlocking));
			return true;
		}

		public Tuple<bool, int> Connect(IPEndPoint addr)
		{
			IOCPConnection connection = new IOCPConnection(null);
			try
			{
				connection.Connect(addr);
			}
			catch (Exception ex)
			{
				this.ProcessMessage("[IOCP] Connect " + addr.ToString() + " Exception; " + ex.Message, true);
				return new Tuple<bool, int>(false, -1);
			}
			this.ProcessMessage("[IOCP] Connected " + addr.ToString(), false);
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				this.OnConnectionAcceptBlocking(connection, null);
			});
			return new Tuple<bool, int>(true, connection.Index);
		}

		internal virtual void OnConnectionAcceptBlocking(IOCPConnection connection, string error)
		{
			int index = connection.Index;
			try
			{
				using (connection.TcpClient)
				{
					if (error != null)
					{
						this.ProcessMessage("[IOCP] ConnectionAccept Exception; " + error, true);
					}
					else if (!this._connections.TryAdd(index, connection))
					{
						this.ProcessMessage(string.Format("[IOCP] ConnectionAccept Exception; Socket handle {0} already in use", index), true);
					}
					else
					{
						this.ProcessMessage(string.Format("[IOCP] [{0}] Connected", index), false);
						byte[] buffer = new byte[4132];
						for (;;)
						{
							int num;
							try
							{
								num = connection.Receive(buffer);
								if (num != 0)
								{
									TcpClient tcpClient2 = connection.TcpClient;
									bool? flag;
									if (tcpClient2 == null)
									{
										flag = null;
									}
									else
									{
										Socket client = tcpClient2.Client;
										flag = ((client != null) ? new bool?(client.Connected) : null);
									}
									bool? flag2 = flag;
									if (flag2.GetValueOrDefault())
									{
										goto IL_125;
									}
								}
								break;
							}
							catch (SocketException)
							{
								break;
							}
							catch (Exception ex)
							{
								this.ProcessMessage(string.Format("[IOCP] [{0}] DataRecive Exception; {1}", index, ex.Message), true);
								break;
							}
							IL_125:
							try
							{
								this.ProcessReceive(index, buffer, num);
								continue;
							}
							catch (Exception ex2)
							{
								this.ProcessMessage(string.Format("[IOCP] [{0}] ProcessData Exception; {1}", index, ex2.Message), true);
								continue;
							}
							break;
						}
					}
				}
			}
			catch (Exception ex3)
			{
				this.ProcessMessage(string.Format("[IOCP] [{0}] ConnectionAccept Unhandled Exception; {1}", index, ex3.Message), true);
			}
			this.ProcessMessage(string.Format("[IOCP] [{0}] Disconnected", index), false);
			try
			{
				this.Disconnect(index);
				IOCPConnection iocpconnection;
				this._connections.TryRemove(index, out iocpconnection);
			}
			catch (Exception ex4)
			{
				this.ProcessMessage(string.Format("[IOCP] [{0}] ConnectionAccept(Disconnect) Unhandled Exception; {1}", index, ex4.Message), true);
			}
		}

		public bool Stop()
		{
			if (this._listener == null)
			{
				return true;
			}
			string str;
			if (this._listener.IsListening && !base.Stop(out str))
			{
				this.ProcessMessage("[IOCP] StopListener Exception; " + str, true);
				return false;
			}
			this.ProcessMessage("[IOCP] Stopped", false);
			this.clearConnections();
			return true;
		}

		private void clearConnections()
		{
			while (this._connections.Count > 0)
			{
				this.Disconnect(this._connections.First<KeyValuePair<int, IOCPConnection>>().Key);
			}
		}

		public virtual void Disconnect(int index)
		{
			IOCPConnection iocpconnection;
			if (!this._connections.ContainsKey(index) || !this._connections.TryGetValue(index, out iocpconnection))
			{
				return;
			}
			try
			{
				iocpconnection.TcpClient.Close();
			}
			catch
			{
			}
		}

		public void Send(int index, byte[] data)
		{
			IOCPConnection iocpconnection;
			if (!this._connections.ContainsKey(index) || !this._connections.TryGetValue(index, out iocpconnection))
			{
				return;
			}
			try
			{
				iocpconnection.TcpClient.Client.Send(data);
			}
			catch (Exception ex)
			{
				this.ProcessMessage(string.Format("[IOCP] [{0}] DataSend Exception; {1}", index, ex.Message), true);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					string text;
					base.Stop(out text);
					try
					{
						using (this._inactiveCleaner)
						{
							this._inactiveCleaner.Stop();
							this._inactiveCleaner.Elapsed -= this.clearInactive;
						}
					}
					catch
					{
					}
				}
				this.disposedValue = true;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		internal ConcurrentDictionary<int, IOCPConnection> _connections = new ConcurrentDictionary<int, IOCPConnection>();

		private System.Timers.Timer _inactiveCleaner = new System.Timers.Timer();

		private bool disposedValue;
	}
}
