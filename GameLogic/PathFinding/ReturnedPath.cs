using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;

namespace DeltaEngine.GameLogic.PathFinding
{
	public struct ReturnedPath
	{
		public List<GraphNode> Path;
		public int FinalCost;

		public List<Vector2D> GetListOfCoordinates()
		{
			if (Path != null)
				return Path.Select(node => node.Position).ToList();
			return new List<Vector2D>(); //ncrunch: no coverage
		}
	}
}
