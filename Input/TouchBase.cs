using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	public class TouchBase
	{
		public TouchBase()
		{
			states = new State[MaxNumberOfTouches];
			locations = new Vector2D[MaxNumberOfTouches];
			ids = new int[MaxNumberOfTouches];
			for (int index = 0; index < MaxNumberOfTouches; index++)
				ids[index] = -1;
		}

		public const int MaxNumberOfTouches = 10;
		public readonly State[] states;
		public readonly Vector2D[] locations;
		public readonly int[] ids;

		public int FindIndexByIdOrGetFreeIndex(int id)
		{
			for (int index = 0; index < ids.Length; index++)
				if (ids[index] == id)
					return index;
			for (int index = 0; index < ids.Length; index++)
				if (ids[index] == -1)
					return index;
			return -1;
		}
	}
}