using System;

namespace ChatServer.Models
{
	public class ChatMember
	{
		public int RoomID;

		public bool IsOwner;

		public byte DisplayIndex;

		public int SocketNumber;

		public string Name;

		public string IP;
	}
}
