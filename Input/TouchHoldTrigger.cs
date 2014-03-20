using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a touch hold to be detected.
	/// </summary>
	public class TouchHoldTrigger : InputTrigger, PositionTrigger, TouchTrigger
	{
		public TouchHoldTrigger(Rectangle holdArea, float holdTime = DefaultHoldTime)
		{
			HoldArea = holdArea;
			HoldTime = holdTime;
		}

		public Rectangle HoldArea { get; private set; }
		public float HoldTime { get; private set; }
		private const float DefaultHoldTime = 1.0f;
		public float Elapsed { get; set; }
		public Vector2D Position { get; set; }

		public TouchHoldTrigger(string holdAreaAndTime)
		{
			string[] parameters = holdAreaAndTime.SplitAndTrim(new[] { ' ', ',' });
			if (parameters.Length > 3)
				HoldArea = GetWishedNumberOfElements(parameters, 4).ToText().Convert<Rectangle>();
			if (parameters.Length > 4)
				HoldTime = parameters[4].Convert<float>();
		}

		/// <summary>
		/// Custom solution to imitate Linq.Enumerable.Take(maxNumberOfElements) as long as our
		/// code conversion is able to support it, see case 10287
		/// </summary>
		private static IList<T> GetWishedNumberOfElements<T>(IList<T> elements, int maxNumberOfElements)
		{
			if (elements.Count <= maxNumberOfElements)
				return elements; //ncrunch: no coverage
			var newList = new List<T>();
			foreach (T element in elements)
				if (newList.Count < maxNumberOfElements)
					newList.Add(element);
				else
					break;
			return newList;
		}

		protected override void StartInputDevice()
		{
			Start<Touch>();
		}

		public void HandleWithTouch(Touch touch)
		{
			if (touch.GetState(0) == State.Pressing)
				startPosition = touch.GetPosition(0);
			Position = touch.GetPosition(0);
			if (CheckHoverState(touch) && IsHovering())
				Invoke();
			else
				Elapsed = 0.0f;
		}

		private Vector2D startPosition;

		private bool CheckHoverState(Touch touch)
		{
			return HoldArea.Contains(startPosition) &&
				touch.GetState(0) == State.Pressed &&
				startPosition.DistanceTo(touch.GetPosition(0)) < PositionEpsilon;
		}

		public bool IsHovering()
		{
			if (Elapsed >= HoldTime || !HoldArea.Contains(Position))
				return false;
			Elapsed += Time.Delta;
			return Elapsed >= HoldTime;
		}
	}
}