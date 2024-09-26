using System;
using System.Linq;
using System.Runtime.InteropServices;
using ChatServer.Models;

namespace ChatServer
{
	public static class ChatServerProtocol
	{
		public static void Core(int index, byte[] buffer)
		{
			switch ((buffer[0] == 193 || buffer[0] == 195) ? buffer[2] : buffer[3])
			{
			case 0:
				ChatServerProtocol.ClientLogin(index, buffer.Cast<Packets.PMSG_CHAT_CLIENTLOGIN>());
				return;
			case 1:
				ChatServerProtocol.ClientDisconnect(index);
				return;
			case 4:
				ChatServerProtocol.RoomChatMessage(index, buffer.Cast<Packets.PMSG_CHAT_MESSAGE>(), buffer);
				return;
			case 5:
				return;
			}
			Form_Main.Log.Write(string.Format("[ChatServer] [{0}] [Unknown Packet] {1}", index, Packets.ToString(buffer)), Logger.LogType.WARNING);
		}

		private static void ClientDisconnect(int index)
		{
			ChatRoom playerRoom = ChatManager.GetPlayerRoom(index);
			if (playerRoom == null)
			{
				return;
			}
			ChatMember chatMember = playerRoom.GetMember(index);
			if (chatMember == null)
			{
				return;
			}
			playerRoom.RemoveMember(chatMember.SocketNumber);
			IOCP.ChatServer.Disconnect(index);
			ChatServerProtocol.RoomChatMessage((int)playerRoom.Number, string.Concat(new string[]
			{
				"[ -- ",
				chatMember.Name,
				" ",
				Language.HasLeftTheConversation,
				" -- ]"
			}));
			if (playerRoom.MemberCount == 1)
			{
				chatMember = playerRoom.GetMembers()[0];
				ChatServerProtocol.ClientDisconnect(chatMember.SocketNumber);
			}
		}

		public static void RoomChatMessage(int RoomNumber, string text)
		{
			ChatRoom room = ChatManager.GetRoom(RoomNumber);
			if (room == null)
			{
				return;
			}
			byte[] bytes = Configs.LanguageEncoding.GetBytes(text);
			string @string = Configs.LanguageEncoding.GetString(bytes);
			Crypt.BuxConvert(ref bytes, bytes.Length);
			byte[] outData = Packets.GET_CHAT_MESSAGE_PACKET(bytes, byte.MaxValue);
			room.LogMessage(255, @string);
			room.GetMembers().ForEach(delegate(ChatMember member)
			{
				IOCP.ChatServer.Send(member.SocketNumber, outData);
			});
		}

		public static void RoomChatMessage(int index, Packets.PMSG_CHAT_MESSAGE data, byte[] buffer)
		{
			ChatRoom playerRoom = ChatManager.GetPlayerRoom(index);
			if (playerRoom == null)
			{
				Form_Main.Log.Write(string.Format("[ChatServer] [{0}] [RoomChatMessage] [Failed] Not member of any conversation", index), Logger.LogType.WARNING);
				return;
			}
			ChatMember senderInfo = playerRoom.GetMember(index);
			if (senderInfo == null)
			{
				Form_Main.Log.Write(string.Format("[ChatServer] [{0}] [RoomChatMessage] [Failed] Not in room {1} anymore", index, playerRoom.Number), Logger.LogType.WARNING);
				return;
			}
			byte[] Message = new byte[(int)data.MessageLength];
			Array.Copy(buffer, Marshal.SizeOf<Packets.PMSG_CHAT_MESSAGE>(data), Message, 0, (int)data.MessageLength);
			string @string = Configs.LanguageEncoding.GetString(Crypt.BuxConvert(Message, (int)data.MessageLength));
			string text;
			if (@string == null)
			{
				text = null;
			}
			else
			{
				text = @string.Split(new char[]
				{
					' '
				}).FirstOrDefault((string word) => ProhibitedWords.Exists(word));
			}
			string text2 = text;
			if (text2 != null)
			{
				Form_Main.Log.Write(string.Format("[ChatServer] [{0}] [RoomChatMessage] [Room ID {1}] [Refused] Content by '{2}' filtered: '{3}'", new object[]
				{
					index,
					playerRoom.Number,
					senderInfo.Name,
					text2
				}), Logger.LogType.Normal);
				ChatServerProtocol.UserChatMessage(index, "[ -- " + Language.ContentFiltered + " -- ]");
				return;
			}
			playerRoom.LogMessage((int)senderInfo.DisplayIndex, @string);
			playerRoom.GetMembers().ForEach(delegate(ChatMember member)
			{
				IOCP.ChatServer.Send(member.SocketNumber, Packets.GET_CHAT_MESSAGE_PACKET(Message, senderInfo.DisplayIndex));
			});
		}

		private static void ClientLogin(int index, Packets.PMSG_CHAT_CLIENTLOGIN data)
		{
			/*
An exception occurred when decompiling this method (0600011B)

ICSharpCode.Decompiler.DecompilerException: Error decompiling System.Void ChatServer.ChatServerProtocol::ClientLogin(System.Int32,ChatServer.Packets/PMSG_CHAT_CLIENTLOGIN)
 ---> System.NullReferenceException: 未将对象引用设置到对象的实例。
   在 ICSharpCode.Decompiler.ILAst.ILAstOptimizer.IntroducePropertyAccessInstructions(ILExpression expr, ILExpression parentExpr, Int32 posInParent) 位置 C:\projects\dnspy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\ILAstOptimizer.cs:行号 1589
   在 ICSharpCode.Decompiler.ILAst.ILAstOptimizer.IntroducePropertyAccessInstructions(ILNode node) 位置 C:\projects\dnspy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\ILAstOptimizer.cs:行号 1579
   在 ICSharpCode.Decompiler.ILAst.ILAstOptimizer.IntroducePropertyAccessInstructions(ILNode node) 位置 C:\projects\dnspy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\ILAstOptimizer.cs:行号 1576
   在 ICSharpCode.Decompiler.ILAst.ILAstOptimizer.IntroducePropertyAccessInstructions(ILNode node) 位置 C:\projects\dnspy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\ILAstOptimizer.cs:行号 1576
   在 ICSharpCode.Decompiler.ILAst.ILAstOptimizer.Optimize(DecompilerContext context, ILBlock method, AutoPropertyProvider autoPropertyProvider, StateMachineKind& stateMachineKind, MethodDef& inlinedMethod, AsyncMethodDebugInfo& asyncInfo, ILAstOptimizationStep abortBeforeStep) 位置 C:\projects\dnspy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\ILAst\ILAstOptimizer.cs:行号 244
   在 ICSharpCode.Decompiler.Ast.AstMethodBodyBuilder.CreateMethodBody(IEnumerable`1 parameters, MethodDebugInfoBuilder& builder) 位置 C:\projects\dnspy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstMethodBodyBuilder.cs:行号 123
   在 ICSharpCode.Decompiler.Ast.AstMethodBodyBuilder.CreateMethodBody(MethodDef methodDef, DecompilerContext context, AutoPropertyProvider autoPropertyProvider, IEnumerable`1 parameters, Boolean valueParameterIsKeyword, StringBuilder sb, MethodDebugInfoBuilder& stmtsBuilder) 位置 C:\projects\dnspy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstMethodBodyBuilder.cs:行号 88
   --- 内部异常堆栈跟踪的结尾 ---
   在 ICSharpCode.Decompiler.Ast.AstMethodBodyBuilder.CreateMethodBody(MethodDef methodDef, DecompilerContext context, AutoPropertyProvider autoPropertyProvider, IEnumerable`1 parameters, Boolean valueParameterIsKeyword, StringBuilder sb, MethodDebugInfoBuilder& stmtsBuilder) 位置 C:\projects\dnspy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstMethodBodyBuilder.cs:行号 92
   在 ICSharpCode.Decompiler.Ast.AstBuilder.AddMethodBody(EntityDeclaration methodNode, EntityDeclaration& updatedNode, MethodDef method, IEnumerable`1 parameters, Boolean valueParameterIsKeyword, MethodKind methodKind) 位置 C:\projects\dnspy\Extensions\ILSpy.Decompiler\ICSharpCode.Decompiler\ICSharpCode.Decompiler\Ast\AstBuilder.cs:行号 1545
*/;
		}

		public static void UserChatMessage(int index, string msg)
		{
			byte[] bytes = Configs.LanguageEncoding.GetBytes(msg);
			Crypt.BuxConvert(ref bytes, bytes.Length);
			byte[] data = Packets.GET_CHAT_MESSAGE_PACKET(bytes, byte.MaxValue);
			IOCP.ChatServer.Send(index, data);
		}
	}
}
