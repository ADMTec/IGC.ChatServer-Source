using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Authenticator.Models;
using ToolsAuthServer.Models;

namespace ChatServer
{
	public partial class Form_NoLicenseFound : Form, INoLicenseForm
	{
		public ErrorCodes? AuthenticationError { get; set; }

		public Form_NoLicenseFound()
		{
			this.InitializeComponent();
			this.Text = base.ProductName;
			this.GetHardwareID();
		}

		private void GetHardwareID()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			if (!Kernel32.GetEnvironmentVariable("WLHardwareGetID", stringBuilder, 100))
			{
				return;
			}
			this.textBox_HWID.Text = stringBuilder.ToString();
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void textBox_HWID_MouseDown(object sender, MouseEventArgs e)
		{
			this.textBox_HWID.SelectAll();
		}

		private void linkLabel_Troubleshoot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://community.igcn.mu/tutorials/article/70-troubleshooting-running-selection-of-tools/");
		}

		private void Form_NoLicenseFound_Load(object sender, EventArgs e)
		{
			this.parseAuthenticationError();
		}

		private void parseAuthenticationError()
		{
			if (this.AuthenticationError == null)
			{
				return;
			}
			string text = string.Format("Code: {0}", (byte)this.AuthenticationError.Value);
			if (this.AuthenticationError.Value >= ErrorCodes.PUBLIC_LicenseNotFound)
			{
				text = text + " (" + this.AuthenticationError.ToString().Split(new char[]
				{
					'_'
				}, 2).Last<string>() + ")";
			}
			this.linkLabel_Troubleshoot.Text = "[" + text + "] -> Get Help";
		}
	}
}
