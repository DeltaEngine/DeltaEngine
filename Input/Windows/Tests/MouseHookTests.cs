using NUnit.Framework;

namespace DeltaEngine.Input.Windows.Tests
{
	public class MouseHookTests
	{
		[Test]
		public void TestIsPressed()
		{
			Assert.True(MouseHook.IsPressed(0x0201));
			Assert.False(MouseHook.IsPressed(0));
		}

		[Test]
		public void TestIsReleased()
		{
			Assert.True(MouseHook.IsReleased(0x00A2));
			Assert.False(MouseHook.IsReleased(0));
		}

		[Test]
		public void TestGetMessageButton()
		{
			Assert.AreEqual(MouseButton.Left, MouseHook.GetMessageButton(0x00A2, 0));
			Assert.AreEqual(MouseButton.Right, MouseHook.GetMessageButton(0x0205, 0));
			Assert.AreEqual(MouseButton.Middle, MouseHook.GetMessageButton(0x0209, 0));
			Assert.AreEqual(MouseButton.X1, MouseHook.GetMessageButton(0x020B, 65536));
			Assert.AreEqual(MouseButton.X2, MouseHook.GetMessageButton(0x020B, 0));
		}

		[Test]
		public void TestButtonQueue()
		{
			var hook = new MouseHook();
			hook.ProcessButtonQueue(State.Released, MouseButton.Left);
		}
	}
}