using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content.Xml;

namespace $safeprojectname$
{
	public class DialoguesXml : XmlContent
	{
		protected DialoguesXml(string contentName)
			: base(contentName)
		{
			messages = new List<Dialogue>();
		}

		internal readonly List<Dialogue> messages;

		protected override void LoadData(Stream fileData)
		{
			base.LoadData(fileData);
			foreach (var dialogue in Data.GetChildren("Dialogue"))
				messages.Add(new Dialogue
				{
					character = dialogue.GetAttributeValue("Character", ""),
					text = dialogue.GetAttributeValue("Text", "")
				});
		}

		public struct Dialogue
		{
			public string character;
			public string text;
		}

		public Dialogue GetMessage(int messageCount)
		{
			return messages.ToArray()[messageCount];
		}

		public static string[] KidsRoomMessages()
		{
			return new[]
			{
				"Time to build some towers and \n" + "prepare for the creeps heading your way...",
				"To build a tower, select a free spot on the field,\n" +
					" and then select the Fire Tower from the menu...",
				"Well done...",
				"Here comes the first Creep. Get ready to Rumble!", "Good job! \n",
				"Your life as a Hero has just begun! \n" + " There is much to learn to defeat \n" +
					"the Creeps and their Evil Masters!",
				"Now build a few more Fire Towers to stop the next wave..."
			};
		}

		public static string[] BathRoomMessages()
		{
			return new[] { "Welcome to the next level of Creepy Towers!", "Now, try it out!" };
		}

		public static string[] LivingRoomMessages()
		{
			return new[] { "Welcome to the Living Room in the House!", "Now, its your turn." };
		}
	}
}