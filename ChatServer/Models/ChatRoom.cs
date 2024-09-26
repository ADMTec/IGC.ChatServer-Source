using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ChatServer.Models
{
	public class ChatRoom
	{
		public int MemberCount
		{
			get
			{
				return this.Members.Count;
			}
		}

		public ChatRoom()
		{
			this.Members = new ConcurrentDictionary<int, ChatMember>();
			this.Tickets = new ConcurrentDictionary<uint, ChatTicket>();
			this.MessageLog = new List<string>();
		}

		public uint GenerateTicket(string name, bool isOwner = false)
		{
			ChatTicket chatTicket = new ChatTicket(name, isOwner);
			this.Tickets.TryAdd(chatTicket.ID, chatTicket);
			return chatTicket.ID;
		}

		//[return: TupleElementNames(new string[]
		//{
		//	"info",
		//	"totalCount"
		//})]
		public ValueTuple<ChatMember, int> GenerateMember(string name, int socketNumber, bool isOwner)
		{
			ChatMember chatMember = new ChatMember
			{
				Name = name,
				SocketNumber = socketNumber,
				IsOwner = isOwner,
				IP = IOCP.ChatServer.GetIP(socketNumber),
				RoomID = (int)this.Number
			};
			chatMember.DisplayIndex = (byte)ChatRoom._memberChatIndexPool.Dequeue();
			if (!this.Members.TryAdd(socketNumber, chatMember))
			{
				return new ValueTuple<ChatMember, int>(null, 0);
			}
			int memberCount = this.MemberCount;
			ChatRoom.MemberJoined(chatMember);
			return new ValueTuple<ChatMember, int>(chatMember, memberCount);
		}

		public ChatMember GetMember(int index)
		{
			ChatMember result;
			if (!this.Members.TryGetValue(index, out result))
			{
				return null;
			}
			return result;
		}

		public ChatMember GetMember(string name)
		{
			return this.Members.Values.ToList<ChatMember>().FirstOrDefault((ChatMember m) => m.Name == name);
		}

		public ChatMember GetOwner()
		{
			return this.Members.Values.ToList<ChatMember>().FirstOrDefault((ChatMember m) => m.IsOwner);
		}

		public ChatTicket UseTicket(uint id)
		{
			ChatTicket result;
			if (!this.Tickets.TryRemove(id, out result))
			{
				return null;
			}
			return result;
		}

		public void LogMessage(int SenderIndex, string Message)
		{
			string text = (SenderIndex == 255) ? string.Empty : this.Members.Values.ToList<ChatMember>().FirstOrDefault((ChatMember m) => (int)m.DisplayIndex == SenderIndex).Name;
			if (Configs.LogMessagesToDatabase && (SenderIndex != 255 || Message.StartsWith("[!] ")))
			{
				string text2 = string.Join(";", from m in this.Members.Values.ToList<ChatMember>().FindAll((ChatMember m) => (int)m.DisplayIndex != SenderIndex)
				select m.Name);
				SQL.EnqueueQuery("INSERT INTO IGC_FriendChat_MessageLog(Name, FriendName, [Text], [Date]) VALUES (?, ?, ?, GETUTCDATE())", new object[]
				{
					(SenderIndex == 255) ? "[!]" : text,
					text2,
					(SenderIndex != 255) ? Message : Message.Substring(Message.IndexOf(':') + 2)
				});
			}
			if (text != string.Empty)
			{
				text += ": ";
			}
			string text3 = string.Concat(new string[]
			{
				"[",
				DateTime.Now.ToString(),
				"] ",
				text,
				Message
			});
			this.MessageLog.Add(text3);
			this.NewMessage(text3);
		}

		public List<ChatMember> GetMembers()
		{
			return this.Members.Values.ToList<ChatMember>();
		}

		public List<ChatMember> GetMembersExcept(int index)
		{
			return this.Members.Values.ToList<ChatMember>().FindAll((ChatMember m) => m.SocketNumber != index);
		}

		public void RemoveMember(int socketNumber)
		{
			ChatMember chatMember;
			if (!this.Members.TryRemove(socketNumber, out chatMember))
			{
				return;
			}
			ChatRoom._memberChatIndexPool.Enqueue((int)chatMember.DisplayIndex);
			ChatRoom.MemberLeft(chatMember);
			if (this.MemberCount == 0)
			{
				ChatManager.RemoveRoom(this.Number);
			}
		}

		public void Close()
		{
			List<ChatMember> list = this.Members.Values.ToList<ChatMember>();
			list.ForEach(delegate(ChatMember mem)
			{
				this.RemoveMember(mem.SocketNumber);
			});
			list.ForEach(delegate(ChatMember mem)
			{
				IOCP.ChatServer.Disconnect(mem.SocketNumber);
			});
			ChatManager.RemoveRoom(this.Number);
		}

		public bool Kick(int number)
		{
			if (this.MemberCount == 2)
			{
				this.Close();
				return true;
			}
			ChatMember member = this.GetMember(number);
			if (member == null)
			{
				return false;
			}
			this.RemoveMember(number);
			IOCP.ChatServer.Disconnect(member.SocketNumber);
			return true;
		}

		public static Action<ChatMember> MemberJoined = delegate(ChatMember mem)
		{
		};

		public static Action<ChatMember> MemberLeft = delegate(ChatMember mem)
		{
		};

		private static PoolManager _memberChatIndexPool = new PoolManager(15);

		public readonly DateTime CreationTime = DateTime.Now;

		public short Number;

		public ConcurrentDictionary<int, ChatMember> Members;

		public ConcurrentDictionary<uint, ChatTicket> Tickets;

		public List<string> MessageLog;

		public Action<string> NewMessage = delegate(string str)
		{
		};

		public string InitialCreator;
	}
}
