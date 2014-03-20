using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	[Ignore]
	internal class RabbitGridTests : TestWithMocksOrVisually
	{
		private static RabbitGrid CreateRabbitGrid()
		{
			var rabbitGrid = new RabbitGrid(20, 10, Rectangle.One);
			rabbitGrid[1, 1] = true;
			rabbitGrid[2, 1] = true;
			rabbitGrid[1, 2] = true;
			rabbitGrid[2, 2] = true;
			return rabbitGrid;
		}

		[Test]
		public void CellHavingTwoNeighborsShallSurvive()
		{
			var rabbitGrid = CreateRabbitGrid();
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(rabbitGrid.ShouldSurvive(1, 1));
		}

		[Test]
		public void CellAliveCausesRabbitToBeVisibleInNextFrame()
		{
			var rabbitGrid = CreateRabbitGrid();
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(rabbitGrid.Rabbits[1,1].IsVisible);
		}

		[Test]
		public void HealthBarShownOnHit()
		{
			var rabbitGrid = CreateRabbitGrid();
			AdvanceTimeAndUpdateEntities();
			rabbitGrid.DoDamage(rabbitGrid.CalculatePositionOfMatrixRabbit(1,1),0.1f, 0.7f);
			Assert.IsTrue(rabbitGrid.Rabbits[1,1].RabbitHealthBar.IsVisible);
		}

		[Test]
		public void DamagingRabbitsTillDeathMakesThemInvisibleAgain()
		{
			var rabbitGrid = CreateRabbitGrid();
			AdvanceTimeAndUpdateEntities();
			rabbitGrid.DoDamage(rabbitGrid.CalculatePositionOfMatrixRabbit(1, 1), 0.1f, 10.0f);
			Assert.IsFalse(rabbitGrid.Rabbits[1,1].IsVisible);
			Assert.IsFalse(rabbitGrid.Rabbits[1,1].RabbitHealthBar.IsVisible);
		}
	}
}