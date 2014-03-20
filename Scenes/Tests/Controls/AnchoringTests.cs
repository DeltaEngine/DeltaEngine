using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class AnchoringTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			anchoring = new AnchoringState();
			control = new Button(new Rectangle(0.1f, 0.2f, 0.3f, 0.4f));
			landscapeControl = new Button(new Rectangle(0.0f, 0.0f, 0.6f, 0.5f));
			portraitControl = new Button(new Rectangle(0.0f, 0.0f, 0.4f, 0.5f));
		}

		private AnchoringState anchoring;
		private Control control;
		private Control landscapeControl;
		private Control portraitControl;

		[Test]
		public void DrawControlsAnchoredToScreenCornersAndEdges()
		{
			new Button(ButtonDrawArea, "Left") { LeftMargin = Left };
			new Button(ButtonDrawArea, "Right") { RightMargin = Right };
			new Button(ButtonDrawArea, "Top") { TopMargin = Top };
			new Button(ButtonDrawArea, "Bottom") { BottomMargin = Bottom };
			new Button(ButtonDrawArea, "Top\nLeft") { TopMargin = Top, LeftMargin = Left };
			new Button(ButtonDrawArea, "Top\nRight") { TopMargin = Top, RightMargin = Right };
			new Button(ButtonDrawArea, "Bottom\nLeft") { BottomMargin = Bottom, LeftMargin = Left };
			new Button(ButtonDrawArea, "Bottom\nRight") { BottomMargin = Bottom, RightMargin = Right };
		}

		//ncrunch: no coverage start
		private static readonly Rectangle ButtonDrawArea = new Rectangle(0.0f, 0.0f, 0.1f, 0.1f);
		private static readonly Margin Left = new Margin(Edge.Left, 0.01f);
		private static readonly Margin Right = new Margin(Edge.Right, 0.01f);
		private static readonly Margin Top = new Margin(Edge.Top, 0.01f);
		private static readonly Margin Bottom = new Margin(Edge.Bottom, 0.01f);
		//ncrunch: no coverage end

		[Test]
		public void DrawControlsAnchoredTogether()
		{
			var centerButton = new Button(ButtonDrawArea, "Click Me");
			var rnd = Randomizer.Current;
			centerButton.Clicked +=
				() => centerButton.TopLeft = new Vector2D(rnd.Get(0.3f, 0.6f), rnd.Get(0.3f, 0.6f));
			new Button(ButtonDrawArea) // button left of center button
			{
				RightMargin = new Margin(centerButton, Edge.Left, 0.01f),
				TopMargin = new Margin(centerButton, Edge.Top, 0.0f)
			};
			new Button(ButtonDrawArea) // button right of center button
			{
				LeftMargin = new Margin(centerButton, Edge.Right, 0.01f),
				TopMargin = new Margin(centerButton, Edge.Top, 0.0f)
			};
			new Button(ButtonDrawArea) // button above center button
			{
				BottomMargin = new Margin(centerButton, Edge.Top, 0.01f),
				LeftMargin = new Margin(centerButton, Edge.Left, 0.0f)
			};
			new Button(ButtonDrawArea) // button below center button
			{
				TopMargin = new Margin(centerButton, Edge.Bottom, 0.01f),
				LeftMargin = new Margin(centerButton, Edge.Left, 0.0f)
			};
			centerButton.Click();
		}

		[Test]
		public void Test()
		{
			var centerButton = new Button(ButtonDrawArea, "Click Me");
			var rnd = Randomizer.Current;
			centerButton.Clicked +=
				() => centerButton.TopLeft = new Vector2D(rnd.Get(0.3f, 0.6f), rnd.Get(0.3f, 0.6f));
			new Button(ButtonDrawArea) // button above center button
			{
				BottomMargin = new Margin(centerButton, Edge.Top, 0.01f),
				RightMargin = new Margin(centerButton, Edge.Right, 0.05f)
			};
			centerButton.Click();
		}

		[Test, CloseAfterFirstFrame]
		public void Constructor()
		{
			Assert.AreEqual(new Margin(Edge.Left), anchoring.LeftMargin);
			Assert.AreEqual(new Margin(Edge.Right), anchoring.RightMargin);
			Assert.AreEqual(new Margin(Edge.Top), anchoring.TopMargin);
			Assert.AreEqual(new Margin(Edge.Bottom), anchoring.BottomMargin);
			Assert.AreEqual(-1, anchoring.PercentageSpan);
		}

		[Test, CloseAfterFirstFrame]
		public void WithNoMarginsDrawAreaIsUnchanged()
		{
			Assert.AreEqual(control.DrawArea, anchoring.CalculateDrawArea(control));
		}

		[Test, CloseAfterFirstFrame]
		public void TopMargin()
		{
			anchoring.TopMargin = new Margin(Edge.Top, 0.05f);
			AssertNearlyEqual(new Rectangle(0.35f, 0.26875f, 0.3f, 0.4f),
				anchoring.CalculateDrawArea(control));
		}

		private static void AssertNearlyEqual(Rectangle expected, Rectangle actual)
		{
			Assert.AreEqual(expected.Left, actual.Left, 0.0001f);
			Assert.AreEqual(expected.Top, actual.Top, 0.0001f);
			Assert.AreEqual(expected.Width, actual.Width, 0.0001f);
			Assert.AreEqual(expected.Height, actual.Height, 0.0001f);
		}

		[Test, CloseAfterFirstFrame]
		public void BottomMargin()
		{
			anchoring.BottomMargin = new Margin(Edge.Bottom, 0.05f);
			AssertNearlyEqual(new Rectangle(0.35f, 0.33125f, 0.3f, 0.4f),
				anchoring.CalculateDrawArea(control));
		}

		[Test, CloseAfterFirstFrame]
		public void LeftMargin()
		{
			anchoring.LeftMargin = new Margin(Edge.Left, 0.05f);
			AssertNearlyEqual(new Rectangle(0.05f, 0.3f, 0.3f, 0.4f),
				anchoring.CalculateDrawArea(control));
		}

		[Test, CloseAfterFirstFrame]
		public void RightMargin()
		{
			anchoring.RightMargin = new Margin(Edge.Right, 0.05f);
			AssertNearlyEqual(new Rectangle(0.65f, 0.3f, 0.3f, 0.4f),
				anchoring.CalculateDrawArea(control));
		}

		[Test, CloseAfterFirstFrame]
		public void LeftRightMargin()
		{
			anchoring.LeftMargin = new Margin(Edge.Left, 0.05f);
			anchoring.RightMargin = new Margin(Edge.Right, 0.15f);
			AssertNearlyEqual(new Rectangle(0.05f, -0.0333f, 0.8f, 1.0666f),
				anchoring.CalculateDrawArea(control));
		}

		[Test, CloseAfterFirstFrame]
		public void TopBottomMargin()
		{
			anchoring.TopMargin = new Margin(Edge.Top, 0.05f);
			anchoring.BottomMargin = new Margin(Edge.Bottom, 0.15f);
			AssertNearlyEqual(new Rectangle(0.364f, 0.2688f, 0.2719f, 0.3625f),
				anchoring.CalculateDrawArea(control));
		}

		[Test, CloseAfterFirstFrame]
		public void NoTopMargin()
		{
			anchoring.LeftMargin = new Margin(Edge.Left, 0.02f);
			anchoring.RightMargin = new Margin(Edge.Right, 0.04f);
			anchoring.BottomMargin = new Margin(Edge.Bottom, 0.06f);
			AssertNearlyEqual(new Rectangle(0.02f, -0.532f, 0.94f, 1.2533f),
				anchoring.CalculateDrawArea(control));
		}

		[Test, CloseAfterFirstFrame]
		public void NoBottomMargin()
		{
			anchoring.LeftMargin = new Margin(Edge.Left, 0.02f);
			anchoring.RightMargin = new Margin(Edge.Right, 0.04f);
			anchoring.TopMargin = new Margin(Edge.Top, 0.06f);
			AssertNearlyEqual(new Rectangle(0.02f, 0.2788f, 0.94f, 1.2533f),
				anchoring.CalculateDrawArea(control));
		}

		[Test, CloseAfterFirstFrame]
		public void NoLeftMargin()
		{
			anchoring.RightMargin = new Margin(Edge.Right, 0.02f);
			anchoring.TopMargin = new Margin(Edge.Top, 0.04f);
			anchoring.BottomMargin = new Margin(Edge.Bottom, 0.06f);
			AssertNearlyEqual(new Rectangle(0.6331f, 0.2587f, 0.3469f, 0.4625f),
				anchoring.CalculateDrawArea(control));
		}

		[Test, CloseAfterFirstFrame]
		public void NoRightMargin()
		{
			anchoring.LeftMargin = new Margin(Edge.Left, 0.02f);
			anchoring.TopMargin = new Margin(Edge.Top, 0.04f);
			anchoring.BottomMargin = new Margin(Edge.Bottom, 0.06f);
			AssertNearlyEqual(new Rectangle(0.02f, 0.2587f, 0.3469f, 0.4625f),
				anchoring.CalculateDrawArea(control));
		}

		[Test, CloseAfterFirstFrame]
		public void AllMargins()
		{
			anchoring.LeftMargin = new Margin(Edge.Left, 0.02f);
			anchoring.RightMargin = new Margin(Edge.Right, 0.04f);
			anchoring.TopMargin = new Margin(Edge.Top, 0.06f);
			anchoring.BottomMargin = new Margin(Edge.Bottom, 0.08f);
			AssertNearlyEqual(new Rectangle(0.02f, 0.2788f, 0.94f, 0.4225f),
				anchoring.CalculateDrawArea(control));
		}

		[Test, CloseAfterFirstFrame]
		public void PercentHintLandscapeImageLandscapeScreen()
		{
			anchoring.PercentageSpan = 0.4f;
			AssertNearlyEqual(new Rectangle(0.365f, 0.3875f, 0.27f, 0.225f),
				anchoring.CalculateDrawArea(landscapeControl));
		}

		[Test, CloseAfterFirstFrame]
		public void PercentHintPortraitImageLandscapeScreen()
		{
			anchoring.PercentageSpan = 0.4f;
			AssertNearlyEqual(new Rectangle(0.41f, 0.3875f, 0.18f, 0.225f),
				anchoring.CalculateDrawArea(portraitControl));
		}

		[Test, CloseAfterFirstFrame]
		public void PercentHintLandscapeImagePortraitScreen()
		{
			Resolve<Window>().ViewportPixelSize = new Size(600, 800);
			anchoring.PercentageSpan = 0.4f;
			AssertNearlyEqual(new Rectangle(0.35f, 0.375f, 0.3f, 0.25f),
				anchoring.CalculateDrawArea(landscapeControl));
		}

		[Test, CloseAfterFirstFrame]
		public void PercentHintPortraitImagePortraitScreen()
		{
			Resolve<Window>().ViewportPixelSize = new Size(600, 800);
			anchoring.PercentageSpan = 0.4f;
			AssertNearlyEqual(new Rectangle(0.35f, 0.3125f, 0.3f, 0.375f),
				anchoring.CalculateDrawArea(portraitControl));
		}

		[Test, CloseAfterFirstFrame]
		public void SetAndGetMargins()
		{
			var button = new Button(Rectangle.One)
			{
				LeftMargin = new Margin(Edge.Left, 1),
				RightMargin = new Margin(Edge.Right, 2),
				TopMargin = new Margin(Edge.Top, 3),
				BottomMargin = new Margin(Edge.Bottom, 4)
			};
			Assert.AreEqual(new Margin(Edge.Left, 1), button.LeftMargin);
			Assert.AreEqual(new Margin(Edge.Right, 2), button.RightMargin);
			Assert.AreEqual(new Margin(Edge.Top, 3), button.TopMargin);
			Assert.AreEqual(new Margin(Edge.Bottom, 4), button.BottomMargin);
		}

	}
}