using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ChatServer
{
	internal class ProhibitedWords
	{
		public static void Read()
		{
			string path = "Data\\IGC_ProhibitedWords.xml";
			if (!File.Exists(path))
			{
				path = "..\\IGCData\\IGC_ProhibitedWords.xml";
				if (!File.Exists(path))
				{
					return;
				}
			}
			try
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Open))
				{
					XDocument.Load(fileStream).Element("ProhibitedWords").Elements("Restrict").ToList<XElement>().ForEach(delegate(XElement elem)
					{
						List<string> words = ProhibitedWords._words;
						string value = elem.Attribute("Word").Value;
						words.Add((value != null) ? value.Trim().ToLower() : null);
					});
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("There was an error while trying to parse xml file.\n" + ex.Message, "ProhibitedWords System", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		public static bool Exists(string word)
		{
			return ProhibitedWords._words.Contains((word != null) ? word.Trim().ToLower() : null);
		}

		private static List<string> _words = new List<string>();
	}
}
