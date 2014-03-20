using DeltaEngine.Datatypes;
using DeltaEngine.Physics2D.Farseer;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	public class AffixToPhysicsTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			physics = new FarseerPhysics();
		}

		private Physics physics;

		[Test, Timeout(5000)]
		public void FallingWhiteCircle()
		{
			CreateFloor();
			CreateCircle();
		}

		private void CreateFloor()
		{
			var startPoint = new Vector2D(0.0f, 0.7f);
			var endPoint = new Vector2D(1.0f, 0.65f);
			new Line2D(startPoint, endPoint, Color.Blue);
			physics.CreateEdge(startPoint, endPoint);
		}

		private void CreateCircle()
		{
			PhysicsBody physicsCircle = physics.CreateCircle(Radius);
			physicsCircle.Position = Center;
			physicsCircle.LinearVelocity = new Vector2D(0.1f, 0.0f);
			physicsCircle.Restitution = 0.85f;
			physicsCircle.Friction = 0.9f;
			new Ellipse(Center, Radius, Radius, Color.White).AffixToPhysics(physicsCircle);
		}

		private const float Radius = 0.02f;
		private static readonly Vector2D Center = Vector2D.Half;
	}
}