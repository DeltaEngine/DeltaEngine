using DeltaEngine.Datatypes;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace DeltaEngine.Physics3D.Jitter
{
	/// <summary>
	/// Contains the results of a raycast check
	/// </summary>
	public class JitterRaycastResult : RaycastResult
	{
		public JitterRaycastResult(CollisionSystem collisionSystem, Ray ray)
		{
			float fraction;
			JVector rayOrigin = JitterDatatypesMapping.Convert(ref ray.Origin);
			JVector rayDirection = JitterDatatypesMapping.Convert(ref ray.Direction);
			Found = collisionSystem.Raycast(rayOrigin, rayDirection, null, out RigidBody,
				out JVectorNormal, out fraction);
			Vector3D surfaceNormal = Vector3D.Zero;
			JitterDatatypesMapping.Convert(ref JVectorNormal, ref surfaceNormal);
			SurfaceNormal = surfaceNormal;
		}

		public bool Found { get; private set; }
		public readonly RigidBody RigidBody;
		public readonly JVector JVectorNormal;
		public float Fraction { get; private set; }
		public Vector3D SurfaceNormal  { get; private set; }
		public PhysicsBody PhysicsBody { get; set; }
	}
}
