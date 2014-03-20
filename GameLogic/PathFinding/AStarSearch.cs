using System;

namespace DeltaEngine.GameLogic.PathFinding
{
	/// <summary>
	/// Algorithm for the graph path finding
	/// </summary>
	public class AStarSearch : GraphSearch
	{
		private const float HeuristicWeight = 100.0f;

		public override bool Search(Graph graphToSearch, int start, int end)
		{
			graph = graphToSearch;
			startNode = start;
			targetNode = end;
			Initialize();
			while (nodesToCheck.Count > 0)
			{
				int currentNode = GetNextNode();
				visitedNodes++;
				if (currentNode == InvalidNodeIndex)
					return false;
				if (currentNode == targetNode)
					return true;
				CheckAdjacency(currentNode);
			}
			return false;
		}

		private void CheckAdjacency(int currentNode)
		{
			for (int connection = 0; connection < graph.AdjacentLinks[currentNode].Count; connection++)
			{
				LinkToGraphNode currentNodeConnection = graph.AdjacentLinks[currentNode][connection];
				if (!currentNodeConnection.IsActive)
					continue;
				int connectedNode = currentNodeConnection.TargetNode;
				int costToThisNode = costSoFar[currentNode] + currentNodeConnection.Costs +
					EstimateDistance(connectedNode, targetNode);
				if (costSoFar[connectedNode] < costToThisNode)
					continue;
				costSoFar[connectedNode] = costToThisNode;
				previousNode[connectedNode] = currentNode;
			}
		}

		private int EstimateDistance(int nodeFrom, int nodeTo)
		{
			return (int)((Math.Abs(graph.Nodes[nodeTo].Position.X - graph.Nodes[nodeFrom].Position.X) +
				Math.Abs(graph.Nodes[nodeTo].Position.Y - graph.Nodes[nodeFrom].Position.Y)) * HeuristicWeight);
		}
	}
}
