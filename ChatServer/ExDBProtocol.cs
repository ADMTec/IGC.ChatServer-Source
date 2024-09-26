using System;
using ChatServer.Models;

namespace ChatServer
{
	internal static class ExDBProtocol
	{
		public static bool ExDBConnectionDone { get; private set; }

		public static void Core(byte[] buffer)
		{
			byte b = (buffer[0] == 193 || buffer[0] == 195) ? buffer[2] : buffer[3];
			if (b == 0)
			{
				ExDBProtocol.excsConnectionResultRecv(buffer.Cast<Packets.SDHP_RESULT>());
				return;
			}
			if (b == 160)
			{
				ExDBProtocol.excsChatRoomCreateReqeustRecv(buffer.Cast<Packets.FCHP_CHATROOM_CREATE_REQ>());
				return;
			}
			if (b != 161)
			{
				Form_Main.Log.Write("[ExDB] [Unknown Packet] " + Packets.ToString(buffer), Logger.LogType.WARNING);
				return;
			}
			ExDBProtocol.excsChatRoomInvitationRequestRecv(buffer.Cast<Packets.FCHP_CHATROOM_INVITATION_REQ>());
		}

		private static void excsConnectionResultRecv(Packets.SDHP_RESULT data)
		{
			if (data.Result == 1)
			{
				ExDBProtocol.ExDBConnectionDone = true;
				return;
			}
			Form_Main.Log.Write("\t-> [Failed] See DataServer logs", Logger.LogType.ERROR);
		}

		private static void excsChatRoomInvitationRequestRecv(Packets.FCHP_CHATROOM_INVITATION_REQ data)
		{
			Packets.FCHP_CHATROOM_CREATE_RESULT fchp_CHATROOM_CREATE_RESULT = new Packets.FCHP_CHATROOM_CREATE_RESULT
			{
				RoomNumber = data.RoomNumber,
				ServerId = data.ServerId,
				Name = data.Name,
				Number = data.Number,
				Type = 2
			};
			ChatRoom room = ChatManager.GetRoom((int)data.RoomNumber);
			if (room == null)
			{
				Form_Main.Log.Write(string.Format("[ExDB] [ChatRoomInvitation] [Room {0}] [Failed] Room not found", data.RoomNumber), Logger.LogType.WARNING);
				fchp_CHATROOM_CREATE_RESULT.Result = 0;
				IOCP.ExDB.Send(fchp_CHATROOM_CREATE_RESULT.Cast<Packets.FCHP_CHATROOM_CREATE_RESULT>());
				return;
			}
			uint num = room.GenerateTicket(data.Name.ToStringEx(true), false);
			fchp_CHATROOM_CREATE_RESULT.TicketNumber = num;
			fchp_CHATROOM_CREATE_RESULT.Result = 1;
			IOCP.ExDB.Send(fchp_CHATROOM_CREATE_RESULT.Cast<Packets.FCHP_CHATROOM_CREATE_RESULT>());
			Form_Main.Log.Write(string.Format("[ExDB] [ChatRoomInvitation] [Room {0}] Generated Ticket: '{1}'; {2}", room.Number, data.Name.ToStringEx(true), num), Logger.LogType.Normal);
		}

		private static void excsChatRoomCreateReqeustRecv(Packets.FCHP_CHATROOM_CREATE_REQ data)
		{
			ChatRoom chatRoom = ChatManager.CreateRoom(data.Name.ToStringEx(true));
			uint num = chatRoom.GenerateTicket(data.Name.ToStringEx(true), true);
			Packets.FCHP_CHATROOM_CREATE_RESULT fchp_CHATROOM_CREATE_RESULT = new Packets.FCHP_CHATROOM_CREATE_RESULT
			{
				Type = 0,
				RoomNumber = chatRoom.Number,
				Result = 1,
				FriendName = data.FriendName,
				Name = data.Name,
				Number = data.Number,
				ServerId = data.ServerId,
				TicketNumber = num
			};
			IOCP.ExDB.Send(fchp_CHATROOM_CREATE_RESULT.Cast<Packets.FCHP_CHATROOM_CREATE_RESULT>());
			Form_Main.Log.Write(string.Format("[ExDB] [ChatRoomCreateReqeust] Created Room {0}", chatRoom.Number), Logger.LogType.Normal);
			uint num2 = chatRoom.GenerateTicket(data.FriendName.ToStringEx(true), false);
			fchp_CHATROOM_CREATE_RESULT.Type = 1;
			fchp_CHATROOM_CREATE_RESULT.Number = data.FriendNumber;
			fchp_CHATROOM_CREATE_RESULT.FriendName = data.Name;
			fchp_CHATROOM_CREATE_RESULT.Name = data.FriendName;
			fchp_CHATROOM_CREATE_RESULT.TicketNumber = num2;
			fchp_CHATROOM_CREATE_RESULT.ServerId = data.FriendServerId;
			IOCP.ExDB.Send(fchp_CHATROOM_CREATE_RESULT.Cast<Packets.FCHP_CHATROOM_CREATE_RESULT>());
			Form_Main.Log.Write(string.Format("[ExDB] [ChatRoomCreateReqeust] [Room {0}] Generated Tickets: '{1}'(Creator); {2} | '{3}'; {4}", new object[]
			{
				chatRoom.Number,
				data.Name.ToStringEx(true),
				num,
				data.FriendName.ToStringEx(true),
				num2
			}), Logger.LogType.Normal);
		}

		public static void CSEXIdentificationSend()
		{
			Packets.SDHP_SERVERINFO type = new Packets.SDHP_SERVERINFO
			{
				Type = 2,
				Port = (ushort)Configs.Port,
				ServerName = "IGC.ChatServer".ToFixedArray(50, true)
			};
			IOCP.ExDB.Send(type.Cast<Packets.SDHP_SERVERINFO>());
		}
	}
}
