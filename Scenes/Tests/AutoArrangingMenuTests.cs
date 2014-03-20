using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests
{
	public class AutoArrangingMenuTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			menu = new AutoArrangingMenu(ButtonSize, BaseRenderLayer);
			menu.SetQuadraticBackground(new Material(ShaderFlags.Position2DColoredTextured, "SimpleSubMenuBackground"));
			text = new FontText(Font.Default, "Nothing pressed", new Rectangle(0.4f, 0.6f, 0.2f, 0.1f));
		}

		private AutoArrangingMenu menu;
		private static readonly Size ButtonSize = new Size(0.3f, 0.1f);
		private const int BaseRenderLayer = 20;
		private FontText text;

		[Test]
		public void CreateNewMenuWithContentName()
		{
			AutoArrangingMenu menuByName = ContentLoader.Load<MenuXml>("TestMenuXml");
			Assert.AreEqual(menuByName.Name, "TestMenuXml");
		}

		public class MenuXml : AutoArrangingMenu
		{
			public MenuXml(string contentName)
				: base(contentName) {}

			protected override void LoadData(Stream fileData) {}
		}

		[Test, ApproveFirstFrameScreenshot]
		public void ShowMenuWithTwoButtons()
		{
			menu.AddMenuOption(() => { text.Text = "Clicked Top Button"; });
			menu.AddMenuOption(() => { text.Text = "Clicked Bottom Button"; });
			menu.Show();
		}

		[Test]
		public void ShowMenuWithThreeButtons()
		{
			menu.AddMenuOption(() => { text.Text = "Clicked Top Button"; });
			menu.AddMenuOption(() => { text.Text = "Clicked Middle Button"; });
			menu.AddMenuOption(() => { text.Text = "Clicked Bottom Button"; });
			menu.Show();
		}

		[Test]
		public void ShowMenuWithThreeTextButtons()
		{
			menu.AddMenuOption(new Theme(), () => { text.Text = "Clicked Top Button"; }, "Top Button");
			menu.AddMenuOption(new Theme(), () => { text.Text = "Clicked Middle Button"; },
				"Middle Button");
			menu.AddMenuOption(new Theme(), () => { text.Text = "Clicked Bottom Button"; },
				"Bottom Button");
			menu.Show();
		}

		[Test, CloseAfterFirstFrame]
		public void CreatingSetsButtonSizeAndMenuCenter()
		{
			Assert.AreEqual(ButtonSize, menu.ButtonSize);
			Assert.AreEqual(Vector2D.Half, menu.Center);
		}

		[Test, CloseAfterFirstFrame, Timeout(2000)]
		public void ChangingButtonSize()
		{
			menu.ButtonSize = Size.Half;
			Assert.AreEqual(Size.Half, menu.ButtonSize);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangingCenterForSetOfButtons()
		{
			menu.Center = Vector2D.One;
			Assert.AreEqual(Vector2D.One, menu.Center);
		}

		[Test, CloseAfterFirstFrame]
		public void AddingMenuOptionAddsButton()
		{
			menu.Center = new Vector2D(0.6f, 0.6f);
			menu.AddMenuOption(() => { });
			Assert.AreEqual(2, menu.Controls.Count);
			var button = (Button)menu.Controls[1];
			Assert.AreEqual(new Theme().Button.ToString(), button.Material.ToString());
			Assert.IsTrue(button.DrawArea.IsNearlyEqual(new Rectangle(0.45f, 0.55f, 0.3f, 0.1f)));
		}

		[Test, CloseAfterFirstFrame]
		public void AddControlContentTwice()
		{
			var control = new TestControl();
			control.IsActive = false;
			Assert.AreEqual(false, control.IsActive);
			control.IsActive = true;
			Assert.AreEqual(false, control.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void ControlEnableAndDisable()
		{
			menu.AddMenuOption(() => { });
			var button = (Button)menu.Controls[1];
			button.IsEnabled = true;
			Assert.AreEqual(true, button.IsEnabled);
			button.IsEnabled = false;
			Assert.AreEqual(false, button.IsEnabled);
		}

		private class TestControl : Control
		{
			public TestControl()
				: base(Rectangle.HalfCentered)
			{
				AddChild(this);
				AddChild(this);
			}
		}

		[Test, CloseAfterFirstFrame]
		public void ClearClearsButtons()
		{
			menu.AddMenuOption(() => { });
			Assert.AreEqual(1, menu.Buttons.Count);
			menu.Clear();
			Assert.AreEqual(0, menu.Buttons.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void ClearMenuOptionsLeavesOtherControlsAlone()
		{
			var logo = new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogo");
			menu.Add(new Sprite(logo, Rectangle.One));
			menu.AddMenuOption(() => { });
			Assert.AreEqual(1, menu.Buttons.Count);
			Assert.AreEqual(3, menu.Controls.Count);
			menu.ClearMenuOptions();
			Assert.AreEqual(0, menu.Buttons.Count);
			Assert.AreEqual(2, menu.Controls.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void MenuRenderLayerIsAppliedToControls()
		{
			menu.AddMenuOption(() => { });
			var ellipse = new Ellipse(Rectangle.HalfCentered, Color.Red);
			menu.Add(ellipse);
			Assert.AreEqual(BaseRenderLayer, menu.RenderLayer);
			Assert.AreEqual(BaseRenderLayer, menu.Buttons[0].RenderLayer);
			Assert.AreEqual(BaseRenderLayer, ellipse.RenderLayer);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangingMenuRenderLayerChangesRenderLayerOfControls()
		{
			menu.AddMenuOption(() => { });
			var ellipse = new Ellipse(Rectangle.HalfCentered, Color.Red);
			menu.Add(ellipse);
			menu.RenderLayer = 30;
			Assert.AreEqual(30, menu.RenderLayer);
			Assert.AreEqual(30, menu.Buttons[0].RenderLayer);
			Assert.AreEqual(30, ellipse.RenderLayer);
		}
	}
}