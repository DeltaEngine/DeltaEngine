using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class PercentageBarTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			bar = new PercentageBar(Center, PercentileColors);
		}

		private PercentageBar bar;
		private static readonly Rectangle Center = new Rectangle(Left, Top, Width, Height);
		private const float Left = 0.3f;
		private const float Top = 0.475f;
		private const float Width = 0.4f;
		private const float Height = 0.05f;
		private static readonly Color LowColor = Color.Red;
		private static readonly Color MidColor = Color.Yellow;
		private static readonly Color HighColor = Color.Green;
		private static readonly Color[] PercentileColors = { LowColor, MidColor, HighColor };

		[Test]
		public void RenderShrinkingLeftAlignedBar()
		{
			bar.Start<Shrink>();
		}

		private class Shrink : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (PercentageBar bar in entities)
					bar.Value -= ShrinkSpeed * Time.Delta;
			}
		}

		private const int ShrinkSpeed = 25;

		[Test]
		public void RenderShrinkingCenterAlignedBar()
		{
			bar.Alignment = PercentageBar.HorizontalAlignment.Center;
			bar.Start<Shrink>();
		}

		[Test, CloseAfterFirstFrame]
		public void DefaultValues()
		{
			Assert.AreEqual(0, bar.Minimum);
			Assert.AreEqual(100, bar.Maximum);
			Assert.AreEqual(100, bar.Value);
			Assert.AreEqual(Center, bar.DrawArea);
			Assert.AreEqual(Width, bar.MaxWidth);
			Assert.AreEqual(HighColor, bar.Color);
			Assert.AreEqual(PercentileColors, bar.PercentileColors);
			Assert.AreEqual(PercentageBar.HorizontalAlignment.Left, bar.Alignment);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeMinimum()
		{
			bar.Minimum = 10;
			Assert.AreEqual(10, bar.Minimum);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeMaximum()
		{
			bar.Maximum = 30;
			Assert.AreEqual(30, bar.Maximum);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangingValueUpdatesDrawArea()
		{
			bar.Value = 60;
			Assert.AreEqual(60, bar.Value);
			Assert.AreEqual(new Rectangle(Left, Top, 0.6f * Width, Height), bar.DrawArea);
		}

		// Have to have this due to a tiny floating point error
		private static void AssertColorsNearlyEqual(Color expectedColor, Color actualColor)
		{
			Assert.AreEqual(expectedColor.R, actualColor.R, 1);
			Assert.AreEqual(expectedColor.G, actualColor.G, 1);
			Assert.AreEqual(expectedColor.B, actualColor.B, 1);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangingValueToLowValueLerpsColorBetweenFirstPercentileColors()
		{
			bar.Value = 30;
			AssertColorsNearlyEqual(LowColor.Lerp(MidColor, 0.6f), bar.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangingValueToHighValueLerpsColorBetweenLastPercentileColors()
		{
			bar.Value = 60;
			AssertColorsNearlyEqual(MidColor.Lerp(HighColor, 0.2f), bar.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void ValueCannotGoBelowMinimum()
		{
			bar.Value = -10;
			Assert.AreEqual(0, bar.Value);
			Assert.AreEqual(0, bar.DrawArea.Width);
			Assert.AreEqual(Color.Red, bar.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void ValueCannotGoAboveMaximum()
		{
			bar.Value = 110;
			Assert.AreEqual(100, bar.Value);
			Assert.AreEqual(Width, bar.DrawArea.Width);
			Assert.AreEqual(Color.Green, bar.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void ValueIncreasesIfBelowNewMinimum()
		{
			bar.Value = 50;
			bar.Minimum = 60;
			Assert.AreEqual(60, bar.Value);
			Assert.AreEqual(Color.Red, bar.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void ValueDecreasesIfAboveNewMaximum()
		{
			bar.Value = 50;
			bar.Maximum = 40;
			Assert.AreEqual(40, bar.Value);
			Assert.AreEqual(Color.Green, bar.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeMaxWidth()
		{
			bar.MaxWidth = 0.6f;
			Assert.AreEqual(0.6f, bar.MaxWidth);
			Assert.AreEqual(0.6f, bar.DrawArea.Width);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeAlignment()
		{
			bar.Alignment = PercentageBar.HorizontalAlignment.Center;
			Assert.AreEqual(PercentageBar.HorizontalAlignment.Center, bar.Alignment);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangePercentileColors()
		{
			bar.Value = 50;
			var colors = new[] { Color.Black, Color.White };
			bar.PercentileColors = colors;
			Assert.AreEqual(colors, bar.PercentileColors);
			AssertColorsNearlyEqual(Color.Gray, bar.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void WhenOnlyOnePercentileColorStaysThatColor()
		{
			bar.PercentileColors = new[] { Color.Blue };
			bar.Value = 75;
			Assert.AreEqual(Color.Blue, bar.Color);
		}
	}
}