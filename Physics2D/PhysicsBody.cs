using DeltaEngine.Datatypes;

namespace DeltaEngine.Physics2D
{
	/// <summary>
	/// Represents a body which responds to physics
	/// </summary>
	public interface PhysicsBody
	{
		Vector2D Position { get; set; }
		bool IsStatic { get; set; }
		float Restitution { get; set; }
		float Friction { get; set; }
		float Rotation { get; set; }
		Vector2D LinearVelocity { get; set; }
		Vector2D[] LineVertices { get; }
		void ApplyLinearImpulse(Vector2D impulse);
		void ApplyAngularImpulse(float impulse);
		void ApplyTorque(float torque);
		void Dispose();
	}
}