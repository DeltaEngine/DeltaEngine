using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace Drench.Tests
{
	public class FloodFillerTests
	{
		[SetUp]
		public void SetUp()
		{
			colors = new Color[Width,Height];
			floodFiller = new FloodFiller(colors);
		}

		private Color[,] colors;
		private const int Width = 4;
		private const int Height = 4;
		private FloodFiller floodFiller;

		[Test]
		public void SettingColorSetsColorAtPosition()
		{
			floodFiller.SetColor(1, 1, Color.Red);
			Assert.AreEqual(Color.Red, colors[1, 1]);
		}

		[Test]
		public void SettingColorToSameColorLeavesItUnchanged()
		{
			floodFiller.SetColor(1, 1, Color.Red);
			floodFiller.SetColor(1, 1, Color.Red);
			Assert.AreEqual(Color.Red, colors[1, 1]);
			Assert.AreEqual(0, floodFiller.ProcessedCount);
		}

		[Test]
		public void SettingColorWhenAllColorsAreTheSameChangesThemAll()
		{
			floodFiller.SetColor(1, 1, Color.Red);
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					Assert.AreEqual(Color.Red, colors[x, y]);
			Assert.AreEqual(16, floodFiller.ProcessedCount);
		}

		[Test]
		public void SettingColorWhenBoardIsSplitChangesOnlyPart()
		{
			SetFirstThreeColumnsToBlue();
			floodFiller.SetColor(1, 1, Color.Red);
			AssertFirstThreeColumnsAreRed();
			AssertRestOfTheColumnsAreBlack();
			Assert.AreEqual(12, floodFiller.ProcessedCount);
		}

		private void SetFirstThreeColumnsToBlue()
		{
			for (int x = 0; x < 3; x++)
				for (int y = 0; y < Height; y++)
					colors[x, y] = Color.Blue;
		}

		private void AssertFirstThreeColumnsAreRed()
		{
			for (int x = 0; x < 3; x++)
				for (int y = 0; y < Height; y++)
					Assert.AreEqual(Color.Red, colors[x, y]);
		}

		private void AssertRestOfTheColumnsAreBlack()
		{
			for (int x = 3; x < Width; x++)
				for (int y = 0; y < Height; y++)
					Assert.AreEqual(Color.TransparentBlack, colors[x, y]);
		}
	}
}