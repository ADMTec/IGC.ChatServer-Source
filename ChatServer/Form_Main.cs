using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using ChatServer.Models;

namespace ChatServer
{
	public partial class Form_Main : Form
	{
		public Form_Main()
		{
			this.InitializeComponent();
			Configs.Read();
			ProhibitedWords.Read();
			Language.Read();
			SQL.Init();
			this.Text += string.Format(" v{0} [TCP: {1}] [Database: {2}]", base.ProductVersion, Configs.Port, Configs.DatabaseName);
			ChatManager.Initialize();
			ChatManager.ChatRoomCreated = (Action<int>)Delegate.Combine(ChatManager.ChatRoomCreated, new Action<int>(this.OnChatRoomCreated));
			ChatManager.ChatRoomDestroyed = (Action<int>)Delegate.Combine(ChatManager.ChatRoomDestroyed, new Action<int>(this.OnChatRoomDestroyed));
			ChatRoom.MemberJoined = (Action<ChatMember>)Delegate.Combine(ChatRoom.MemberJoined, new Action<ChatMember>(this.OnChatRoomParticipantJoin));
			ChatRoom.MemberLeft = (Action<ChatMember>)Delegate.Combine(ChatRoom.MemberLeft, new Action<ChatMember>(this.OnChatRoomParticipantLeave));
			Form_Main.Log = new Logger(ref this.richTextBox_Log, 1000, "Data\\Logs\\");
			Form_Main.Log.Write("ChatServer initialization started", Logger.LogType.Normal);
		}

		private void OnChatRoomParticipantLeave(ChatMember mem)
		{
			base.Invoke(new Action(delegate()
			{
				this.ChatTreeViewRoom_RemoveParticipant(mem.RoomID, mem.Name);
			}));
		}

		private void OnChatRoomParticipantJoin(ChatMember mem)
		{
			base.Invoke(new Action(delegate()
			{
				this.ChatTreeViewRoom_AddParticipant(mem.RoomID, mem.Name);
			}));
		}

		private void OnChatRoomDestroyed(int id)
		{
			base.Invoke(new Action(delegate()
			{
				this.ChatTreeView_RemoveRoom(id);
			}));
		}

		private void OnChatRoomCreated(int id)
		{
			base.Invoke(new Action(delegate()
			{
				this.ChatTreeView_AddRoom(id);
			}));
		}

		private void restartToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.isRestartPending = true;
			Application.Restart();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to close the application?", "Application Close", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
				return;
			}
			Form_Main.IsClosing = true;
			IOCP.Stop();
			if (!this.isRestartPending)
			{
				Environment.Exit(0);
			}
		}

		private void Form_Main_Load(object sender, EventArgs e)
		{

		}

		private void ChatTreeView_AddRoom(int RoomID)
		{
			lock (this)
			{
				string text = string.Format("treeViewNode_Room_{0}", RoomID);
				if (!this.treeView_ChatConversations.Nodes.ContainsKey(text))
				{
					this.treeView_ChatConversations.Nodes.Add(new TreeNode(string.Format("Room {0}", RoomID))
					{
						Name = text,
						ContextMenuStrip = this.contextMenuStrip_RoomOptions
					});
					Form_Main.Log.Write(string.Format("[UI] [Room {0} Created]", RoomID), Logger.LogType.Normal);
				}
			}
		}

		private void ChatTreeView_RemoveRoom(int RoomID)
		{
			lock (this)
			{
				string key = string.Format("treeViewNode_Room_{0}", RoomID);
				if (this.treeView_ChatConversations.Nodes.ContainsKey(key))
				{
					this.treeView_ChatConversations.Nodes.RemoveByKey(key);
					Form_Main.Log.Write(string.Format("[UI] [Room {0} Destroyed] No participants in conersation", RoomID), Logger.LogType.Normal);
				}
			}
		}

		private void ChatTreeViewRoom_AddParticipant(int RoomID, string ParticipantName)
		{
			lock (this)
			{
				string key = string.Format("treeViewNode_Room_{0}", RoomID);
				if (this.treeView_ChatConversations.Nodes.ContainsKey(key))
				{
					string text = "treeViewNode_Participant_" + ParticipantName;
					if (!this.treeView_ChatConversations.Nodes[key].Nodes.ContainsKey(text))
					{
						this.treeView_ChatConversations.Nodes[key].Nodes.Add(new TreeNode(ParticipantName)
						{
							Name = text,
							ContextMenuStrip = this.contextMenuStrip_ParticipantOptions
						});
						Form_Main.Log.Write(string.Format("[UI] [Room {0}] '{1}' joined", RoomID, ParticipantName), Logger.LogType.Normal);
					}
				}
			}
		}

		private void ChatTreeViewRoom_RemoveParticipant(int RoomID, string ParticipantName)
		{
			lock (this)
			{
				string key = string.Format("treeViewNode_Room_{0}", RoomID);
				if (this.treeView_ChatConversations.Nodes.ContainsKey(key))
				{
					string key2 = "treeViewNode_Participant_" + ParticipantName;
					if (this.treeView_ChatConversations.Nodes[key].Nodes.ContainsKey(key2))
					{
						this.treeView_ChatConversations.Nodes[key].Nodes.RemoveByKey(key2);
						Form_Main.Log.Write(string.Format("[UI] [Room {0}] '{1}' left", RoomID, ParticipantName), Logger.LogType.Normal);
						if (this.treeView_ChatConversations.Nodes[key].Nodes.Count == 0)
						{
							this.ChatTreeView_RemoveRoom(RoomID);
						}
					}
				}
			}
		}

		private void viewChatToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode nodeAt = this.treeView_ChatConversations.GetNodeAt(this.LastNodeMouseDownPosition);
			if (nodeAt == null || !nodeAt.Name.StartsWith("treeViewNode_Room_"))
			{
				return;
			}
			ChatRoom room = ChatManager.GetRoom(Convert.ToInt32(nodeAt.Name.Replace("treeViewNode_Room_", string.Empty)));
			if (room == null)
			{
				return;
			}
			Form_ChatConversation form_ChatConversation = new Form_ChatConversation(room);
			form_ChatConversation.FormClosed += this.onChatConversationFormClosed;
			form_ChatConversation.Show();
		}

		private void onChatConversationFormClosed(object sender, FormClosedEventArgs e)
		{
			Form form = (Form)sender;
			form.FormClosed -= this.onChatConversationFormClosed;
			using (form)
			{
			}
		}

		private void treeView_ChatConversations_MouseDown(object sender, MouseEventArgs e)
		{
			this.LastNodeMouseDownPosition = new Point(e.X, e.Y);
		}

		private void destroyRoomToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode nodeAt = this.treeView_ChatConversations.GetNodeAt(this.LastNodeMouseDownPosition);
			if (nodeAt == null || !nodeAt.Name.StartsWith("treeViewNode_Room_"))
			{
				return;
			}
			int num = Convert.ToInt32(nodeAt.Name.Replace("treeViewNode_Room_", string.Empty));
			ChatRoom room = ChatManager.GetRoom(num);
			if (room == null)
			{
				return;
			}
			Form_Main.Log.Write(string.Format("[UI] [Room {0}] Closed by Admin", num), Logger.LogType.Normal);
			ChatServerProtocol.RoomChatMessage((int)room.Number, "[ -- " + Language.ConversationEndedByAdmin + " -- ]");
			room.Close();
		}

		private void saveRoomMessageLogToFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode nodeAt = this.treeView_ChatConversations.GetNodeAt(this.LastNodeMouseDownPosition);
			if (nodeAt == null || !nodeAt.Name.StartsWith("treeViewNode_Room_"))
			{
				return;
			}
			int num = Convert.ToInt32(nodeAt.Name.Replace("treeViewNode_Room_", string.Empty));
			ChatRoom room = ChatManager.GetRoom(num);
			if (room == null)
			{
				return;
			}
			string fileName;
			using (SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				FileName = string.Format("{0} Room {1}.log", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), num),
				Filter = "Log Files (*.log)|*.log|Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
			})
			{
				if (saveFileDialog.ShowDialog() != DialogResult.OK)
				{
					return;
				}
				fileName = saveFileDialog.FileName;
			}
			using (StreamWriter sw = new StreamWriter(fileName, false))
			{
				room.MessageLog.ToList<string>().ForEach(delegate(string line)
				{
					sw.WriteLine(line);
				});
			}
		}

		private void kickToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode nodeAt = this.treeView_ChatConversations.GetNodeAt(this.LastNodeMouseDownPosition);
			if (nodeAt == null || !nodeAt.Name.StartsWith("treeViewNode_Participant_"))
			{
				return;
			}
			ChatRoom room = ChatManager.GetRoom(Convert.ToInt32(nodeAt.Parent.Name.Replace("treeViewNode_Room_", string.Empty)));
			if (room == null)
			{
				return;
			}
			string name = nodeAt.Name.Replace("treeViewNode_Participant_", string.Empty);
			ChatMember member = room.GetMember(name);
			if (member == null)
			{
				return;
			}
			Form_Main.Log.Write(string.Format("[UI] [Room {0}] '{1}' Kicked by Admin", room.Number, member.Name), Logger.LogType.Normal);
			ChatServerProtocol.RoomChatMessage((int)room.Number, string.Concat(new string[]
			{
				"[ -- ",
				member.Name,
				" ",
				Language.HasBeenRemovedFromTheConversationByAdmin,
				" -- ]"
			}));
			room.Kick(member.SocketNumber);
		}

		private void banToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode nodeAt = this.treeView_ChatConversations.GetNodeAt(this.LastNodeMouseDownPosition);
			if (nodeAt == null || !nodeAt.Name.StartsWith("treeViewNode_Participant_"))
			{
				return;
			}
			ChatRoom room = ChatManager.GetRoom(Convert.ToInt32(nodeAt.Parent.Name.Replace("treeViewNode_Room_", string.Empty)));
			if (room == null)
			{
				return;
			}
			string name = nodeAt.Name.Replace("treeViewNode_Participant_", string.Empty);
			ChatMember member = room.GetMember(name);
			if (member == null)
			{
				return;
			}
			Form_Main.Log.Write(string.Format("[UI] [Room {0}] '{1}' Banned by Admin", room.Number, member.Name), Logger.LogType.Normal);
			SQL.Execute("INSERT INTO IGC_FriendChat_BannedCharacters VALUES (?)", new object[]
			{
				member.Name
			});
			ChatServerProtocol.RoomChatMessage((int)room.Number, string.Concat(new string[]
			{
				"[ -- ",
				member.Name,
				" ",
				Language.HasBeenBannedFromUsingFriendChat,
				" -- ]"
			}));
			room.Kick(member.SocketNumber);
		}

		private void banIPToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode nodeAt = this.treeView_ChatConversations.GetNodeAt(this.LastNodeMouseDownPosition);
			if (nodeAt == null || !nodeAt.Name.StartsWith("treeViewNode_Participant_"))
			{
				return;
			}
			ChatRoom room = ChatManager.GetRoom(Convert.ToInt32(nodeAt.Parent.Name.Replace("treeViewNode_Room_", string.Empty)));
			if (room == null)
			{
				return;
			}
			string name = nodeAt.Name.Replace("treeViewNode_Participant_", string.Empty);
			ChatMember member = room.GetMember(name);
			if (member == null)
			{
				return;
			}
			Form_Main.Log.Write("[UI] ['" + member.IP + "' Banned by Admin]", Logger.LogType.Normal);
			SQL.Execute("INSERT INTO IGC_FriendChat_BannedIPs VALUES (?)", new object[]
			{
				member.IP
			});
			IOCP.ChatServer.FindAll(member.IP).ForEach(delegate(int socketIndex)
			{
				ChatRoom playerRoom = ChatManager.GetPlayerRoom(socketIndex);
				if (playerRoom == null)
				{
					return;
				}
				ChatMember member2 = playerRoom.GetMember(socketIndex);
				ChatServerProtocol.RoomChatMessage((int)playerRoom.Number, string.Concat(new string[]
				{
					"[ -- ",
					member2.Name,
					" ",
					Language.HasBeenBannedFromUsingFriendChat,
					" -- ]"
				}));
				playerRoom.Kick(socketIndex);
			});
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.button_ToggleChats.Text = (this.splitContainer.Panel2Collapsed ? ">" : "<");
			this.splitContainer.Panel2Collapsed = !this.splitContainer.Panel2Collapsed;
		}

		public static Logger Log;

		private bool isRestartPending;

		private Point LastNodeMouseDownPosition = new Point(0, 0);

		public static bool IsClosing;
	}
}
