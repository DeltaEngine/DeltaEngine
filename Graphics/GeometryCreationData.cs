using DeltaEngine.Content;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics
{
	public class GeometryCreationData : ContentCreationData
	{
		public GeometryCreationData(VertexFormat format, int numberOfVertices, int numberOfIndices)
		{
			Format = format;
			NumberOfVertices = numberOfVertices;
			NumberOfIndices = numberOfIndices;
		}

		public VertexFormat Format { get; private set; }
		public int NumberOfVertices { get; private set; }
		public int NumberOfIndices { get; private set; }
	}
}