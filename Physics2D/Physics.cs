using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Physics2D
{
	/// <summary>
	/// Main physics class used for performing simulations and creating bodies and shapes.
	/// </summary>
	public abstract class Physics : Entity, RapidUpdateable
	{
		public abstract PhysicsBody CreateCircle(float radius);
		public abstract PhysicsBody CreateRectangle(Size size);
		public abstract PhysicsBody CreateEdge(Vector2D startPoint, Vector2D endPoint);
		public abstract PhysicsBody CreateEdge(params Vector2D[] positions);
		public abstract PhysicsBody CreatePolygon(params Vector2D[] positions);
		public abstract PhysicsJoint CreateFixedAngleJoint(PhysicsBody body, float targetAngle);

		public abstract PhysicsJoint CreateAngleJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			float targetAngle);

		public abstract PhysicsJoint CreateRevoluteJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			Vector2D anchor);

		public abstract PhysicsJoint CreateLineJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			Vector2D axis);

		public abstract Vector2D Gravity { get; set; }

		public void RapidUpdate()
		{
			Simulate(Time.RapidUpdateDelta);
		}

		protected abstract void Simulate(float delta);

		protected void AddBody(PhysicsBody body)
		{
			bodies.Add(body);
		}

		private readonly List<PhysicsBody> bodies = new List<PhysicsBody>();

		public IEnumerable<PhysicsBody> Bodies
		{
			get { return bodies; }
		}
	}
}