using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class ControlTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateNewControlsWithIDNames()
		{
			var button1 = new Button(new Theme(), new Rectangle());
			Assert.AreEqual("Button1", button1.Name);
			var button2 = new Button(new Theme(), new Rectangle());
			Assert.AreEqual("Button2", button2.Name);
			var picture1 = new Picture(new Theme(), new Material(Color.Red, ShaderFlags.Position2DColored),
				new Rectangle());
			Assert.AreEqual("Picture1", picture1.Name);
		}

		[Test]
		public void AddControlAfterRemovingOne()
		{
			var button1 = new Button(new Theme(), new Rectangle());
			Assert.AreEqual("Button1", button1.Name);
			var button2 = new Button(new Theme(), new Rectangle());
			Assert.AreEqual("Button2", button2.Name);
			button1.IsActive = false;
			var button3 = new Button(new Theme(), new Rectangle());
			Assert.AreEqual("Button1", button3.Name);
		}
	}
}