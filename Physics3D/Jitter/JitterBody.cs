using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace DeltaEngine.Physics3D.Jitter
{
	/// <summary>
	/// Jitter body implementation supporting all 3D physics shapes.
	/// </summary>
	internal sealed class JitterBody : PhysicsBody
	{
		internal JitterBody(RigidBody jitterBody)
			: base(null)
		{
			this.jitterBody = jitterBody;
			this.jitterBody.Tag = this;
			jitterShape = jitterBody.Shape;
		}

		private readonly RigidBody jitterBody;
		private Shape jitterShape;

		public JitterBody(JitterPhysics physicsManager, PhysicsShape shape, Vector3D initialPosition,
			float mass, float restitution)
			: base(shape)
		{
			CreateShape();
			RotationMatrix = Matrix.Identity;
			jitterBody = new RigidBody(jitterShape)
			{
				IsStatic = false,
				Mass = mass,
				Material = { Restitution = restitution },
				Position = JitterDatatypesMapping.Convert(ref initialPosition),
				Tag = this
			};
			physicsManager.jitterWorld.AddBody(jitterBody);
		}

		public override Matrix RotationMatrix
		{
			get
			{
				JitterDatatypesMapping.Convert(jitterBody.Orientation, ref rotationMatrix);
				return rotationMatrix;
			}
			set
			{
				if (jitterBody != null)
					jitterBody.Orientation = JitterDatatypesMapping.Convert(ref value);
			}
		}

		private Matrix rotationMatrix = Matrix.Identity;

		private void CreateShape()
		{
			if (Shape.ShapeType == ShapeType.Sphere)
				CreateSphereShape();
			if (Shape.ShapeType == ShapeType.Box)
				CreateBoxShape();
			if (Shape.ShapeType == ShapeType.Capsule)
				CreateCapsuleShape();
			if (Shape.ShapeType == ShapeType.Cone)
				CreateConeShape();
			if (Shape.ShapeType == ShapeType.Cylinder)
				CreateCylinderShape();
			if (Shape.ShapeType == ShapeType.Mesh)
				CreateMeshShape();
			if (Shape.ShapeType == ShapeType.Terrain)
				CreateTerrainShape();
		}

		private void CreateSphereShape()
		{
			jitterShape = new SphereShape(Shape.Radius);
		}

		private void CreateBoxShape()
		{
			Vector3D size = Shape.Size;
			jitterShape = new BoxShape(size.Y, size.Z, size.X);
		}

		private void CreateCapsuleShape()
		{
			jitterShape = new CapsuleShape(Shape.Depth, Shape.Radius);
		}

		private void CreateConeShape()
		{
			jitterShape = new ConeShape(Shape.Height, Shape.Radius);
		}

		private void CreateCylinderShape()
		{
			jitterShape = new CylinderShape(Shape.Height, Shape.Radius);
		}

		private void CreateMeshShape()
		{
			jitterShape = new TriangleMeshShape(
				new Octree(new List<JVector>(), new List<TriangleVertexIndices>()));
		}

		private void CreateTerrainShape()
		{
			jitterShape = new TerrainShape(
				ArrayExtensions.GetWithDefault<PhysicsShape.PropertyType, float[,]>(Shape.Properties,
					PhysicsShape.PropertyType.Height),
				ArrayExtensions.GetWithDefault<PhysicsShape.PropertyType, float>(Shape.Properties,
					PhysicsShape.PropertyType.ScaleX),
				ArrayExtensions.GetWithDefault<PhysicsShape.PropertyType, float>(Shape.Properties,
					PhysicsShape.PropertyType.ScaleY));
		}

		internal void FireCollisionBegin(PhysicsBody other)
		{
			OnCollisionBegin(other);
		}

		internal void FireCollisionEnd(PhysicsBody other)
		{
			OnCollisionEnd(other);
		}

		protected override void SetIsStatic(bool value)
		{
			jitterBody.IsStatic = value;
		}

		protected override void SetIsActive(bool value)
		{
			jitterBody.IsActive = value;
		}

		protected override void SetFriction(float value)
		{
			Body.Material.KineticFriction = value;
			if (jitterBody.IsStatic)
				jitterBody.Material.StaticFriction = value;
		}

		public RigidBody Body
		{
			get { return jitterBody; }
		}

		public override Vector3D Position
		{
			get { return JitterDatatypesMapping.Convert(jitterBody.Position); }
			set { jitterBody.Position = JitterDatatypesMapping.Convert(ref value); }
		}

		public override Vector2D Position2D
		{
			get { return new Vector2D(jitterBody.Position.X, jitterBody.Position.Y); }
		}

		public override Vector3D LinearVelocity
		{
			get { return JitterDatatypesMapping.Convert(jitterBody.LinearVelocity); }
			set { jitterBody.LinearVelocity = JitterDatatypesMapping.Convert(ref value); }
		}

		public override Vector3D AngularVelocity
		{
			get { return JitterDatatypesMapping.Convert(jitterBody.AngularVelocity); }
			set { jitterBody.AngularVelocity = JitterDatatypesMapping.Convert(ref value); }
		}

		public override float AngularVelocity2D
		{
			get { return jitterBody.AngularVelocity.X; }
			set { jitterBody.AngularVelocity = new JVector(value, 0f, 0f); }
		}

		public override float Mass
		{
			get { return base.Mass; }
			set
			{
				jitterBody.Mass = value;
				base.Mass = value;
			}
		}

		public override float Restitution
		{
			get { return base.Restitution; }
			set
			{
				jitterBody.Material.Restitution = value;
				base.Restitution = value;
			}
		}

		public override BoundingBox BoundingBox
		{
			get
			{
				return new BoundingBox(
					JitterDatatypesMapping.Convert(jitterBody.BoundingBox.Min),
					JitterDatatypesMapping.Convert(jitterBody.BoundingBox.Max));
			}
		}

		/// <summary>
		/// Applies a force at the center of mass.
		/// </summary>
		public override void ApplyForce(Vector3D force)
		{
			jitterBody.AddForce(JitterDatatypesMapping.Convert(ref force));
		}

		/// <summary>
		/// Apply a force at a world point. If the force is not applied at the center of mass, it will
		/// generate a torque and affect the angular velocity. This wakes up the body.
		/// </summary>
		public override void ApplyForce(Vector3D force, Vector3D forcePosition)
		{
			jitterBody.AddForce(JitterDatatypesMapping.Convert(ref force),
				JitterDatatypesMapping.Convert(ref forcePosition));
		}

		/// <summary>
		/// Apply a torque. This affects the angular velocity without affecting the linear velocity of
		/// the center of mass. It wakes up the body.
		/// </summary>
		public override void ApplyTorque(Vector3D torque)
		{
			jitterBody.AddTorque(JitterDatatypesMapping.Convert(ref torque));
		}

		/// <summary>
		/// Apply an impulse at a point. This immediately modifies the velocity. It wakes up the body.
		/// </summary>
		public override void ApplyLinearImpulse(Vector3D impulse)
		{
			jitterBody.ApplyImpulse(JitterDatatypesMapping.Convert(ref impulse));
		}

		/// <summary>
		/// Apply an impulse at a point. This immediately modifies the velocity. It wakes up the body
		/// and modifies the angular velocity if the point of application is not at the center of mass.
		/// </summary>
		public override void ApplyLinearImpulse(Vector3D impulse, Vector3D impulsePosition)
		{
			jitterBody.ApplyImpulse(JitterDatatypesMapping.Convert(ref impulse),
				JitterDatatypesMapping.Convert(ref impulsePosition));
		}

		public override void ApplyAngularImpulse(Vector3D impulse)
		{
			jitterBody.ApplyImpulse(JitterDatatypesMapping.Convert(ref impulse));
		}
	}
}