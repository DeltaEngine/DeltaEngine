using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering3D
{
	/// <summary>
	/// A quad mesh that always has a fixed orientation with respect to the camera
	/// </summary>
	public class Billboard : Entity3D
	{
		public Billboard(Vector3D position, Size size, Material material,
			BillboardMode billboardMode = BillboardMode.CameraFacing)
			: base(position)
		{
			mode = billboardMode;
			planeQuad = new PlaneQuad(size, material);
			OnDraw<BillboardRenderer>();
		}

		internal readonly BillboardMode mode;
		public readonly PlaneQuad planeQuad;
	}
}