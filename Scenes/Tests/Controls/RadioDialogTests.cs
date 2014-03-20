using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class RadioDialogTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			dialog = new RadioDialog(Rectangle.FromCenter(0.5f, 0.5f, 0.4f, 0.3f));
			AdvanceTimeAndUpdateEntities();
			dialog.AddButton("Top Button");
			dialog.AddButton("Middle Button");
			dialog.AddButton("Bottom Button");
			dialog.Add(new FontText(Font.Default, "", new Rectangle(0.4f, 0.7f, 0.2f, 0.1f)));
			AdvanceTimeAndUpdateEntities();
			dialog.Start<UpdateText>();
			InitializeMouse();
		}

		private RadioDialog dialog;

		private class UpdateText : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (RadioDialog dialog in entities)
					dialog.Get<FontText>().Text = "Button '" + dialog.SelectedButton.Text + "'";
			}
		}

		private void InitializeMouse()
		{
			mouse = Resolve<Mouse>() as MockMouse;
			if (mouse == null)
				return; //ncrunch: no coverage
			mouse.SetNativePosition(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities();
		}

		private MockMouse mouse;

		[Test]
		public void RenderRadioDialogWithThreeButtonsWithTheMiddleDisabled()
		{
			var buttons = dialog.Get<List<RadioButton>>();
			buttons[1].IsEnabled = false;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(Color.Gray, buttons[0].Color);
			Assert.AreEqual(Color.DarkGray, buttons[1].Color);
		}

		[Test]
		public void RenderGrowingRadioDialog()
		{
			dialog.Start<Grow>();
			dialog.Start<SetText>();
		}

		private class SetText : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (RadioDialog dialog in entities)
					dialog.Get<FontText>().Text = "Button '" + dialog.SelectedButton.Text + "'";
			}
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingRadioButtonSelectsIt()
		{
			if (mouse == null)
				return; // ncrunch: no coverage
			var buttons = dialog.Get<List<RadioButton>>();
			Assert.IsFalse(buttons[1].State.IsSelected);
			PressAndReleaseMouse(Vector2D.One);
			Assert.IsFalse(buttons[1].State.IsSelected);
			PressAndReleaseMouse(new Vector2D(0.35f, 0.5f));
			Assert.IsTrue(buttons[1].State.IsSelected);
		}

		private void PressAndReleaseMouse(Vector2D position)
		{
			SetMouseState(State.Pressing, position);
			SetMouseState(State.Releasing, position);
		}

		private void SetMouseState(State state, Vector2D position)
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			mouse.SetNativePosition(position);
			mouse.SetButtonState(MouseButton.Left, state);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingOneRadioButtonCausesTheOthersToUnselect()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			var buttons = dialog.Get<List<RadioButton>>();
			PressAndReleaseMouse(new Vector2D(0.35f, 0.5f));
			Assert.IsTrue(buttons[1].State.IsSelected);
			PressAndReleaseMouse(new Vector2D(0.35f, 0.6f));
			Assert.IsFalse(buttons[1].State.IsSelected);
			Assert.IsTrue(buttons[2].State.IsSelected);
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingRadioButtonDoesNotSelectItIfDisabled()
		{
			var buttons = dialog.Get<List<RadioButton>>();
			buttons[1].IsEnabled = false;
			PressAndReleaseMouse(new Vector2D(0.35f, 0.5f));
			Assert.IsFalse(buttons[1].State.IsSelected);
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingDisabledRadioButtonDoesNotCauseTheOthersToUnselect()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			var buttons = dialog.Get<List<RadioButton>>();
			buttons[2].IsEnabled = false;
			PressAndReleaseMouse(new Vector2D(0.35f, 0.5f));
			PressAndReleaseMouse(new Vector2D(0.35f, 0.6f));
			Assert.IsTrue(buttons[1].State.IsSelected);
			Assert.IsFalse(buttons[2].State.IsSelected);
		}

		[Test, CloseAfterFirstFrame]
		public void DisablingDialogDisablesAllButtons()
		{
			dialog.IsEnabled = false;
			var buttons = dialog.Get<List<RadioButton>>();
			foreach (RadioButton button in buttons)
				Assert.IsFalse(button.IsEnabled);
		}

		[Test, CloseAfterFirstFrame]
		public void ReEnableDialogEnablesAllButtons()
		{
			dialog.IsEnabled = false;
			dialog.IsEnabled = true;
			var buttons = dialog.Get<List<RadioButton>>();
			foreach (RadioButton button in buttons)
				Assert.IsTrue(button.IsEnabled);
		}

		[Test]
		public void RenderRadioDialogAttachedToMouse()
		{
			new Command(point => dialog.DrawArea = Rectangle.FromCenter(point, dialog.DrawArea.Size)).
				Add(new MouseMovementTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void SaveAndLoad()
		{
			var stream = BinaryDataExtensions.SaveToMemoryStream(dialog);
			var loadedDialog = (RadioDialog)stream.CreateFromMemoryStream();
			Assert.AreEqual(3, loadedDialog.Buttons.Count);
			Assert.AreEqual(dialog.Buttons[1].DrawArea, loadedDialog.Buttons[1].DrawArea);
			Assert.AreEqual(dialog.Buttons[1].Text, loadedDialog.Buttons[1].Text);
		}

		[Test]
		public void DrawLoadedRadioDialog()
		{
			var stream = BinaryDataExtensions.SaveToMemoryStream(dialog);
			dialog.IsActive = false;
			stream.CreateFromMemoryStream();
		}

		[Test]
		public void DrawLoadedRadioDialogAttachedToMouse()
		{
			var stream = BinaryDataExtensions.SaveToMemoryStream(dialog);
			dialog.IsActive = false;
			var loadedDialog = (RadioDialog)stream.CreateFromMemoryStream();
			new Command(point => loadedDialog.DrawArea = Rectangle.FromCenter(point,
				loadedDialog.DrawArea.Size)).Add(new MouseMovementTrigger());
		}
	}
}