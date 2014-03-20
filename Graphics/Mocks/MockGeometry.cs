using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics.Mocks
{
	/// <summary>
	/// Mock geometry used in unit tests.
	/// </summary>
	public class MockGeometry : Geometry
	{
		//ncrunch: no coverage start
		public MockGeometry(string contentName)
			: base(contentName) {}

		protected MockGeometry(GeometryCreationData creationData)
			: base(creationData) {}

		protected override void LoadData(Stream fileData) {}

		public override void Draw() {}

		protected override void SetNativeData(byte[] vertexData, short[] indices)
		{
			var verticesStream = new MemoryStream(vertexData);
			vertexReader = new BinaryReader(verticesStream);
			cachedIndices = indices;
		}

		private BinaryReader vertexReader;
		private short[] cachedIndices;

		protected override void DisposeData()
		{
			if (vertexReader != null)
				vertexReader.Dispose();
		}

		public Vector3D GetVertexPosition(int vertexIndex)
		{
			vertexReader.BaseStream.Position = cachedIndices[vertexIndex] * Format.Stride;
			float x = vertexReader.ReadSingle();
			float y = vertexReader.ReadSingle();
			float z = vertexReader.ReadSingle();
			return new Vector3D(x, y, z);
		}
	}
}