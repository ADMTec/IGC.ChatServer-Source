using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ChatServer.Models;

namespace ChatServer
{
	public partial class Form_ChatConversation : Form
	{
		public Form_ChatConversation(ChatRoom room)
		{
			this.Room = room;
			this.InitializeComponent();
			base.ActiveControl = this.textBox_Message;
			this.Text = string.Format("Chat Viewer - Room {0}", this.Room.Number);
			this.InitMessageLogs();
			ChatRoom room2 = this.Room;
			room2.NewMessage = (Action<string>)Delegate.Combine(room2.NewMessage, new Action<string>(this.room_OnNewMessage));
		}

		private void Form_ChatConversation_FormClosing(object sender, FormClosingEventArgs e)
		{
			ChatRoom room = this.Room;
			room.NewMessage = (Action<string>)Delegate.Remove(room.NewMessage, new Action<string>(this.room_OnNewMessage));
		}

		private void room_OnNewMessage(string message)
		{
			base.Invoke(new Action(delegate()
			{
				this.richTextBox_MessageLog.AppendText(message + "\n");
			}));
		}

		private void InitMessageLogs()
		{
			this.Room.MessageLog.ToList<string>().ForEach(delegate(string msg)
			{
				this.richTextBox_MessageLog.AppendText(msg + "\n");
			});
		}

		private void Form_ChatConversation_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				base.Close();
			}
		}

		private void textBox_Message_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData != Keys.Return)
			{
				return;
			}
			this.button_SendAdminMessage.PerformClick();
		}

		private void SendAdminMessage()
		{
			ChatServerProtocol.RoomChatMessage((int)this.Room.Number, "[!] " + this.textBox_Message.Text.Trim());
			this.textBox_Message.Clear();
		}

		private void button_SendAdminMessage_Click(object sender, EventArgs e)
		{
			this.SendAdminMessage();
		}

		private ChatRoom Room;
	}
}
