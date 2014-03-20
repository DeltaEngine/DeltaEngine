using DeltaEngine.Datatypes;

namespace DeltaEngine.Physics3D
{
	/// <summary>
	/// Contains the results of a raycast check
	/// </summary>
	public interface RaycastResult
	{
		bool Found { get; }
		PhysicsBody PhysicsBody { get; }
		float Fraction { get; }
		Vector3D SurfaceNormal { get; }
	}
}
