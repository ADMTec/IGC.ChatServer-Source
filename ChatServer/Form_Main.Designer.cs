namespace ChatServer
{
	public partial class Form_Main : global::System.Windows.Forms.Form
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Chat Conversations");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.richTextBox_Log = new System.Windows.Forms.RichTextBox();
            this.button_ToggleChats = new System.Windows.Forms.Button();
            this.treeView_ChatConversations = new System.Windows.Forms.TreeView();
            this.contextMenuStrip_RoomOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewChatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveRoomMessageLogToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.destroyRoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_ParticipantOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.kickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.banToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.banIPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.contextMenuStrip_RoomOptions.SuspendLayout();
            this.contextMenuStrip_ParticipantOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.richTextBox_Log);
            this.splitContainer.Panel1.Controls.Add(this.button_ToggleChats);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.treeView_ChatConversations);
            this.splitContainer.Size = new System.Drawing.Size(628, 286);
            this.splitContainer.SplitterDistance = 483;
            this.splitContainer.TabIndex = 2;
            // 
            // richTextBox_Log
            // 
            this.richTextBox_Log.BackColor = System.Drawing.Color.Black;
            this.richTextBox_Log.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_Log.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_Log.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.richTextBox_Log.Location = new System.Drawing.Point(0, 0);
            this.richTextBox_Log.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox_Log.Name = "richTextBox_Log";
            this.richTextBox_Log.ReadOnly = true;
            this.richTextBox_Log.Size = new System.Drawing.Size(466, 284);
            this.richTextBox_Log.TabIndex = 0;
            this.richTextBox_Log.Text = "";
            this.richTextBox_Log.WordWrap = false;
            // 
            // button_ToggleChats
            // 
            this.button_ToggleChats.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_ToggleChats.Font = new System.Drawing.Font("Segoe UI Light", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.button_ToggleChats.Location = new System.Drawing.Point(466, 0);
            this.button_ToggleChats.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_ToggleChats.Name = "button_ToggleChats";
            this.button_ToggleChats.Size = new System.Drawing.Size(15, 284);
            this.button_ToggleChats.TabIndex = 2;
            this.button_ToggleChats.Text = ">";
            this.button_ToggleChats.UseVisualStyleBackColor = true;
            this.button_ToggleChats.Click += new System.EventHandler(this.button1_Click);
            // 
            // treeView_ChatConversations
            // 
            this.treeView_ChatConversations.BackColor = System.Drawing.SystemColors.Control;
            this.treeView_ChatConversations.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView_ChatConversations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_ChatConversations.Location = new System.Drawing.Point(0, 0);
            this.treeView_ChatConversations.Margin = new System.Windows.Forms.Padding(4);
            this.treeView_ChatConversations.Name = "treeView_ChatConversations";
            treeNode1.Name = "Node_ChatConversations";
            treeNode1.Text = "Chat Conversations";
            this.treeView_ChatConversations.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeView_ChatConversations.Size = new System.Drawing.Size(139, 284);
            this.treeView_ChatConversations.TabIndex = 0;
            this.treeView_ChatConversations.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_ChatConversations_MouseDown);
            // 
            // contextMenuStrip_RoomOptions
            // 
            this.contextMenuStrip_RoomOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewChatToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveRoomMessageLogToFileToolStripMenuItem,
            this.toolStripSeparator3,
            this.destroyRoomToolStripMenuItem});
            this.contextMenuStrip_RoomOptions.Name = "contextMenuStrip";
            this.contextMenuStrip_RoomOptions.Size = new System.Drawing.Size(226, 82);
            // 
            // viewChatToolStripMenuItem
            // 
            this.viewChatToolStripMenuItem.Name = "viewChatToolStripMenuItem";
            this.viewChatToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.viewChatToolStripMenuItem.Text = "Open Chat Viewer";
            this.viewChatToolStripMenuItem.Click += new System.EventHandler(this.viewChatToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(222, 6);
            // 
            // saveRoomMessageLogToFileToolStripMenuItem
            // 
            this.saveRoomMessageLogToFileToolStripMenuItem.Name = "saveRoomMessageLogToFileToolStripMenuItem";
            this.saveRoomMessageLogToFileToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.saveRoomMessageLogToFileToolStripMenuItem.Text = "Save Message Log to File";
            this.saveRoomMessageLogToFileToolStripMenuItem.Click += new System.EventHandler(this.saveRoomMessageLogToFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(222, 6);
            // 
            // destroyRoomToolStripMenuItem
            // 
            this.destroyRoomToolStripMenuItem.Name = "destroyRoomToolStripMenuItem";
            this.destroyRoomToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.destroyRoomToolStripMenuItem.Text = "Destroy";
            this.destroyRoomToolStripMenuItem.Click += new System.EventHandler(this.destroyRoomToolStripMenuItem_Click);
            // 
            // contextMenuStrip_ParticipantOptions
            // 
            this.contextMenuStrip_ParticipantOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kickToolStripMenuItem,
            this.toolStripSeparator2,
            this.banToolStripMenuItem,
            this.banIPToolStripMenuItem});
            this.contextMenuStrip_ParticipantOptions.Name = "contextMenuStrip_ParticipantOptions";
            this.contextMenuStrip_ParticipantOptions.Size = new System.Drawing.Size(206, 76);
            // 
            // kickToolStripMenuItem
            // 
            this.kickToolStripMenuItem.Name = "kickToolStripMenuItem";
            this.kickToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.kickToolStripMenuItem.Text = "Kick";
            this.kickToolStripMenuItem.Click += new System.EventHandler(this.kickToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(202, 6);
            // 
            // banToolStripMenuItem
            // 
            this.banToolStripMenuItem.Name = "banToolStripMenuItem";
            this.banToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.banToolStripMenuItem.Text = "Ban (Character Name)";
            this.banToolStripMenuItem.Click += new System.EventHandler(this.banToolStripMenuItem_Click);
            // 
            // banIPToolStripMenuItem
            // 
            this.banIPToolStripMenuItem.Name = "banIPToolStripMenuItem";
            this.banIPToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this.banIPToolStripMenuItem.Text = "Ban (IP)";
            this.banIPToolStripMenuItem.Click += new System.EventHandler(this.banIPToolStripMenuItem_Click);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 286);
            this.Controls.Add(this.splitContainer);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form_Main";
            this.Text = "IGC.ChatServer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Main_FormClosing);
            this.Load += new System.EventHandler(this.Form_Main_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.contextMenuStrip_RoomOptions.ResumeLayout(false);
            this.contextMenuStrip_ParticipantOptions.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.SplitContainer splitContainer;

		private global::System.Windows.Forms.RichTextBox richTextBox_Log;

		private global::System.Windows.Forms.TreeView treeView_ChatConversations;

		private global::System.Windows.Forms.ContextMenuStrip contextMenuStrip_RoomOptions;

		private global::System.Windows.Forms.ToolStripMenuItem viewChatToolStripMenuItem;

		private global::System.Windows.Forms.ToolStripMenuItem destroyRoomToolStripMenuItem;

		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

		private global::System.Windows.Forms.ContextMenuStrip contextMenuStrip_ParticipantOptions;

		private global::System.Windows.Forms.ToolStripMenuItem kickToolStripMenuItem;

		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator2;

		private global::System.Windows.Forms.ToolStripMenuItem banToolStripMenuItem;

		private global::System.Windows.Forms.ToolStripMenuItem banIPToolStripMenuItem;

		private global::System.Windows.Forms.ToolStripMenuItem saveRoomMessageLogToFileToolStripMenuItem;

		private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator3;

		private global::System.Windows.Forms.Button button_ToggleChats;
	}
}
