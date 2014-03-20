using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DeltaEngine.Extensions;

namespace DeltaEngine.Datatypes
{
	public class RangeGraph<T> : BaseRangeGraph<T>
		where T : Lerp<T>
	{
		protected RangeGraph() {}

		public RangeGraph(T minimum, T maximum)
			: base(minimum, maximum) {}

		public RangeGraph(T value)
			: this(CreateListFromValue(value)) {}

		private static List<T> CreateListFromValue(T value)
		{
			return new List<T> { value };
		}

		public RangeGraph(List<T> values)
			: base(values) {}

		public RangeGraph(T[] values)
			: base(values) {}

		public RangeGraph(string stringRangeGraph)
		{
			var partitions = stringRangeGraph.SplitAndTrim(new[] { '{', '}' });
			if (partitions.Length < 5 || (partitions.Length - 1) % 2 != 0)
				throw new InvalidStringFormat();
			Values = new T[partitions.Length / 2];
			try
			{
				TryCreateInstance(partitions);
			}
			catch (Exception)
			{
				throw new TypeInStringNotEqualToInitializedType();
			}
		}

		private void TryCreateInstance(string[] partitions)
		{
			for (int i = 1; i < partitions.Length; i += 2)
				Values[i / 2] = (T)Activator.CreateInstance(typeof(T), partitions[i]);
		}

		public override T GetInterpolatedValue(float interpolation)
		{
			if (Values.Length == 1)
				return Values[0];
			if (interpolation >= 1.0f)
				return Values[Values.Length - 1];
			var intervalLeft = (int)(interpolation * (Values.Length - 1));
			var intervalInterpolation = (interpolation * (Values.Length - 1)) - intervalLeft;
			return Values[intervalLeft].Lerp(Values[intervalLeft + 1], intervalInterpolation);
		}

		public override void SetValue(int index, T value)
		{
			if (index >= Values.Length)
				AddValueAfter(Values.Length, value);
			else if (index < 0)
				AddValueBefore(0, value);
			else
				Values[index] = value;
		}

		public void AddValueAfter(int leftIndex, T value, float percentageInbetween = 0.5f)
		{
			if (leftIndex < 0)
			{
				AddValueBefore(0, value);
				return;
			}
			var insertIndex = leftIndex >= Values.Length ? Values.Length : leftIndex + 1;
			ExpandAndAddValue(insertIndex, value);
		}

		public void AddValueBefore(int rightIndex, T value, float percentageInbetween = 0.5f)
		{
			if (rightIndex >= Values.Length)
			{
				AddValueAfter(Values.Length, value);
				return;
			}
			var insertIndex = rightIndex < 0 ? 0 : rightIndex;
			ExpandAndAddValue(insertIndex, value);
		}

		[Pure]
		public override string ToString()
		{
			string stringOfValues = "({" + Start + "}";
			for (int i = 1; i < Values.Length; i++)
				stringOfValues += ", {" + Values[i] + "}";
			stringOfValues += ")";
			return stringOfValues;
		}
	}
}