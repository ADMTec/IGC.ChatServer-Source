using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ChatServer
{
	public class Packets
	{
		public static byte LOBYTE(int w)
		{
			return (byte)(w & 255);
		}

		public static byte HIBYTE(int w)
		{
			return (byte)(w >> 8 & 255);
		}

		public static string ToString(byte[] buffer)
		{
			return BitConverter.ToString(buffer).ToUpper().Replace("-", " ");
		}

		public static byte[] GET_CHAT_MESSAGE_PACKET(byte[] message, byte clientIndex)
		{
			List<byte> list = new List<byte>();
			list.Add(193);
			list.Add(4);
			list.Add(clientIndex);
			list.Add((byte)message.Length);
			list.AddRange(message);
			list.Insert(1, (byte)(list.Count + 1));
			return list.ToArray();
		}

		[StructLayout(LayoutKind.Sequential)]
		public class PWMSG_HEAD
		{
			public PWMSG_HEAD(byte c, byte sizeH, byte sizeL, byte headcode)
			{
				this.c = c;
				this.sizeH = sizeH;
				this.sizeL = sizeL;
				this.headcode = headcode;
			}

			public PWMSG_HEAD(byte c, byte headcode)
			{
				this.c = c;
				this.headcode = headcode;
			}

			public byte c;

			public byte sizeH;

			public byte sizeL;

			public byte headcode;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class PBMSG_HEAD
		{
			public PBMSG_HEAD(byte c, byte size, byte headcode)
			{
				this.c = c;
				this.size = size;
				this.headcode = headcode;
			}

			public PBMSG_HEAD(byte c, byte headcode)
			{
				this.c = c;
				this.headcode = headcode;
			}

			public byte c;

			public byte size;

			public byte headcode;
		}

		public struct SDHP_RESULT
		{
			public Packets.PBMSG_HEAD h;

			public byte Result;

			public ulong ItemCount;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class SDHP_SERVERINFO
		{
			public SDHP_SERVERINFO()
			{
				this.h = new Packets.PBMSG_HEAD(193, (byte)Marshal.SizeOf<Packets.SDHP_SERVERINFO>(this), 0);
			}

			private Packets.PBMSG_HEAD h;

			public byte Type;

			public ushort Port;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
			public char[] ServerName;

			public ushort ServerCode;

			public byte ServerVIP;

			public ushort MaxHWIDUseCount;

			public int ServerType;
		}

		public struct FCHP_CHATROOM_CREATE_REQ
		{
			public Packets.PBMSG_HEAD h;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public byte[] Name;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public byte[] FriendName;

			public byte Type;

			public short Number;

			public short ServerId;

			public short FriendNumber;

			public short FriendServerId;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class FCHP_CHATROOM_CREATE_RESULT
		{
			public FCHP_CHATROOM_CREATE_RESULT()
			{
				this.h = new Packets.PBMSG_HEAD(193, (byte)Marshal.SizeOf<Packets.FCHP_CHATROOM_CREATE_RESULT>(this), 160);
			}

			private Packets.PBMSG_HEAD h;

			public byte Result;

			public short RoomNumber;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public byte[] Name;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public byte[] FriendName;

			public short Number;

			public short ServerId;

			public uint TicketNumber;

			public uint Trash;

			public byte Type;
		}

		public struct FCHP_CHATROOM_INVITATION_REQ
		{
			public Packets.PBMSG_HEAD h;

			public short RoomNumber;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public byte[] Name;

			public short Number;

			public short ServerId;

			public byte Type;
		}

		public struct PMSG_CHAT_CLIENTLOGIN
		{
			public Packets.PBMSG_HEAD h;

			public byte btResult;

			public ushort RoomNumber;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public byte[] szTicketNumber;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class PMSG_CHAT_CLIENTLIST_COUNT
		{
			public PMSG_CHAT_CLIENTLIST_COUNT(ushort count)
			{
				this.h = new Packets.PWMSG_HEAD(194, 2);
				this.wCount = count;
				int w = Marshal.SizeOf<Packets.PMSG_CHAT_CLIENTLIST_COUNT>(this) + Marshal.SizeOf<Packets.CHAT_CLIENTLIST>() * (int)count;
				this.h.sizeH = Packets.HIBYTE(w);
				this.h.sizeL = Packets.LOBYTE(w);
			}

			private Packets.PWMSG_HEAD h;

			public byte JUNK1;

			public byte JUNK2;

			private ushort wCount;
		}

		public struct CHAT_CLIENTLIST
		{
			public byte ClientIndex;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public byte[] szClientName;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class CHAT_ADDPARTICIPANT
		{
			public CHAT_ADDPARTICIPANT()
			{
				this.h = new Packets.PBMSG_HEAD(193, (byte)Marshal.SizeOf<Packets.CHAT_ADDPARTICIPANT>(this), 1);
			}

			private Packets.PBMSG_HEAD h;

			public byte Status;

			public byte ClientIndex;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public byte[] szClientName;
		}

		public struct PMSG_CHAT_MESSAGE
		{
			private Packets.PBMSG_HEAD h;

			public byte ClientIndex;

			public byte MessageLength;
		}
	}
}
