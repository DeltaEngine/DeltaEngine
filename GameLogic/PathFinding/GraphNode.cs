using DeltaEngine.Datatypes;

namespace DeltaEngine.GameLogic.PathFinding
{
	public struct GraphNode
	{
		public GraphNode(int id)
			: this()
		{
			Id = id;
		}

		public int Id { get; private set; }
		public Vector2D Position;
	}
}