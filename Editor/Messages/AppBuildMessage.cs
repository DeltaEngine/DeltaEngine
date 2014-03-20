using System;

namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// A message sent from the BuildService which informs about the specific compilation state.
	/// </summary>
	public class AppBuildMessage
	{
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		protected AppBuildMessage() {}

		public AppBuildMessage(string text)
		{
			Text = text;
			TimeStamp = DateTime.Now;
			Type = AppBuildMessageType.BuildInfo;
			Filename = "";
			TextLine = "";
			TextColumn = "";
		}

		public string Text { get; private set; }
		public DateTime TimeStamp { get; private set; }
		public AppBuildMessageType Type { get; set; }
		public string Project { get; set; }
		public string Filename { get; set; }
		public string TextLine { get; set; }
		public string TextColumn { get; set; }

		public override string ToString()
		{
			return Type + ": " + Text;
		}
	}
}