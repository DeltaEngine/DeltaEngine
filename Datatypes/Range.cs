using System;
using System.Diagnostics.Contracts;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Interval of two values; Allows a random value in between to be obtained.
	/// </summary>
	public class Range<T> : Lerp<Range<T>>
		where T : Lerp<T>
	{
		public Range() {}

		public Range(T minimum, T maximum)
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			Start = minimum;
			End = maximum;
		}

		public Range(string rangeAsString)
		{
			var partitions = rangeAsString.SplitAndTrim(new[] { '{', '}', '<', '>' });
			if (partitions.Length != 5 || partitions[0] != "(" || partitions[4] != ")" || partitions[2] != ",")
				throw new InvalidStringFormat();
			InitializeComponentsFromStringsIfPossible(partitions[1], partitions[3]);
		}

		public class TypeInStringNotEqualToInitializedType : Exception {}

		public class InvalidStringFormat : Exception {}

		private void InitializeComponentsFromStringsIfPossible(string startAsString,
			string endAsString)
		{
			try
			{
				Start = (T)Activator.CreateInstance(typeof(T), startAsString);
				End = (T)Activator.CreateInstance(typeof(T), endAsString);
			}
			catch
			{
				throw new TypeInStringNotEqualToInitializedType();
			}
		}

		public virtual T Start { get; set; }
		public virtual T End { get; set; }

		public T GetRandomValue()
		{
			return Start.Lerp(End, Randomizer.Current.Get());
		}

		public Range<T> Lerp(Range<T> other, float interpolation)
		{
			var start = Start.Lerp(other.Start, interpolation);
			var end = End.Lerp(other.End, interpolation);
			return new Range<T>(start, end);
		}

		[Pure]
		public override string ToString()
		{
			return "({" + Start + "},{" + End + "})";
		}
	}
}