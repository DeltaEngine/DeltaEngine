using System;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder
{
	public class AppBuildMessageViewModel
	{
		/// <summary>
		/// The ViewModel of a BuildMessage that provides all required data for the
		/// AppBuildMessagesListView.
		/// </summary>
		public AppBuildMessageViewModel(AppBuildMessage buildMessage)
		{
			MessageData = buildMessage;
		}

		internal readonly AppBuildMessage MessageData;

		public string ImagePath
		{
			get { return "/DeltaEngine.Editor.AppBuilder;component/Images/" + GetIconFilename(); }
		}

		private string GetIconFilename()
		{
			return MessageData.Type + "Icon.png";
		}

		public string IsoTime
		{
			get
			{
				var time = MessageData.TimeStamp;
				return time.Hour.ToString("00") + ":" + time.Minute.ToString("00") + ":" +
					time.Second.ToString("00");
			}
		}

		public string Message
		{
			get { return MessageData.Text; }
		}

		public string Project
		{
			get { return MessageData.Project; }
		}

		public string FileWithLineAndColumn
		{
			get
			{
				if (!IsFileAvailable)
					return "";
				return MessageData.Filename + " (" + MessageData.TextLine + "," + MessageData.TextColumn +
					")";
			}
		}

		private bool IsFileAvailable
		{
			get { return !String.IsNullOrEmpty(MessageData.Filename); }
		}

		// ncrunch: no coverage start
		public override string ToString()
		{
			string basicMessage = MessageData.Type + ": " + Project + " - " + Message;
			return IsFileAvailable ? basicMessage + " - " + FileWithLineAndColumn : basicMessage;
		}
		// ncrunch: no coverage end
	}
}