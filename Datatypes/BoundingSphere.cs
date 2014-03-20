using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Contains the center position in 3D and radius. Allows quick collision and intersection tests.
	/// </summary>
	[DebuggerDisplay("BoundingSphere(Center={Center}, Radius={Radius})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct BoundingSphere
	{
		public BoundingSphere(Vector3D center, float radius)
			: this()
		{
			Center = center;
			Radius = radius;
		}

		public Vector3D Center { get; set; }
		public float Radius { get; set; }

		public bool IsColliding(BoundingSphere other)
		{
			float combinedRadii = Radius + other.Radius;
			return Center.DistanceSquared(other.Center) < combinedRadii * combinedRadii;
		}
	}
}