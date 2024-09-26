using System;

namespace ChatServer.Models
{
	public class ChatTicket
	{
		public ChatTicket(string playerName, bool isRoomOwner)
		{
			this.ID = (uint)ChatTicket._idRandomizer.Next(0, int.MaxValue);
			this.PlayerName = playerName;
			this.IsRoomOwner = isRoomOwner;
		}

		private static Random _idRandomizer = new Random();

		public readonly uint ID;

		public readonly string PlayerName;

		public readonly bool IsRoomOwner;
	}
}
