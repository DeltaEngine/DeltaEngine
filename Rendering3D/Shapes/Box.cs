using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering3D.Shapes
{
	public class Box : Model
	{
		//ncrunch: no coverage start
		public Box(string modelContentName, Vector3D position)
			: base(modelContentName, position) {}
		//ncrunch: no coverage end

		public Box(ModelData data, Vector3D position)
			: base(data, position) {}

		public Box(ModelData data, Vector3D position, Quaternion orientation)
			: base(data, position, orientation) {}

		public Box(Vector3D size, Color color, Vector3D position)
			: this(new ModelData(new BoxMesh(size, color)), position) {}

		public Box(Vector3D size, Color color)
			: this(new ModelData(new BoxMesh(size, color)), Vector3D.Zero) {}
	}
}
