using System;
using System.IO;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;

namespace ChatServer
{
	internal static class Language
	{
		public static string YourAreBannedFromUsingFriendChat { get; private set; } = "You are Banned from using Friend Chat";

		public static string YouHaveJoinedTheConversation { get; private set; } = "You have joined the conversation";

		public static string IsBannedFromUsingFriendChat { get; private set; } = "is Banned from using Friend Chat";

		public static string HasBeenBannedFromUsingFriendChat { get; private set; } = "has been Banned from using Friend Chat";

		public static string HasJoinedTheConversation { get; private set; } = "has joined the Conversation";

		public static string ConversationEndedByAdmin { get; private set; } = "Conversation ended by Admin";

		public static string HasBeenRemovedFromTheConversationByAdmin { get; private set; } = "has been removed from the Conversation by Admin";

		public static string HasLeftTheConversation { get; private set; } = "has left the Conversation";

		public static string ContentFiltered { get; private set; } = "Content filtered";

		public static string WelcomeRememberToBehave { get; private set; } = "Welcome! Remembert to Behave!";

		public static void Read()
		{
			if (!File.Exists("Data\\Languages\\" + Configs.LanguageFileName))
			{
				MessageBox.Show("'Data\\Languages\\" + Configs.LanguageFileName + "' file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Environment.Exit(0);
				return;
			}
			IniData iniData = new FileIniDataParser().ReadFile("Data\\Languages\\" + Configs.LanguageFileName, Configs.LanguageEncoding);
			Language.YourAreBannedFromUsingFriendChat = (iniData["General"]["YourAreBannedFromUsingFriendChat"] ?? Language.YourAreBannedFromUsingFriendChat);
			Language.IsBannedFromUsingFriendChat = (iniData["General"]["IsBannedFromUsingFriendChat"] ?? Language.IsBannedFromUsingFriendChat);
			Language.HasBeenBannedFromUsingFriendChat = (iniData["General"]["HasBeenBannedFromUsingFriendChat"] ?? Language.HasBeenBannedFromUsingFriendChat);
			Language.YouHaveJoinedTheConversation = (iniData["General"]["YouHaveJoinedTheConversation"] ?? Language.YouHaveJoinedTheConversation);
			Language.HasJoinedTheConversation = (iniData["General"]["HasJoinedTheConversation"] ?? Language.HasJoinedTheConversation);
			Language.ConversationEndedByAdmin = (iniData["General"]["ConversationEndedByAdmin"] ?? Language.ConversationEndedByAdmin);
			Language.HasBeenRemovedFromTheConversationByAdmin = (iniData["General"]["HasBeenRemovedFromTheConversationByAdmin"] ?? Language.HasBeenRemovedFromTheConversationByAdmin);
			Language.HasLeftTheConversation = (iniData["General"]["HasLeftTheConversation"] ?? Language.HasLeftTheConversation);
			Language.ContentFiltered = (iniData["General"]["ContentFiltered"] ?? Language.ContentFiltered);
			Language.WelcomeRememberToBehave = (iniData["General"]["WelcomeRememberToBehave"] ?? Language.WelcomeRememberToBehave);
		}
	}
}
