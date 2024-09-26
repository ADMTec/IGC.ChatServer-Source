using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;
using Authenticator.Models;
using ToolsAuthServer.Models;

namespace Authenticator.Network
{
	internal static class Client
	{
		internal static void Initialize()
		{
			Client._disconnected = false;
			Client._socket = null;
			Client._recBuffer = null;
			Client._authServers = null;
			Client.ApprovedDisconnect = false;
		}

		internal static void Connect(int port, bool authServerChange, int reconnectCount = 1)
		{
			if (Client._authServers == null)
			{
				Client._authServers = new Queue<Tuple<string, int>>(new Tuple<string, int>[]
				{
					new Tuple<string, int>("auth1.igcn.mu", port),
					new Tuple<string, int>("auth2.igcn.mu", port)
				});
				if (authServerChange)
				{
					Client._authServers = new Queue<Tuple<string, int>>(Client._authServers.Reverse<Tuple<string, int>>());
				}
			}
			Tuple<string, int> tuple = Client._authServers.Dequeue();
			Client._authServers.Enqueue(tuple);
			try
			{
				Client._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				Client._socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Debug, true);
				Client.ConnectTimeout(Client._socket, tuple, 15000);
				if (!Client._socket.Connected)
				{
					throw new Exception();
				}
			}
			catch
			{
				if (reconnectCount == 4)
				{
					Client.OnDisconnect(ErrorCodes.PUBLIC_ServerUnreachable);
				}
				else
				{
					Client.Connect(port, false, ++reconnectCount);
				}
				return;
			}
			Protocol.SHandshake();
		}

		public static void RecSync()
		{
			for (;;)
			{
				int num = 0;
				try
				{
					num = Client._socket.Receive(Client._recBuffer = new byte[1024]);
				}
				catch
				{
					Client.OnDisconnect(ErrorCodes.PUBLIC_ServcerConnectionLost);
					return;
				}
				if (num == 0)
				{
					break;
				}
				if (Client._recBuffer.Length != num)
				{
					Array.Resize<byte>(ref Client._recBuffer, num);
				}
				Queue<byte[]> queue = new Queue<byte[]>();
				bool flag;
				PacketManager.Result result = PacketManager.Add(Client._recBuffer, out flag, ref queue);
				if (result == PacketManager.Result.MaxLenLimit)
				{
					goto Block_3;
				}
				while (queue.Count > 0)
				{
					Protocol.Core(queue.Dequeue());
				}
				if (!flag)
				{
					return;
				}
			}
			Client.OnDisconnect(ErrorCodes.PUBLIC_ServcerConnectionLost);
			return;
			Block_3:
			Client.OnDisconnect(ErrorCodes.PRIVATE_PacketMaxLenReached_Client);
		}

		private static void ConnectTimeout(Socket socket, Tuple<string, int> endpoint, int timeout = 15000)
		{
			try
			{
				IAsyncResult asyncResult = socket.BeginConnect(endpoint.Item1, endpoint.Item2, null, null);
				bool flag = asyncResult.AsyncWaitHandle.WaitOne(timeout, true);
				if (flag)
				{
					try
					{
						socket.EndConnect(asyncResult);
						goto IL_51;
					}
					catch
					{
						socket.Close();
						goto IL_51;
					}
				}
				socket.Close();
				IL_51:;
			}
			catch
			{
				try
				{
					socket.Close();
				}
				catch
				{
				}
			}
		}

		internal static void Send(byte[] data)
		{
			try
			{
				Client._socket.Send(data);
			}
			catch
			{
				Client.OnDisconnect(ErrorCodes.PUBLIC_ServcerConnectionLost);
			}
		}

		internal static void Disconnect()
		{
			if (Client._disconnected)
			{
				return;
			}
			Client._disconnected = true;
			Client._recBuffer = null;
			try
			{
				Client._socket.Shutdown(SocketShutdown.Both);
				Client._socket.Disconnect(false);
				Client._socket.Close();
				Client._socket.Dispose();
			}
			catch
			{
			}
		}

		internal static void OnDisconnect(ErrorCodes errorCode)
		{
			Client.Disconnect();
			if (!Client.ApprovedDisconnect)
			{
				Protocol.Callback(false, null);
				if (Application.OpenForms.Count == 0)
				{
					try
					{
						Form form = Activator.CreateInstance(Protocol.Type2) as Form;
						(form as INoLicenseForm).AuthenticationError = new ErrorCodes?(Protocol.ErrorCode ?? errorCode);
						Application.Run(form);
					}
					catch
					{
					}
					Environment.Exit(0);
				}
			}
		}

		private const int BUFFER_SIZE = 1024;

		private static bool _disconnected;

		private static Socket _socket;

		private static byte[] _recBuffer;

		private static Queue<Tuple<string, int>> _authServers;

		internal static bool ApprovedDisconnect;
	}
}
