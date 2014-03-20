using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Terminal.Tests
{
	public class ConsoleTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			RegisterMock(new TestCommand());
			Resolve<Window>().ViewportPixelSize = new Size(500, 500);
			console = new Console();
			keyboard = Resolve<Keyboard>() as MockKeyboard;
			lastKey = Key.None;
			AdvanceTimeAndUpdateEntities();
		}

		private Console console;
		private MockKeyboard keyboard;
		private Key lastKey;

		[Test]
		public void CreateConsole()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			Assert.AreEqual(0.01f, console.DrawArea.Left);
			Assert.AreEqual(0.1f, console.DrawArea.Top);
			Assert.AreEqual(0.99f, console.DrawArea.Right);
			Assert.AreEqual(0.98f, console.DrawArea.Width);
			Assert.AreEqual(0.352f, console.DrawArea.Bottom);
			Assert.IsTrue(console.IsEnabled);
			Assert.AreEqual(Console.EnabledBackgroundColor, console.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void TypingGoesIntoTheCommandLine()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKeys(new List<Key> { Key.A, Key.B, Key.C });
			Assert.AreEqual("> abc_", console.command.Text);
		}

		private void PressKeys(List<Key> keys)
		{
			foreach (Key key in keys)
				PressKey(key);
		}

		private void PressKey(Key key)
		{
			if (lastKey != Key.None)
				keyboard.SetKeyboardState(lastKey, State.Pressed);
			keyboard.SetKeyboardState(key, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			lastKey = key;
		}

		[Test, CloseAfterFirstFrame]
		public void PressingEnterExecutesCommandTrimsItAndMovesItToHistory()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKeys(new List<Key> { Key.A, Key.Space, Key.Space, Key.B, Key.Enter });
			Assert.AreEqual("> _", console.command.Text);
			Assert.AreEqual("> a b\nError: Unknown console command 'a'", console.history.Text);
			Assert.AreEqual("", console.autoCompletions.Text);
			Assert.AreEqual(1, console.commands.Count);
			Assert.AreEqual("a b", console.commands[0]);
		}

		[Test, CloseAfterFirstFrame]
		public void FillingHistoryRemovesTheFirstHistoryLine()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKeys(new List<Key> 
			{ Key.D1, Key.Enter, Key.D2, Key.Enter, Key.D3, Key.Enter, Key.D4, Key.Enter });
			Assert.AreEqual(
				"> 2\nError: Unknown console command '2'\n" + 
				"> 3\nError: Unknown console command '3'\n" +
				"> 4\nError: Unknown console command '4'", console.history.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void RenderDisabledConsole()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			console.IsEnabled = false;
		}

		[Test, CloseAfterFirstFrame]
		public void TypingDoesNotGoIntoTheCommandLineIfNotEnabled()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			console.IsEnabled = false;
			PressKeys(new List<Key> { Key.A, Key.B, Key.C });
			Assert.AreEqual("> _", console.command.Text);
			Assert.AreEqual(Console.DisabledBackgroundColor, console.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void TypingUpdatesAutoCompletions()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKeys(new List<Key> { Key.A });
			Assert.AreEqual("AddFloats Single Single\nAddInts Int32 Int32", console.autoCompletions.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void TabAutoCompletes()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKeys(new List<Key> { Key.A, Key.D, Key.D, Key.F, Key.Tab });
			Assert.AreEqual("> AddFloats_", console.command.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void PressingF12Inactivates()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKey(Key.F12);
			Assert.IsFalse(console.IsActive);
			Assert.IsFalse(console.history.IsActive);
			Assert.IsFalse(console.command.IsActive);
			Assert.IsFalse(console.autoCompletions.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void PressingF12TwiceReactivates()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKeys(new List<Key> { Key.F12, Key.F12 });
			Assert.IsTrue(console.IsActive);
			Assert.IsTrue(console.history.IsActive);
			Assert.IsTrue(console.command.IsActive);
			Assert.IsTrue(console.autoCompletions.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void PressingCursorUpGetsTheLastCommand()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKeys(new List<Key> { Key.D1, Key.Enter, Key.D2, Key.Enter, Key.CursorUp });
			Assert.AreEqual("> 2_", console.command.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void PressingCursorUpTwiceGetsTheLastButOneCommand()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKeys(new List<Key> { Key.D1, Key.Enter, Key.D2, Key.Enter, Key.CursorUp, Key.CursorUp });
			Assert.AreEqual("> 1_", console.command.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void PressingCursorUpManyTimesGetsTheFirstCommand()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKeys(new List<Key>
			{ Key.D1, Key.Enter, Key.D2, Key.Enter, Key.CursorUp, Key.CursorUp, Key.CursorUp });
			Assert.AreEqual("> 1_", console.command.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void PressingCursorUpTwiceThenCursorDownGetsTheLastCommand()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKeys(new List<Key>
			{ Key.D1, Key.Enter, Key.D2, Key.Enter, Key.CursorUp, Key.CursorUp, Key.CursorDown });
			Assert.AreEqual("> 2_", console.command.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void PressingCursorUpThenCursorDownLotsOfTimesClearsTheCommandLine()
		{
			if (keyboard == null)
				return; //ncrunch: no coverage
			PressKeys(new List<Key>
			{ Key.D1, Key.Enter, Key.D2, Key.Enter, Key.CursorUp, 
				Key.CursorUp, Key.CursorDown, Key.CursorDown });
			Assert.AreEqual("> _", console.command.Text);
		}
	}
}