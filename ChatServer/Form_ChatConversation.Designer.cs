namespace ChatServer
{
	public partial class Form_ChatConversation : global::System.Windows.Forms.Form
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::ChatServer.Form_ChatConversation));
			this.richTextBox_MessageLog = new global::System.Windows.Forms.RichTextBox();
			this.panel1 = new global::System.Windows.Forms.Panel();
			this.textBox_Message = new global::System.Windows.Forms.TextBox();
			this.button_SendAdminMessage = new global::System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.richTextBox_MessageLog.BackColor = global::System.Drawing.Color.Black;
			this.richTextBox_MessageLog.BorderStyle = global::System.Windows.Forms.BorderStyle.None;
			this.richTextBox_MessageLog.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.richTextBox_MessageLog.Font = new global::System.Drawing.Font("Tahoma", 9f, global::System.Drawing.FontStyle.Bold, global::System.Drawing.GraphicsUnit.Point, 177);
			this.richTextBox_MessageLog.ForeColor = global::System.Drawing.Color.WhiteSmoke;
			this.richTextBox_MessageLog.Location = new global::System.Drawing.Point(0, 0);
			this.richTextBox_MessageLog.Margin = new global::System.Windows.Forms.Padding(4, 4, 4, 4);
			this.richTextBox_MessageLog.Name = "richTextBox_MessageLog";
			this.richTextBox_MessageLog.ReadOnly = true;
			this.richTextBox_MessageLog.Size = new global::System.Drawing.Size(477, 185);
			this.richTextBox_MessageLog.TabIndex = 0;
			this.richTextBox_MessageLog.Text = "";
			this.richTextBox_MessageLog.WordWrap = false;
			this.panel1.Controls.Add(this.textBox_Message);
			this.panel1.Controls.Add(this.button_SendAdminMessage);
			this.panel1.Dock = global::System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new global::System.Drawing.Point(0, 185);
			this.panel1.Margin = new global::System.Windows.Forms.Padding(2, 2, 2, 2);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new global::System.Windows.Forms.Padding(1);
			this.panel1.Size = new global::System.Drawing.Size(477, 24);
			this.panel1.TabIndex = 2;
			this.textBox_Message.Dock = global::System.Windows.Forms.DockStyle.Fill;
			this.textBox_Message.Location = new global::System.Drawing.Point(1, 1);
			this.textBox_Message.Margin = new global::System.Windows.Forms.Padding(2, 2, 2, 2);
			this.textBox_Message.MaxLength = 142;
			this.textBox_Message.Name = "textBox_Message";
			this.textBox_Message.Size = new global::System.Drawing.Size(427, 23);
			this.textBox_Message.TabIndex = 2;
			this.textBox_Message.KeyUp += new global::System.Windows.Forms.KeyEventHandler(this.textBox_Message_KeyUp);
			this.button_SendAdminMessage.Dock = global::System.Windows.Forms.DockStyle.Right;
			this.button_SendAdminMessage.Location = new global::System.Drawing.Point(428, 1);
			this.button_SendAdminMessage.Margin = new global::System.Windows.Forms.Padding(2, 2, 2, 2);
			this.button_SendAdminMessage.Name = "button_SendAdminMessage";
			this.button_SendAdminMessage.Size = new global::System.Drawing.Size(48, 22);
			this.button_SendAdminMessage.TabIndex = 3;
			this.button_SendAdminMessage.Text = "Send";
			this.button_SendAdminMessage.UseVisualStyleBackColor = true;
			this.button_SendAdminMessage.Click += new global::System.EventHandler(this.button_SendAdminMessage_Click);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(7f, 15f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(477, 209);
			base.Controls.Add(this.richTextBox_MessageLog);
			base.Controls.Add(this.panel1);
			this.Font = new global::System.Drawing.Font("Segoe UI", 9f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 177);
			base.Icon = (global::System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.KeyPreview = true;
			base.Margin = new global::System.Windows.Forms.Padding(4, 4, 4, 4);
			base.Name = "Form_ChatConversation";
			base.FormClosing += new global::System.Windows.Forms.FormClosingEventHandler(this.Form_ChatConversation_FormClosing);
			base.KeyDown += new global::System.Windows.Forms.KeyEventHandler(this.Form_ChatConversation_KeyDown);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			base.ResumeLayout(false);
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.RichTextBox richTextBox_MessageLog;

		private global::System.Windows.Forms.Panel panel1;

		private global::System.Windows.Forms.TextBox textBox_Message;

		private global::System.Windows.Forms.Button button_SendAdminMessage;
	}
}
