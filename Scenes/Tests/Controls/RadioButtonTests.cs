using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class RadioButtonTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			centerButton = new RadioButton(Center, "Hello");
		}

		private RadioButton centerButton;
		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.1f);

		[Test, ApproveFirstFrameScreenshot]
		public void RenderRadioButton() {}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderThreeRadioButtonsWithTheMiddleDisabled()
		{
			new RadioButton(Top, "Hello");
			new RadioButton(Bottom, "Hey");
			centerButton.IsEnabled = false;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(Color.DarkGray, centerButton.Color);
		}

		private static readonly Rectangle Top = Rectangle.FromCenter(0.5f, 0.4f, 0.5f, 0.1f);
		private static readonly Rectangle Bottom = Rectangle.FromCenter(0.5f, 0.6f, 0.5f, 0.1f);

		[Test, CloseAfterFirstFrame]
		public void DefaultsToEnabled()
		{
			Assert.IsTrue(centerButton.IsEnabled);
			Assert.AreEqual(Color.Gray, centerButton.Color);
		}

		[Test]
		public void RenderRadioButtonAttachedToMouse()
		{
			new Command(
				point => centerButton.DrawArea = Rectangle.FromCenter(point, centerButton.DrawArea.Size)).
				Add(new MouseMovementTrigger());
		}

		[Test]
		public void SaveAndLoad()
		{
			centerButton.Text = "Original";
			var stream = BinaryDataExtensions.SaveToMemoryStream(centerButton);
			var loadedButton = (RadioButton)stream.CreateFromMemoryStream();
			loadedButton.Text = "Loaded";
			Assert.AreEqual(Center, loadedButton.DrawArea);
			loadedButton.DrawArea = loadedButton.DrawArea.Move(new Vector2D(0.0f, 0.1f));
		}
	}
}