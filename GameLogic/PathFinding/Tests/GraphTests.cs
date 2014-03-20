using NUnit.Framework;

namespace DeltaEngine.GameLogic.PathFinding.Tests
{
	public class GraphTests
	{
		/// <summary>
		///     0
		///     |
		/// 1 - 2 - 3
		///     |
		///     4
		/// </summary>
		[SetUp]
		public void Initialize()
		{
			graphAsPlusShape = new Graph(NumberOfNodesInGraph);
			ConnectGraphNodesBidirectional(2, 1);
			ConnectGraphNodesBidirectional(2, 0);
			ConnectGraphNodesBidirectional(2, 3);
			ConnectGraphNodesBidirectional(2, 4);
		}

		private Graph graphAsPlusShape;
		private const int NumberOfNodesInGraph = 5;

		private void ConnectGraphNodesBidirectional(int aNodeId, int bNodeId)
		{
			graphAsPlusShape.Connect(aNodeId, bNodeId);
		}

		[Test]
		public void ThrowExceptionISearchedForWrongNode()
		{
			Assert.Throws<Graph.InvalidNodeId>(() => graphAsPlusShape.GetNode(-1));
		}

		[Test]
		public void CheckForExistingGraphNodes()
		{
			for (int nodeId = 0; nodeId < NumberOfNodesInGraph; nodeId++)
				Assert.IsNotNull(graphAsPlusShape.GetNode(nodeId));
		}

		[Test]
		public void CheckForConnectedNodes()
		{
			Assert.IsTrue(graphAsPlusShape.IsConnected(2, 0));
			Assert.IsTrue(graphAsPlusShape.IsConnected(0, 2));
			Assert.IsTrue(graphAsPlusShape.IsConnected(2, 1));
			Assert.IsTrue(graphAsPlusShape.IsConnected(1, 2));
			Assert.IsTrue(graphAsPlusShape.IsConnected(2, 3));
			Assert.IsTrue(graphAsPlusShape.IsConnected(3, 2));
			Assert.IsTrue(graphAsPlusShape.IsConnected(2, 4));
			Assert.IsTrue(graphAsPlusShape.IsConnected(4, 2));
		}

		[Test]
		public void CheckForNotConnectedNodes()
		{
			Assert.IsFalse(graphAsPlusShape.IsConnected(1, 0));
			Assert.IsFalse(graphAsPlusShape.IsConnected(0, 1));
			Assert.IsFalse(graphAsPlusShape.IsConnected(3, 0));
			Assert.IsFalse(graphAsPlusShape.IsConnected(0, 3));
			Assert.IsFalse(graphAsPlusShape.IsConnected(3, 4));
			Assert.IsFalse(graphAsPlusShape.IsConnected(4, 3));
			Assert.IsFalse(graphAsPlusShape.IsConnected(1, 4));
			Assert.IsFalse(graphAsPlusShape.IsConnected(4, 1));
			Assert.IsFalse(graphAsPlusShape.IsConnected(0, 4));
			Assert.IsFalse(graphAsPlusShape.IsConnected(4, 0));
			Assert.IsFalse(graphAsPlusShape.IsConnected(1, 3));
			Assert.IsFalse(graphAsPlusShape.IsConnected(3, 1));
			Assert.IsFalse(graphAsPlusShape.IsConnected(2, 2));
		}

		[Test]
		public void CheckAdjacentLinks()
		{
			Assert.AreEqual(4, graphAsPlusShape.AdjacentLinks[2].Count);
			Assert.AreEqual(1, graphAsPlusShape.AdjacentLinks[0].Count);
			Assert.AreEqual(1, graphAsPlusShape.AdjacentLinks[1].Count);
			Assert.AreEqual(1, graphAsPlusShape.AdjacentLinks[3].Count);
			Assert.AreEqual(1, graphAsPlusShape.AdjacentLinks[4].Count);
		}
	}
}