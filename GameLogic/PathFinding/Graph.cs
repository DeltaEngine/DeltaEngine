using System;
using System.Collections.Generic;

namespace DeltaEngine.GameLogic.PathFinding
{
	/// <summary>
	/// Basis for path finding with A-Star, usually used via LevelGraph for 2D maps.
	/// </summary>
	public class Graph
	{
		public Graph(int numberOfNodes)
		{
			Nodes = new GraphNode[numberOfNodes];
			for (int i = 0; i < NumberOfNodes; i++)
				Nodes[i] = new GraphNode(i);
			AdjacentLinks = new List<LinkToGraphNode>[numberOfNodes];
			for (int i = 0; i < NumberOfNodes; i++)
				AdjacentLinks[i] = new List<LinkToGraphNode>();
		}

		public GraphNode GetNode(int nodeId)
		{
			if (IsInvalidNodeId(nodeId))
				throw new InvalidNodeId(nodeId);
			return Nodes[nodeId];
		}

		protected bool IsInvalidNodeId(int nodeId)
		{
			return nodeId < 0 || nodeId >= Nodes.Length;
		}

		public class InvalidNodeId : Exception
		{
			public InvalidNodeId(int nodeId)
				: base("nodeId: " + nodeId) { }
		}

		public int NumberOfNodes
		{
			get { return Nodes.Length; }
		}

		public GraphNode[] Nodes { get; private set; }
		public List<LinkToGraphNode>[] AdjacentLinks { get; protected set; }

		public virtual void Connect(int idOfNodeA, int idOfNodeB, int costs = 10,
			bool isBidirectionalLink = true)
		{
			if (!IsConnected(idOfNodeA, idOfNodeB))
				AdjacentLinks[idOfNodeA].Add(new LinkToGraphNode(idOfNodeB, costs));
			if (isBidirectionalLink && !IsConnected(idOfNodeB, idOfNodeA))
				AdjacentLinks[idOfNodeB].Add(new LinkToGraphNode(idOfNodeA, costs));
		}

		public bool IsConnected(int idOfNodeA, int idOfNodeB)
		{
			for (int i = 0; i < AdjacentLinks[idOfNodeA].Count; i++)
				if (AdjacentLinks[idOfNodeA][i].TargetNode == idOfNodeB)
					return true;
			return false;
		}
	}
}