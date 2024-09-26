using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ToolsAuthServer.Models;

namespace Authenticator.Network
{
	internal static class Protocol
	{
		internal static void Initialize()
		{
			Protocol.ID = ToolIDs.General;
			Protocol.AddVal = string.Empty;
			Protocol.Callback = null;
			Protocol.Callback = delegate(bool b, byte[] d)
			{
			};
			Protocol.Type1 = null;
			Protocol.Type2 = null;
		}

		internal static void Core(byte[] data)
		{
			Packet packet = new Packet(data);
			if (!packet.IsValid)
			{
				Client.OnDisconnect(packet.IsCryptFail ? ErrorCodes.PUBLIC_CryptFail : ErrorCodes.PRIVATE_InvalidPacket_Client);
				return;
			}
			byte code = packet.Code;
			if (code != 1)
			{
				if (code != 160)
				{
					if (code == 255)
					{
						Protocol.pErrorCode(packet.Data[0]);
						Client.OnDisconnect(ErrorCodes.PRIVATE_Unknown);
						return;
					}
				}
				else
				{
					Protocol.pSystemInfo(packet.Code, packet.Data);
				}
				return;
			}
			Protocol.pHandshake(packet.Data);
		}

		internal static void SHandshake()
		{
			Protocol._handshakeChallenge = new byte[2];
			Protocol._random.NextBytes(Protocol._handshakeChallenge);
			byte[] array = Packet.Create(1, Protocol._handshakeChallenge);
			if (array == null)
			{
				Client.OnDisconnect(ErrorCodes.PUBLIC_CryptFail);
				return;
			}
			Client.Send(array);
			Client.RecSync();
		}

		private static void pHandshake(byte[] data)
		{
			if (data.Length != 2)
			{
				Client.OnDisconnect(ErrorCodes.PRIVATE_InvalidHandshake_Client);
				return;
			}
			ushort num = (ushort)((Protocol._handshakeChallenge[0] + Protocol._handshakeChallenge[1]) * 70);
			ushort num2 = BitConverter.ToUInt16(data, 0);
			if (num2 != num)
			{
				Client.OnDisconnect(ErrorCodes.PRIVATE_InvalidHandshake_Client);
				return;
			}
			Protocol.sSystemInfo();
		}

		private static void sSystemInfo()
		{
			List<byte> list = new List<byte>();
			StringBuilder stringBuilder = new StringBuilder(100);
			string s = "XXXX-XXXX-XXXX-XXXX-XXXX-XXXX-XXXX-XXXX";
			if (WinlicenseSDK.WLHardwareGetID(stringBuilder))
			{
				s = stringBuilder.ToString();
			}
			list.AddRange(Encoding.UTF8.GetBytes(s));
			if (Protocol.ID != ToolIDs.Essential)
			{
				list.Add((byte)Protocol.ID);
			}
			if (!string.IsNullOrEmpty(Protocol.AddVal))
			{
				list.AddRange(Encoding.UTF8.GetBytes(Protocol.AddVal));
			}
			byte[] array = Packet.Create(160, list.ToArray());
			if (array == null)
			{
				Client.OnDisconnect(ErrorCodes.PUBLIC_CryptFail);
				return;
			}
			Client.Send(array);
			Client.RecSync();
		}

		private static void pSystemInfo(byte res, byte[] additionalData)
		{
			Client.ApprovedDisconnect = (res == 160);
			Client.Disconnect();
			if (Client.ApprovedDisconnect)
			{
				Protocol.Callback(true, additionalData);
				if (Protocol.Type1 != null && Application.OpenForms.Count == 0)
				{
					try
					{
						Application.Run(Activator.CreateInstance(Protocol.Type1) as Form);
					}
					catch
					{
					}
					Environment.Exit(0);
				}
			}
		}

		private static void pErrorCode(byte errorCode)
		{
			if (Enum.IsDefined(typeof(ErrorCodes), errorCode))
			{
				Protocol.ErrorCode = new ErrorCodes?((ErrorCodes)errorCode);
				return;
			}
			Protocol.ErrorCode = new ErrorCodes?(ErrorCodes.PRIVATE_Unknown);
		}

		internal static ToolIDs ID;

		internal static string AddVal;

		internal static Action<bool, byte[]> Callback;

		internal static Type Type1;

		internal static Type Type2;

		internal static ErrorCodes? ErrorCode = null;

		private static Random _random = new Random();

		private static byte[] _handshakeChallenge;
	}
}
