using DeltaEngine.Datatypes;

namespace DeltaEngine.GameLogic.PathFinding
{
	/// <summary>
	/// 2D path finding graph for levels using columns (x) and rows (y) for indexing.
	/// </summary>
	public class LevelGraph : Graph
	{
		public LevelGraph(int columns, int rows)
			: base(columns * rows)
		{
			this.columns = columns;
			this.rows = rows;
			unreachableNodes = new bool[NumberOfNodes];
			for (int y = 0; y < rows; y++)
				for (int x = 0; x < columns; x++)
					Nodes[x + y * columns].Position = new Vector2D(x, y);
			ConnectAllNodesToLinkGrid();
		}

		private readonly int columns;
		private readonly int rows;
		private readonly bool[] unreachableNodes;

		public GraphNode GetNode(int x, int y)
		{
			return GetNode(GetNodeId(x, y));
		}

		private int GetNodeId(int x, int y)
		{
			return x + y * columns;
		}

		protected void ConnectAllNodesToLinkGrid()
		{
			for (int y = 0; y < rows; y++)
				for (int x = 0; x < columns; x++)
					ConnectNodes(x, y);
		}

		private void ConnectNodes(int x, int y)
		{
			int nodeId = GetNodeId(x, y);
			if (IsInvalidNodeId(nodeId) || unreachableNodes[nodeId])
				return; //ncrunch: no coverage
			ConnectAdjacentNode(nodeId, x - 1, y);
			ConnectAdjacentNode(nodeId, x + 1, y);
			ConnectAdjacentNode(nodeId, x, y - 1);
			ConnectAdjacentNode(nodeId, x, y + 1);
			if (AreDiagonalPathAllowed)
				ConnectDiagonalNodes(x, y, nodeId);
		}

		public bool AreDiagonalPathAllowed;

		private void ConnectAdjacentNode(int nodeId, int neighborX, int neighborY)
		{
			if (IsValidGridPosition(neighborX, neighborY))
				Connect(nodeId, GetNodeId(neighborX, neighborY));
		}

		private bool IsValidGridPosition(int columnIndex, int rowIndex)
		{
			return rowIndex >= 0 && rowIndex < rows && columnIndex >= 0 && columnIndex < columns;
		}

		public override void Connect(int idOfNodeA, int idOfNodeB, int costs = 10,
			bool isBidirectionalLink = true)
		{
			if (IsInvalidNodeId(idOfNodeB) || unreachableNodes[idOfNodeB])
				return; //ncrunch: no coverage
			base.Connect(idOfNodeA, idOfNodeB, costs, isBidirectionalLink);
		}

		private void ConnectDiagonalNodes(int x, int y, int nodeId)
		{
			ConnectAdjacentNode(nodeId, x + 1, y + 1);
			ConnectAdjacentNode(nodeId, x + 1, y - 1);
			ConnectAdjacentNode(nodeId, x - 1, y - 1);
			ConnectAdjacentNode(nodeId, x - 1, y + 1);
		}

		public void SetUnreachableAndUpdate(int index)
		{
			SetReachableState(index, false);
			SetUnreachableNode(index);
		}

		private void SetReachableState(int index, bool state)
		{
			foreach (var adjacency in AdjacentLinks[index])
				foreach (var branch in AdjacentLinks[adjacency.TargetNode])
					if (branch.TargetNode == index)
					{
						branch.IsActive = state;
						break;
					}
		}

		public void SetUnreachableNode(int nodeIndex)
		{
			unreachableNodes[nodeIndex] = true;
		}

		public void UpdateWeightInAdjacentNodes(Vector2D position, int range, int weight)
		{
			var minRow = (int)(position.Y - range < 0 ? 0 : position.Y - range);
			var maxRow = position.Y + range >= rows ? rows - 1 : position.Y + range;
			var minColumn = (int)(position.X - range < 0 ? 0 : position.X - range);
			var maxColumn = position.X + range >= columns ? columns - 1 : position.X + range;
			for (int i = minColumn; i <= maxColumn; i++)
				for (int j = minRow; j <= maxRow; j++)
					UpdateLinkWeightToNode(GetNodeId(i, j), weight);
		}

		private void UpdateLinkWeightToNode(int nodeId, int weight)
		{
			foreach (var adjacency in AdjacentLinks[nodeId])
				foreach (var branch in AdjacentLinks[adjacency.TargetNode])
					if (branch.TargetNode == nodeId)
					{
						branch.Costs += weight;
						break;
					}
		}

		public void SetReachableAndUpdate(int index)
		{
			SetReachableState(index, true);
			unreachableNodes[index] = false;
		}

		public int GetClosestNode(Vector3D position)
		{
			var minimumDistance = (Nodes[0].Position - position).LengthSquared;
			int index = 0;
			for (int i = 1; i < Nodes.Length; i++)
			{
				var distanceSquared = (Nodes[i].Position - position).LengthSquared;
				if (distanceSquared >= minimumDistance)
					continue;
				minimumDistance = distanceSquared;
				index = i;
			}
			return index;
		}

		public bool IsUnreachableNode(int nodeIndex)
		{
			return unreachableNodes[nodeIndex];
		}

		public void ResetGraph()
		{
			for (int i = 0; i < NumberOfNodes; i++)
				SetReachableAndUpdate(i);
		}
	}
}