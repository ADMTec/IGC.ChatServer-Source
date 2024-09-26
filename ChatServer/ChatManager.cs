using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using ChatServer.Models;

namespace ChatServer
{
	internal static class ChatManager
	{
		public static void Initialize()
		{
			ChatRoom.MemberJoined = (Action<ChatMember>)Delegate.Combine(ChatRoom.MemberJoined, new Action<ChatMember>(ChatManager.onChatRoomMemberJoined));
			ChatRoom.MemberLeft = (Action<ChatMember>)Delegate.Combine(ChatRoom.MemberLeft, new Action<ChatMember>(ChatManager.onChatRoomMemberLeft));
			ChatManager._roomCleaner.AutoReset = true;
			ChatManager._roomCleaner.Interval = TimeSpan.FromSeconds(10.0).TotalMilliseconds;
			ChatManager._roomCleaner.Elapsed += ChatManager.clearEmptyRooms;
			ChatManager._roomCleaner.Start();
		}

		private static void clearEmptyRooms(object sender, ElapsedEventArgs e)
		{
			ChatManager._rooms.Values.ToList<ChatRoom>().FindAll((ChatRoom room) => room.MemberCount < 2 && room.CreationTime.AddSeconds(30.0) < DateTime.Now).ForEach(delegate(ChatRoom room)
			{
				room.Close();
			});
		}

		private static void onChatRoomMemberLeft(ChatMember mem)
		{
			int num;
			ChatManager._chatPlayers.TryRemove(mem.SocketNumber, out num);
		}

		private static void onChatRoomMemberJoined(ChatMember mem)
		{
			ChatManager._chatPlayers.TryAdd(mem.SocketNumber, mem.RoomID);
		}

		public static ChatRoom CreateRoom(string creator)
		{
			ChatRoom chatRoom = new ChatRoom
			{
				Number = (short)ChatManager._roomIndexPool.Dequeue(),
				InitialCreator = creator
			};
			ChatManager._rooms.TryAdd((int)chatRoom.Number, chatRoom);
			ChatManager.ChatRoomCreated((int)chatRoom.Number);
			return chatRoom;
		}

		public static ChatRoom GetRoom(int number)
		{
			ChatRoom result;
			if (!ChatManager._rooms.TryGetValue(number, out result))
			{
				return null;
			}
			return result;
		}

		public static ChatRoom GetPlayerRoom(int index)
		{
			int key;
			if (!ChatManager._chatPlayers.TryGetValue(index, out key))
			{
				return null;
			}
			ChatRoom result;
			if (!ChatManager._rooms.TryGetValue(key, out result))
			{
				return null;
			}
			return result;
		}

		public static void RemoveRoom(short number)
		{
			ChatRoom chatRoom;
			if (!ChatManager._rooms.TryRemove((int)number, out chatRoom))
			{
				return;
			}
			ChatManager._roomIndexPool.Enqueue((int)chatRoom.Number);
			ChatManager.ChatRoomDestroyed((int)number);
		}

		public static bool HandleDisconnect(int index)
		{
			ChatRoom playerRoom = ChatManager.GetPlayerRoom(index);
			return playerRoom != null && playerRoom.Kick(index);
		}

		public static Action<int> ChatRoomCreated = delegate(int i)
		{
		};

		public static Action<int> ChatRoomDestroyed = delegate(int i)
		{
		};

		private static Timer _roomCleaner = new Timer();

		private static PoolManager _roomIndexPool = new PoolManager(1000);

		private static ConcurrentDictionary<int, ChatRoom> _rooms = new ConcurrentDictionary<int, ChatRoom>();

		private static ConcurrentDictionary<int, int> _chatPlayers = new ConcurrentDictionary<int, int>();
	}
}
