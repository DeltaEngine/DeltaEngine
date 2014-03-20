using System;
using System.Collections.Generic;

namespace DeltaEngine.GameLogic.PathFinding
{
	/// <summary>
	/// Base class for path finding, currently only AStarSearch is supported and used.
	/// </summary>
	public abstract class GraphSearch
	{
		public abstract bool Search(Graph graph, int startNode, int targetNode);

		public ReturnedPath GetPath()
		{
			var shortestPath = new Stack<int>();
			int currentNode = targetNode;
			while (currentNode != startNode)
			{
				shortestPath.Push(currentNode);
				currentNode = previousNode[currentNode];
			}
			return new ReturnedPath
			{
				Path = CalculateListToReturn(shortestPath, currentNode),
				FinalCost = costSoFar[targetNode]
			};
		}

		protected int startNode;
		protected int targetNode;
		protected int[] previousNode;

		private List<GraphNode> CalculateListToReturn(Stack<int> shortestPath, int currentNode)
		{
			shortestPath.Push(currentNode);
			var list = new List<GraphNode>();
			foreach (var nodeIndex in shortestPath.ToArray())
				list.Add(graph.Nodes[nodeIndex]);
			return list;
		}

		protected Graph graph;

		protected void Initialize()
		{
			if (graph.NumberOfNodes == 0)
				return;
			costSoFar = new int[graph.NumberOfNodes];
			previousNode = new int[graph.NumberOfNodes];
			nodesToCheck.Clear();
			visitedNodes = 0;
			for (int i = 0; i < graph.NumberOfNodes; i++)
			{
				costSoFar[i] = Infinity;
				previousNode[i] = InvalidNodeIndex;
				nodesToCheck.Add(i);
			}
			costSoFar[startNode] = 0;
		}

		protected int[] costSoFar;
		protected List<int> nodesToCheck = new List<int>();
		protected const int Infinity = Int32.MaxValue;
		protected const int InvalidNodeIndex = -1;
		protected int visitedNodes;

		protected int GetNextNode()
		{
			int minCost = Infinity;
			int nextNode = InvalidNodeIndex;
			foreach (int t in nodesToCheck)
				if (costSoFar[t] < minCost)
				{
					minCost = costSoFar[t];
					nextNode = t;
				}
			nodesToCheck.Remove(nextNode);
			return nextNode;
		}
	}
}
