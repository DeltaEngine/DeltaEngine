using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering3D.Shapes
{
	public class Circle3D : Entity3D
	{
		public Circle3D(Vector3D center, float radius, Color color)
			: base(center)
		{
			Add(color);
			Add(radius);
			OnDraw<Circle3DRenderer>();
		}

		public Vector3D Center
		{
			get { return Get<Vector3D>(); }
			set { Set(value); }
		}

		public float Radius
		{
			get { return Get<float>(); }
			set { Set(value); }
		}
	}
}
