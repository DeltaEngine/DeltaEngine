using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class MessagesTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void CheckChildrensRoomMessages()
		{
			var msges = DialoguesXml.KidsRoomMessages();
			Assert.AreEqual(7, msges.Length);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckBathRoomMessages()
		{
			var msges = DialoguesXml.BathRoomMessages();
			Assert.AreEqual(2, msges.Length);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckLivingRoommMessages()
		{
			var msges = DialoguesXml.LivingRoomMessages();
			Assert.AreEqual(2, msges.Length);
		}
	}
}