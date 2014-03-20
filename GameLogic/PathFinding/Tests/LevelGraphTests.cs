using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.GameLogic.PathFinding.Tests
{
	public class LevelGraphTests
	{
		/// <summary>
		/// 0 - 1 - 2
		/// |   |   |
		/// 3 - 4 - 5
		/// </summary>
		[SetUp]
		public void CreateGraphAs3X2Grid()
		{
			grid3X2 = new LevelGraph(3, 2);
		}

		private LevelGraph grid3X2;

		[Test]
		public void CheckNodePositions()
		{
			Assert.AreEqual(new Vector2D(0, 0), grid3X2.GetNode(0).Position);
			Assert.AreEqual(new Vector2D(1, 0), grid3X2.GetNode(1).Position);
			Assert.AreEqual(new Vector2D(2, 0), grid3X2.GetNode(2).Position);
			Assert.AreEqual(new Vector2D(0, 1), grid3X2.GetNode(3).Position);
			Assert.AreEqual(new Vector2D(1, 1), grid3X2.GetNode(4).Position);
			Assert.AreEqual(new Vector2D(2, 1), grid3X2.GetNode(5).Position);
		}

		[Test]
		public void CheckForConnectedNodes()
		{
			Assert.IsTrue(grid3X2.IsConnected(0, 1));
			Assert.IsTrue(grid3X2.IsConnected(1, 0));
			Assert.IsTrue(grid3X2.IsConnected(1, 4));
			Assert.IsTrue(grid3X2.IsConnected(2, 1));
			Assert.IsTrue(grid3X2.IsConnected(2, 5));
			Assert.IsTrue(grid3X2.IsConnected(3, 4));
			Assert.IsTrue(grid3X2.IsConnected(4, 5));
		}

		[Test]
		public void CheckForNotConnectedNodes()
		{
			Assert.IsFalse(grid3X2.IsConnected(0, 4));
			Assert.IsFalse(grid3X2.IsConnected(4, 0));
			Assert.IsFalse(grid3X2.IsConnected(1, 3));
			Assert.IsFalse(grid3X2.IsConnected(3, 1));
			Assert.IsFalse(grid3X2.IsConnected(1, 5));
			Assert.IsFalse(grid3X2.IsConnected(5, 1));
			Assert.IsFalse(grid3X2.IsConnected(2, 4));
			Assert.IsFalse(grid3X2.IsConnected(4, 2));
			Assert.IsFalse(grid3X2.IsConnected(0, 5));
			Assert.IsFalse(grid3X2.IsConnected(5, 0));
			Assert.IsFalse(grid3X2.IsConnected(2, 3));
			Assert.IsFalse(grid3X2.IsConnected(3, 2));
		}

		[Test]
		public void CheckForGraphBranches()
		{
			Assert.AreEqual(1, grid3X2.AdjacentLinks[0][0].TargetNode);
			Assert.AreEqual(3, grid3X2.AdjacentLinks[0][1].TargetNode);
			Assert.AreEqual(0, grid3X2.AdjacentLinks[1][0].TargetNode);
			Assert.AreEqual(2, grid3X2.AdjacentLinks[1][1].TargetNode);
			Assert.AreEqual(4, grid3X2.AdjacentLinks[1][2].TargetNode);
			Assert.AreEqual(1, grid3X2.AdjacentLinks[2][0].TargetNode);
			Assert.AreEqual(5, grid3X2.AdjacentLinks[2][1].TargetNode);
		}

		[Test]
		public void SetUnreachableNodeAndCheck()
		{
			const int Index = 0;
			grid3X2.SetUnreachableAndUpdate(Index);
			Assert.IsFalse(grid3X2.AdjacentLinks[1][Index].IsActive);
			Assert.IsFalse(grid3X2.AdjacentLinks[3][Index].IsActive);
			Assert.IsTrue(grid3X2.IsUnreachableNode(0));
		}

		[Test]
		public void MakingNodeReachableEnablesAllConnections()
		{
			const int Index = 0;
			grid3X2.SetUnreachableAndUpdate(Index);
			Assert.IsFalse(grid3X2.AdjacentLinks[1][Index].IsActive);
			grid3X2.SetReachableAndUpdate(Index);
			Assert.IsTrue(grid3X2.AdjacentLinks[1][Index].IsActive);
		}

		[Test]
		public void CheckClosestNodeIndexGivenPosition()
		{
			Assert.AreEqual(0, grid3X2.GetClosestNode(new Vector2D(0.2f, 0.3f)));
			Assert.AreEqual(1, grid3X2.GetClosestNode(new Vector2D(0.6f, 0.0f)));
			Assert.AreEqual(3, grid3X2.GetClosestNode(new Vector2D(0.0f, 1.4f)));
		}

		[Test]
		public void CheckGetNodes()
		{
			Assert.AreEqual(4, grid3X2.GetNode(1, 1).Id);
		}

		[Test]
		public void TestAllowingDiagonalPaths()
		{
			var grid = new MockLevelGraph(3, 2);
			Assert.AreEqual(1, grid.AdjacentLinks[0][0].TargetNode);
			Assert.AreEqual(3, grid.AdjacentLinks[0][1].TargetNode);
			Assert.AreEqual(4, grid.AdjacentLinks[0][2].TargetNode);
			Assert.AreEqual(0, grid.AdjacentLinks[1][0].TargetNode);
			Assert.AreEqual(2, grid.AdjacentLinks[1][1].TargetNode);
			Assert.AreEqual(4, grid.AdjacentLinks[1][2].TargetNode);
			Assert.AreEqual(5, grid.AdjacentLinks[1][3].TargetNode);
			Assert.AreEqual(3, grid.AdjacentLinks[1][4].TargetNode);
			Assert.AreEqual(1, grid.AdjacentLinks[2][0].TargetNode);
			Assert.AreEqual(5, grid.AdjacentLinks[2][1].TargetNode);
			Assert.AreEqual(4, grid.AdjacentLinks[2][2].TargetNode);
		}

		[Test]
		public void CheckUpdateWeightInLinks()
		{
			grid3X2.UpdateWeightInAdjacentNodes(new Vector2D(0, 1), 1, 10);
			Assert.AreEqual(20, grid3X2.AdjacentLinks[1][0].Costs);
			Assert.AreEqual(10, grid3X2.AdjacentLinks[1][1].Costs);
			Assert.AreEqual(20, grid3X2.AdjacentLinks[1][2].Costs);
			grid3X2.ResetGraph();
			foreach(var links in grid3X2.AdjacentLinks)
				foreach (var link in links)
					Assert.IsTrue(link.IsActive);
		}

		public class MockLevelGraph : LevelGraph
		{
			public MockLevelGraph(int columns, int rows)
				: base(columns, rows)
			{
				AreDiagonalPathAllowed = true;
				ConnectAllNodesToLinkGrid();
			}
		}
	}
}