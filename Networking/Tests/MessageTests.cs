using DeltaEngine.Core;
using DeltaEngine.Networking.Messages;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	public class MessageTests
	{
		[Test]
		public void SaveAndLoadInfoMessage()
		{
			var info = new LogInfoMessage("A test info message");
			var data = BinaryDataExtensions.SaveToMemoryStream(info);
			var loadedInfo = data.CreateFromMemoryStream() as LogInfoMessage;
			Assert.AreEqual(info.Text, loadedInfo.Text);
			Assert.AreEqual(info.TimeStamp, loadedInfo.TimeStamp);
		}
	}
}