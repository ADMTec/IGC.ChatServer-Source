using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Utilities.Network
{
	public class IOCPListener
	{
		internal IOCPListener()
		{
		}

		internal IOCPListener(int port)
		{
			this._port = port;
			this._listener = new IOCPListener.Listener(IPAddress.Any, port);
		}

		internal bool Start(out string error, int backlog = -1)
		{
			error = null;
			if (this._listener == null)
			{
				return true;
			}
			try
			{
				if (backlog > 0)
				{
					this._listener.Start(backlog);
				}
				else
				{
					this._listener.Start();
				}
			}
			catch (Exception ex)
			{
				error = ex.Message;
				return false;
			}
			return true;
		}

		internal bool Stop(out string error)
		{
			error = null;
			if (this._listener == null)
			{
				return true;
			}
			try
			{
				this._listener.Stop();
			}
			catch (Exception ex)
			{
				error = ex.Message;
				return false;
			}
			return true;
		}

		internal void AcceptAsync(Action<IOCPConnection, string> callback)
		{
			if (this._listener == null || !this._listener.IsListening)
			{
				return;
			}
			ThreadPool.QueueUserWorkItem(delegate(object obj)
			{
				for (;;)
				{
					string error = null;
					IOCPConnection connection = null;
					try
					{
						connection = new IOCPConnection(this._listener.AcceptTcpClient());
					}
					catch (Exception ex)
					{
						if (!this._listener.IsListening)
						{
							break;
						}
						error = ex.Message;
					}
					ThreadPool.QueueUserWorkItem(delegate(object obj1)
					{
						callback(connection, error);
					});
				}
			});
		}

		internal readonly IOCPListener.Listener _listener;

		internal readonly int _port;

		internal class Listener : TcpListener
		{
			internal bool IsListening
			{
				get
				{
					return base.Active;
				}
			}

			internal Listener(IPAddress ipAddr, int port) : base(ipAddr, port)
			{
			}
		}
	}
}
