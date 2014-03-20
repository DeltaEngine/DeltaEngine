using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace DeltaEngine.Physics2D.Farseer
{
	/// <summary>
	/// Implements the Farseer flavor of physics bodies
	/// </summary>
	internal class FarseerBody : PhysicsBody
	{
		public FarseerBody(Body body)
		{
			Body = body;
			Body.BodyType = BodyType.Dynamic;
		}

		public Body Body { get; private set; }
		internal UnitConverter UnitConverter { get; set; }

		public Vector2D Position
		{
			get { return UnitConverter.Convert(UnitConverter.ToDisplayUnits(Body.Position)); }
			set
			{
				Vector2 position = UnitConverter.Convert(value);
				Body.Position = UnitConverter.ToSimUnits(position);
			}
		}

		public bool IsStatic
		{
			get { return Body.IsStatic; }
			set { Body.IsStatic = value; }
		}

		public float Restitution
		{
			get { return Body.Restitution; }
			set { Body.Restitution = value; }
		}

		public float Friction
		{
			get { return Body.Friction; }
			set { Body.Friction = value; }
		}

		public float Rotation
		{
			get { return Body.Rotation.RadiansToDegrees(); }
			set { Body.Rotation = value.DegreesToRadians(); }
		}

		public Vector2D LinearVelocity
		{
			get { return UnitConverter.Convert(UnitConverter.ToDisplayUnits(Body.LinearVelocity)); }
			set
			{
				Vector2 velocity = UnitConverter.Convert(value);
				Body.LinearVelocity = UnitConverter.ToSimUnits(velocity);
			}
		}

		public void ApplyLinearImpulse(Vector2D impulse)
		{
			Vector2 fImpulse = UnitConverter.Convert(impulse);
			Body.ApplyLinearImpulse(ref fImpulse);
		}

		public void ApplyAngularImpulse(float impulse)
		{
			Body.ApplyAngularImpulse(impulse);
		}

		public void ApplyTorque(float torque)
		{
			Body.ApplyTorque(torque);
		}

		public Vector2D[] LineVertices
		{
			get
			{
				Transform xf;
				Body.GetTransform(out xf);
				var vertices = new List<Vector2D>();
				foreach (var fixture in Body.FixtureList)
					vertices.AddRange(GetShapeVerticesFromFixture(fixture, xf));
				return vertices.ToArray();
			}
		}

		private IEnumerable<Vector2D> GetShapeVerticesFromFixture(Fixture fixture, Transform xf)
		{
			var shape = fixture.Shape;
			switch (shape.ShapeType)
			{
			case ShapeType.Polygon:
				return GetPolygonShapeVertices(shape as PolygonShape, xf);
			case ShapeType.Edge:
				return GetEdgeShapeVertices(shape as EdgeShape, xf);
			case ShapeType.Circle:
				return GetCircleShapeVertices(shape as CircleShape, xf);
			}
			//This will never be reached, PhysicsBody internally will not allow it.
			return new List<Vector2D>(); //ncrunch: no coverage
		}

		private IEnumerable<Vector2D> GetPolygonShapeVertices(PolygonShape polygon, Transform xf)
		{
			int vertexCount = polygon.Vertices.Count;
			var tempVertices = new Vector2[vertexCount];
			for (int i = 0; i < vertexCount; ++i)
				tempVertices[i] = MathUtils.Mul(ref xf, polygon.Vertices[i]);
			return GetDrawVertices(tempVertices, vertexCount);
		}

		private IEnumerable<Vector2D> GetDrawVertices(Vector2[] vertices, int vertexCount)
		{
			var drawVertices = new List<Vector2D>();
			for (int i = 0; i < (vertexCount - 1); i++)
			{
				drawVertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[i])));
				drawVertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[i + 1])));
			}
			drawVertices.Add(
				UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[vertexCount - 1])));
			drawVertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(vertices[0])));
			return drawVertices.ToArray();
		}

		private IEnumerable<Vector2D> GetEdgeShapeVertices(EdgeShape edge, Transform xf)
		{
			Vector2 v1 = MathUtils.Mul(ref xf, edge.Vertex1);
			Vector2 v2 = MathUtils.Mul(ref xf, edge.Vertex2);

			return new[]
			{
				UnitConverter.Convert(UnitConverter.ToDisplayUnits(v1)),
				UnitConverter.Convert(UnitConverter.ToDisplayUnits(v2))
			};
		}

		private IEnumerable<Vector2D> GetCircleShapeVertices(CircleShape circle, Transform xf)
		{
			CircleData circleData = CreateCircleData(circle, xf);
			return CreateCircleVertexArray(circleData);
		}

		private static CircleData CreateCircleData(CircleShape circle, Transform xf)
		{
			var circleData = new CircleData
			{
				center = MathUtils.Mul(ref xf, circle.Position),
				radius = circle.Radius,
				segments = CircleSegments,
				increment = Math.PI * 2.0 / 32,
				theta = 0.0
			};
			return circleData;
		}

		private const int CircleSegments = 32;

		private struct CircleData
		{
			internal Vector2 center;
			internal float radius;
			internal int segments;
			internal double increment;
			internal double theta;
		}

		private Vector2D[] CreateCircleVertexArray(CircleData circleData)
		{
			var vertices = new List<Vector2D>();
			for (int i = 0; i < circleData.segments; i++)
			{
				Vector2 v1 = CreateCircleVertexVectorV1(circleData);
				Vector2 v2 = CreateCircleVertexVectorV2(circleData);
				vertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(v1)));
				vertices.Add(UnitConverter.Convert(UnitConverter.ToDisplayUnits(v2)));
				circleData.theta += circleData.increment;
			}
			return vertices.ToArray();
		}

		private static Vector2 CreateCircleVertexVectorV1(CircleData circleData)
		{
			return circleData.center +
				circleData.radius *
					new Vector2((float)Math.Cos(circleData.theta), (float)Math.Sin(circleData.theta));
		}

		private static Vector2 CreateCircleVertexVectorV2(CircleData circleData)
		{
			return circleData.center +
				circleData.radius *
					new Vector2((float)Math.Cos(circleData.theta + circleData.increment),
						(float)Math.Sin(circleData.theta + circleData.increment));
		}

		public void Dispose()
		{
			Body.Dispose();
		}
	}
}