namespace ChatServer
{
	public partial class Form_NoLicenseFound : global::System.Windows.Forms.Form, global::Authenticator.Models.INoLicenseForm
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
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::ChatServer.Form_NoLicenseFound));
			this.label1 = new global::System.Windows.Forms.Label();
			this.label2 = new global::System.Windows.Forms.Label();
			this.textBox_HWID = new global::System.Windows.Forms.TextBox();
			this.button_OK = new global::System.Windows.Forms.Button();
			this.linkLabel_Troubleshoot = new global::System.Windows.Forms.LinkLabel();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(79, 25);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(393, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "No active license found. Please activate this software using IGC.GameServer";
			this.label2.AutoSize = true;
			this.label2.Location = new global::System.Drawing.Point(95, 57);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(71, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Hardware ID";
			this.textBox_HWID.Location = new global::System.Drawing.Point(168, 54);
			this.textBox_HWID.Name = "textBox_HWID";
			this.textBox_HWID.ReadOnly = true;
			this.textBox_HWID.Size = new global::System.Drawing.Size(265, 22);
			this.textBox_HWID.TabIndex = 2;
			this.textBox_HWID.Text = "XXXX-XXXX-XXXX-XXXX-XXXX-XXXX-XXXX-XXXX";
			this.textBox_HWID.MouseDown += new global::System.Windows.Forms.MouseEventHandler(this.textBox_HWID_MouseDown);
			this.button_OK.Location = new global::System.Drawing.Point(186, 91);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new global::System.Drawing.Size(170, 31);
			this.button_OK.TabIndex = 3;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new global::System.EventHandler(this.button_OK_Click);
			this.linkLabel_Troubleshoot.Dock = global::System.Windows.Forms.DockStyle.Bottom;
			this.linkLabel_Troubleshoot.Location = new global::System.Drawing.Point(0, 128);
			this.linkLabel_Troubleshoot.Name = "linkLabel_Troubleshoot";
			this.linkLabel_Troubleshoot.Padding = new global::System.Windows.Forms.Padding(0, 0, 10, 10);
			this.linkLabel_Troubleshoot.Size = new global::System.Drawing.Size(536, 28);
			this.linkLabel_Troubleshoot.TabIndex = 4;
			this.linkLabel_Troubleshoot.TabStop = true;
			this.linkLabel_Troubleshoot.Text = "Help";
			this.linkLabel_Troubleshoot.TextAlign = global::System.Drawing.ContentAlignment.BottomRight;
			this.linkLabel_Troubleshoot.LinkClicked += new global::System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_Troubleshoot_LinkClicked);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(536, 156);
			base.Controls.Add(this.linkLabel_Troubleshoot);
			base.Controls.Add(this.button_OK);
			base.Controls.Add(this.textBox_HWID);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			this.Font = new global::System.Drawing.Font("Segoe UI", 8.25f, global::System.Drawing.FontStyle.Regular, global::System.Drawing.GraphicsUnit.Point, 0);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (global::System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Form_NoLicenseFound";
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "IGC.ChatServer";
			base.TopMost = true;
			base.Load += new global::System.EventHandler(this.Form_NoLicenseFound_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.Label label2;

		private global::System.Windows.Forms.TextBox textBox_HWID;

		private global::System.Windows.Forms.Button button_OK;

		private global::System.Windows.Forms.LinkLabel linkLabel_Troubleshoot;
	}
}
