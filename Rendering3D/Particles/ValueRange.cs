using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Rendering3D.Particles
{
	public struct ValueRange : Lerp<ValueRange>
	{
		public ValueRange(float value)
			: this(value, value) {}

		public ValueRange(float minimum, float maximum)
			: this()
		{
			Start = minimum;
			End = maximum;
		}

		public ValueRange(string rangeString)
			: this()
		{
			float[] partitions = rangeString.SplitIntoFloats();
			if(partitions.Length != 2)
				throw new InvalidStringFormat();
			Start = partitions[0];
			End = partitions[1];
		}

		public class InvalidStringFormat : Exception{}

		public float Start { get; set; }
		public float End { get; set; }

		public float GetRandomValue()
		{
			return Start.Lerp(End, Randomizer.Current.Get());
		}

		public ValueRange Lerp(ValueRange other, float interpolation)
		{
			return new ValueRange(Start.Lerp(other.Start, interpolation),
				End.Lerp(other.End, interpolation));
		}

		public override string ToString()
		{
			return Start.ToInvariantString() + ", " + End.ToInvariantString();
		}
	}
}