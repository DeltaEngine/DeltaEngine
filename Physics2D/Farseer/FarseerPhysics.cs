using DeltaEngine.Datatypes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace DeltaEngine.Physics2D.Farseer
{
	/// <summary>
	/// The Farseer physics implementation
	/// </summary>
	public class FarseerPhysics : Physics
	{
		public override PhysicsBody CreateCircle(float radius)
		{
			Body circle = BodyFactory.CreateCircle(world, unitConverter.ToSimUnits(radius), 1.0f);
			var body = new FarseerBody(circle) { UnitConverter = unitConverter };
			AddBody(body);
			return body;
		}

		private readonly World world =
			new World(new Vector2(0.0f, DefaultGravity * SimUnitsToDisplayUnits));
		private const float DefaultGravity = 0.982f;
		private const float SimUnitsToDisplayUnits = 100.0f;
		private readonly UnitConverter unitConverter = new UnitConverter(SimUnitsToDisplayUnits);

		public override PhysicsBody CreateRectangle(Size size)
		{
			Body rectangle = BodyFactory.CreateRectangle(world, unitConverter.ToSimUnits(size.Width),
				unitConverter.ToSimUnits(size.Height), 1.0f);
			var body = new FarseerBody(rectangle) { UnitConverter = unitConverter };
			AddBody(body);
			return body;
		}

		public override PhysicsBody CreateEdge(Vector2D startPoint, Vector2D endPoint)
		{
			Body edge = BodyFactory.CreateEdge(world,
				unitConverter.ToSimUnits(unitConverter.Convert(startPoint)),
				unitConverter.ToSimUnits(unitConverter.Convert(endPoint)));
			var body = new FarseerBody(edge) { UnitConverter = unitConverter };
			AddBody(body);
			body.IsStatic = true;
			return body;
		}

		public override PhysicsBody CreateEdge(params Vector2D[] positions)
		{
			var edge = new Body(world);
			Vertices farseerVertices = unitConverter.Convert(positions);
			for (int i = 0; i < farseerVertices.Count - 1; ++i)
				FixtureFactory.AttachEdge(farseerVertices[i], farseerVertices[i + 1], edge);

			var body = new FarseerBody(edge) { UnitConverter = unitConverter };
			AddBody(body);
			return body;
		}

		public override PhysicsBody CreatePolygon(params Vector2D[] positions)
		{
			Vertices polygon = unitConverter.Convert(positions);
			Vector2 centroid = -polygon.GetCentroid();
			polygon.Translate(ref centroid);
			Body body = BodyFactory.CreatePolygon(world, polygon, 1.0f);
			var newBody = new FarseerBody(body) { UnitConverter = unitConverter };
			AddBody(newBody);
			return newBody;
		}

		public override PhysicsJoint CreateFixedAngleJoint(PhysicsBody body, float targetAngle)
		{
			FixedAngleJoint farseerJoint = JointFactory.CreateFixedAngleJoint(world,
				((FarseerBody)body).Body);
			farseerJoint.TargetAngle = targetAngle;
			return new FarseerJoint(farseerJoint, body, body);
		}

		public override PhysicsJoint CreateAngleJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			float targetAngle)
		{
			AngleJoint farseerJoint = JointFactory.CreateAngleJoint(world, ((FarseerBody)bodyA).Body,
				((FarseerBody)bodyB).Body);
			farseerJoint.TargetAngle = targetAngle;
			return new FarseerJoint(farseerJoint, bodyA, bodyB);
		}

		public override PhysicsJoint CreateRevoluteJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			Vector2D anchor)
		{
			RevoluteJoint farseerJoint = JointFactory.CreateRevoluteJoint(world,
				((FarseerBody)bodyA).Body, ((FarseerBody)bodyB).Body, unitConverter.Convert(anchor));
			return new FarseerJoint(farseerJoint, bodyA, bodyB);
		}

		public override PhysicsJoint CreateLineJoint(PhysicsBody bodyA, PhysicsBody bodyB,
			Vector2D axis)
		{
			PrismaticJoint farseerJoint = JointFactory.CreatePrismaticJoint(((FarseerBody)bodyA).Body,
				((FarseerBody)bodyB).Body, ((FarseerBody)bodyB).Body.Position, unitConverter.Convert(axis));
			world.AddJoint(farseerJoint);
			return new FarseerJoint(farseerJoint, bodyA, bodyB);
		}

		protected override void Simulate(float delta)
		{
			world.Step(delta);
		}

		public override Vector2D Gravity
		{
			get { return unitConverter.Convert(world.Gravity); }
			set { world.Gravity = unitConverter.Convert(value); }
		}
	}
}