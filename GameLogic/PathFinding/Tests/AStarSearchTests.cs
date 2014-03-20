using NUnit.Framework;

namespace DeltaEngine.GameLogic.PathFinding.Tests
{
	public class AStarSearchTests
	{
		/// <summary>
		/// 0 - 1 - 3
		/// |   |
		/// 2   4 - 5
		/// </summary>
		[SetUp]
		public void Initialize()
		{
			aStar = new AStarSearch();
			graph = new Graph(6);
			graph.Connect(0, 1);
			graph.Connect(0, 2);
			graph.Connect(1, 3);
			graph.Connect(1, 4);
			graph.Connect(4, 5, 1, false);
			graph.AdjacentLinks[0][0].IsActive = false;
		}

		private Graph graph;
		private AStarSearch aStar;

		[Test]
		public void SearchForPathBetweenNodes()
		{
			Assert.IsTrue(aStar.Search(graph, 0, 2));
			Assert.IsTrue(aStar.Search(graph, 1, 5));
			Assert.IsFalse(aStar.Search(graph, 5, 2));
		}

		[Test]
		public void GraphWithNoNodesHasNoPath()
		{
			Assert.IsFalse(aStar.Search(new Graph(0), 0, 0));
		}

		[Test]
		public void CheckPathList()
		{
			aStar.Search(graph, 0, 2);
			Assert.AreEqual(2, aStar.GetPath().GetListOfCoordinates().Count);
			aStar.Search(graph, 1, 5);
			Assert.AreEqual(3, aStar.GetPath().GetListOfCoordinates().Count);
		}
	}
}