using System.Collections.Generic;
using DeltaEngine.Extensions;

namespace DeltaEngine.Datatypes
{
	public abstract class BaseRangeGraph<T> : Range<T>
		where T : Lerp<T>
	{
		protected BaseRangeGraph()
		{
			Values = new T[2];
		}

		protected BaseRangeGraph(T minimum, T maximum)
		{
			Values = new T[2];
			Values[0] = minimum;
			Values[1] = maximum;
		}

		protected BaseRangeGraph(List<T> values)
		{
			Values = values.ToArray();
		}

		protected BaseRangeGraph(T[] values)
		{
			Values = values;
		} 

		public T[] Values { get; protected set; }

		public abstract void SetValue(int index, T value);

		public override T Start
		{
			get { return Values[0]; }
			set { Values[0] = value; }
		}

		public override T End
		{
			get { return Values[Values.Length - 1]; }
			set { Values[Values.Length - 1] = value; }
		}

		protected void ExpandAndAddValue(int insertIndex, T value)
		{
			Values = Values.Insert(value, insertIndex);
		}

		public abstract T GetInterpolatedValue(float interpolation);
	}
}