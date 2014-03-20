using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Physics3D
{
	public class PhysicsShape
	{
		public PhysicsShape(ShapeType type)
		{
			ShapeType = type;
			Properties = new Dictionary<PropertyType, object>();
		}

		public Dictionary<PropertyType, object> Properties { get; private set; }

		public enum PropertyType : byte
		{
			Density,
			Radius,
			Width,
			Height,
			Depth,
			Size,
			Offset,
			Vertices,
			Heights,
			ScaleX,
			ScaleY,
			Mesh,
			LocalSpaceMatrix,
			InvertTriangles,
			NumberOfTeeth,
			TipPercentage,
			ToothHeight,
			RadiusX,
			RadiusY,
			Edges
		}

		public ShapeType ShapeType { get; private set; }

		public float Radius
		{
			get
			{
				var radius = ArrayExtensions.GetWithDefault<PropertyType, float>(Properties,
					PropertyType.Radius);
				if (radius == 0)
					throw new CannotCreateThisShapeWithoutRadius();
				return radius;
			}
			set { Properties.Add(PropertyType.Radius, value); }
		}

		public class CannotCreateThisShapeWithoutRadius : Exception {}

		public Vector3D Size
		{
			get
			{
				var size = ArrayExtensions.GetWithDefault<PropertyType, Vector3D>(Properties,
					PropertyType.Size);
				if (size.X == 0 || size.Y == 0.0f || size.Z == 0.0f)
					throw new CannotCreateThisShapeWithoutSize();
				return size;
			}
			set { Properties.Add(PropertyType.Size, value); }
		}

		public class CannotCreateThisShapeWithoutSize : Exception {}

		public float Depth
		{
			get
			{
				var depth = ArrayExtensions.GetWithDefault<PropertyType, float>(Properties,
					PropertyType.Depth);
				if (depth == 0.0f)
					throw new CannotCreateThisShapeWithoutDepth();
				return depth;
			}
			set { Properties.Add(PropertyType.Depth, value); }
		}

		public class CannotCreateThisShapeWithoutDepth : Exception {}

		public float Height
		{
			get
			{
				var height = ArrayExtensions.GetWithDefault<PropertyType, float>(Properties,
					PropertyType.Height);
				if (height == 0)
					throw new CannotCreateThisShapeWithoutHeight();
				return height;
			}
			set { Properties.Add(PropertyType.Height, value); }
		}

		public class CannotCreateThisShapeWithoutHeight : Exception {}
	}
}