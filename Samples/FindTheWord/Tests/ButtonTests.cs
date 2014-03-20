using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	public class ButtonTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowHoverState()
		{
			new GameButton("Wurm1", new Rectangle(0, 0, 0.5f, 0.5f));
		}

		[Test]
		public void ShowClickableButton()
		{
			var button = new GameButton("Wurm1", new Rectangle(0, 0, 0.5f, 0.5f));
			button.Clicked += () => button.Alpha = button.Alpha == 1.0f ? 0.5f : 1.0f;
		}

		[Test]
		public void CreateButton()
		{
			var button = new GameButton("Wurm1", new Rectangle(0, 0, 0.5f, 0.5f));
			Assert.IsNotNull(button);
		}
	}
}