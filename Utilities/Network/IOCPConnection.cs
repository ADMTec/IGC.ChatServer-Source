using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Utilities.Network
{
	internal class IOCPConnection
	{
		public int Index
		{
			get
			{
				return this.TcpClient.Client.Handle.ToInt32();
			}
		}

		public string IPPort
		{
			get
			{
				EndPoint remoteEndPoint = this.TcpClient.Client.RemoteEndPoint;
				if (remoteEndPoint == null)
				{
					return null;
				}
				return remoteEndPoint.ToString();
			}
		}

		public string IP
		{
			get
			{
				if (this._ip == null)
				{
					EndPoint remoteEndPoint = this.TcpClient.Client.RemoteEndPoint;
					this._ip = ((remoteEndPoint != null) ? remoteEndPoint.ToString().Split(new char[]
					{
						':'
					}).First<string>() : null);
				}
				return this._ip;
			}
		}

		public ushort Port
		{
			get
			{
				EndPoint remoteEndPoint = this.TcpClient.Client.RemoteEndPoint;
				return ushort.Parse(((remoteEndPoint != null) ? remoteEndPoint.ToString().Split(new char[]
				{
					':'
				}).Last<string>() : null) ?? "0");
			}
		}

		public IOCPConnection(TcpClient tcpClient = null)
		{
			this.TcpClient = (tcpClient ?? new TcpClient());
			this.TcpClient.LingerState = new LingerOption(true, 10);
		}

		public void Connect(IPEndPoint addr)
		{
			this.TcpClient.Connect(addr);
		}

		public int Receive(byte[] buffer)
		{
			int num = 0;
			try
			{
				do
				{
					num = this.TcpClient.Client.Receive(buffer);
				}
				while (num < 1 && this.TcpClient.Available > 0);
			}
			catch
			{
			}
			this.LastActive = DateTime.Now;
			return num;
		}

		public DateTime LastActive = DateTime.Now;

		private string _ip;

		public readonly TcpClient TcpClient;
	}
}
