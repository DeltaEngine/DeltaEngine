using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D;

namespace DeltaEngine.GameLogic
{
	public abstract class Actor3D : HierarchyEntity3D, Actor
	{
		protected Actor3D(Vector3D position)
			: this(position, Quaternion.Identity) {}

		protected Actor3D(Vector3D position, Quaternion orientation)
			: base(position, orientation)
		{
			scaleFactor = 1;
		}

		public float RotationZ
		{
			get { return rotationZ; }
			set
			{
				rotationZ = value;
				Orientation = Quaternion.FromAxisAngle(Vector3D.UnitZ, RotationZ);
				OnOrientationChanged();
			}
		}

		private float rotationZ;

		public float ScaleFactor
		{
			get { return scaleFactor; }
			set
			{
				scaleFactor = value;
				Scale = new Vector3D(scaleFactor, scaleFactor, scaleFactor);
			}
		}

		private float scaleFactor;

		public abstract void RenderModel();

		public bool IsColliding(Actor other)
		{
			return GetBoundingBox().IsColliding(other.GetBoundingBox());
		}

		public bool Is2D()
		{
			return false;
		}

		public Rectangle GetDrawArea()
		{
			return Rectangle.FromCenter(Position.X, Position.Y, Scale.X, Scale.Y);
		}

		public BoundingBox GetBoundingBox()
		{
			return BoundingBox.FromCenter(Position, Scale);
		}

		public BoundingSphere GetBoundingSphere()
		{
			return new BoundingSphere(Position, Math.Max(Scale.X, Math.Max(Scale.Y, Scale.Z)));
		}

		protected override void OnPositionChanged()
		{
			if (PositionChanged != null)
				PositionChanged();
			base.OnPositionChanged();
		}

		public event Action PositionChanged;

		protected override void OnOrientationChanged()
		{
			if (OrientationChanged != null)
				OrientationChanged();
			base.OnOrientationChanged();
		}

		public event Action OrientationChanged;

		protected override void OnScaleChanged()
		{
			if (ScaleChanged != null)
				ScaleChanged();
			base.OnScaleChanged();
		}
		public event Action ScaleChanged;
	}
}