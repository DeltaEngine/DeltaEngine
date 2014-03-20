namespace DeltaEngine.GameLogic.PathFinding
{
	public class LinkToGraphNode
	{
		public LinkToGraphNode(int idOfTargetNode, int costs)
		{
			TargetNode = idOfTargetNode;
			Costs = costs;
			IsActive = true;
		}

		public int TargetNode { get; private set; }
		public int Costs { get; set; }
		public bool IsActive { get; set; }
	}
}