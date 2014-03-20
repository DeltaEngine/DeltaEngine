using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using DeltaEngine.Extensions;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Min and max vector for a 3D bounding box. Can also be used to calculate a BoundingSphere.
	/// </summary>
	[DebuggerDisplay("BoundingBox(Min={Min}, Max={Max})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct BoundingBox : IEquatable<BoundingBox>
	{
		public BoundingBox(Vector3D min, Vector3D max)
			: this()
		{
			Min = min;
			Max = max;
		}

		public Vector3D Min;
		public Vector3D Max;

		public BoundingBox(IList<Vector3D> points)
			: this()
		{
			if (points == null || points.Count == 0)
				throw new NoPointsSpecified();
			Min = points[0];
			Max = points[0];
			for (int num = 0; num < points.Count; num++)
			{
				IncreaseMinimumDimensionIfNecessary(points[num]);
				IncreaseMaximumDimensionIfNecessary(points[num]);
			}
		}

		public class NoPointsSpecified : Exception {}

		private void IncreaseMinimumDimensionIfNecessary(Vector3D newPossibleMinimum)
		{
			if (newPossibleMinimum.X < Min.X)
				Min.X = newPossibleMinimum.X;
			if (newPossibleMinimum.Y < Min.Y)
				Min.Y = newPossibleMinimum.Y;
			if (newPossibleMinimum.Z < Min.Z)
				Min.Z = newPossibleMinimum.Z;
		}

		private void IncreaseMaximumDimensionIfNecessary(Vector3D newPossibleMaximum)
		{
			if (newPossibleMaximum.X > Max.X)
				Max.X = newPossibleMaximum.X;
			if (newPossibleMaximum.Y > Max.Y)
				Max.Y = newPossibleMaximum.Y;
			if (newPossibleMaximum.Z > Max.Z)
				Max.Z = newPossibleMaximum.Z;
		}

		public static BoundingBox FromCenter(Vector3D position, Vector3D scale)
		{
			return new BoundingBox(position - scale / 2, position + scale / 2);
		}

		public override bool Equals(object obj)
		{
			return obj is BoundingBox && Equals((BoundingBox)obj);
		}

		[Pure]
		public bool Equals(BoundingBox other)
		{
			return Min.Equals(other.Min) && Max.Equals(other.Max);
		}

		public override int GetHashCode()
		{
			// ReSharper disable NonReadonlyFieldInGetHashCode
			return Min.GetHashCode() ^ Max.GetHashCode();
		}

		[Pure]
		public bool IsNearlyEqual(BoundingBox other)
		{
			return Min.IsNearlyEqual(other.Min) && Max.IsNearlyEqual(other.Max);
		}

		public bool IsColliding(BoundingBox other)
		{
			return Max.X >= other.Min.X && Min.X <= other.Max.X && Max.Y >= other.Min.Y &&
				Min.Y <= other.Max.Y && Max.Z >= other.Min.Z && Min.Z <= other.Max.Z;
		}

		public bool IsColliding(BoundingSphere sphere)
		{
			Vector3D clampedLocation = new Vector3D(GetClampedValueX(sphere.Center.X),
				GetClampedValueY(sphere.Center.Y), GetClampedValueZ(sphere.Center.Z));
			return clampedLocation.DistanceSquared(sphere.Center) <= (sphere.Radius * sphere.Radius);
		}

		private float GetClampedValueX(float positionX)
		{
			return positionX > Max.X ? Max.X : MathExtensions.Max(positionX, Min.X);
		}

		private float GetClampedValueY(float positionY)
		{
			return positionY > Max.Y ? Max.Y : MathExtensions.Max(positionY, Min.Y);
		}

		private float GetClampedValueZ(float positionZ)
		{
			return positionZ > Max.Z ? Max.Z : MathExtensions.Max(positionZ, Min.Z);
		}

		public Vector3D? Intersect(Ray ray)
		{
			var oneOverDirection = new Vector3D(1.0f / ray.Direction.X, 1.0f / ray.Direction.Y,
				1.0f / ray.Direction.Z);
			float distMinX = (Min.X - ray.Origin.X) * oneOverDirection.X;
			float distMaxX = (Max.X - ray.Origin.X) * oneOverDirection.X;
			float distMinY = (Min.Y - ray.Origin.Y) * oneOverDirection.Y;
			float distMaxY = (Max.Y - ray.Origin.Y) * oneOverDirection.Y;
			float distMinZ = (Min.Z - ray.Origin.Z) * oneOverDirection.Z;
			float distMaxZ = (Max.Z - ray.Origin.Z) * oneOverDirection.Z;
			float distMax = MathExtensions.Min(
				MathExtensions.Min(MathExtensions.Max(distMinX, distMaxX),
					MathExtensions.Max(distMinY, distMaxY)), MathExtensions.Max(distMinZ, distMaxZ));
			if (distMax < 0)
				return null;
			float distMin = MathExtensions.Max(
				MathExtensions.Max(MathExtensions.Min(distMinX, distMaxX),
					MathExtensions.Min(distMinY, distMaxY)), MathExtensions.Min(distMinZ, distMaxZ));
			if (distMin > distMax)
				return null;
			return ray.Origin + ray.Direction * distMin;
		}

		public void Merge(BoundingBox otherBox)
		{
			if (otherBox.Min.X < Min.X)
				Min.X = otherBox.Min.X;
			if (otherBox.Min.Y < Min.Y)
				Min.Y = otherBox.Min.Y;
			if (otherBox.Min.Z < Min.Z)
				Min.Z = otherBox.Min.Z;
			if (otherBox.Max.X > Max.X)
				Max.X = otherBox.Max.X;
			if (otherBox.Max.Y > Max.Y)
				Max.Y = otherBox.Max.Y;
			if (otherBox.Max.Z > Max.Z)
				Max.Z = otherBox.Max.Z;
		}
	}
}