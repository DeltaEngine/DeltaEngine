using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using DeltaEngine.Extensions;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Ray struct, used to fire rays into a 3D scene to find out what we can
	/// hit with that ray (for mouse picking and other simple collision stuff).
	/// </summary>
	[DebuggerDisplay("Ray(Origin {Origin}, Direction {Direction})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct Ray : IEquatable<Ray>
	{
		public Ray(Vector3D origin, Vector3D direction)
		{
			Origin = origin;
			Direction = direction;
		}

		public Ray(string stringRay) 
			: this()
		{
			var partitions = stringRay.SplitAndTrim(new[]{'{', '}'});
			if(partitions.Length != 5)
				throw new InvalidNumberOfStringComponents();
			if (partitions[0] != "Ray(" || partitions[2] != "," || partitions[4] != ")")
				throw  new InvalidStringFormat();
			Origin = new Vector3D(partitions[1]);
			Direction = new Vector3D(partitions[3]);
		}

		public class InvalidNumberOfStringComponents : Exception{}

		public class InvalidStringFormat : Exception{}

		public Vector3D Origin;
		public Vector3D Direction;

		public bool Equals(Ray other)
		{
			return Origin == other.Origin && Direction == other.Direction;
		}

		[Pure]
		public override string ToString()
		{
			return "Ray({" + Origin + "},{" + Direction + "})";
		}
	}
}