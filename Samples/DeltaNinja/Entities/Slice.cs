using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Shapes;
using System.Collections.Generic;

namespace DeltaNinja.Entities
{
	class Slice
	{
		long startTime;
		bool isActive = false;
		Vector2D lastMousePosition;
		List<Line2D> segments = new List<Line2D>();

		public bool IsActive { get { return isActive; } }

		public bool IsOver
		{
			get
			{
				return isActive && GlobalTime.Current.Milliseconds - startTime >= 300;
			}
		}

		public void Reset()
		{
			foreach (var segment in segments)
				segment.IsActive = false;

			segments.Clear();
			isActive = false;
		}

		public void Switch(Vector2D position)
		{
			if (isActive)
				Reset();
			else
			{
				lastMousePosition = position;
				isActive = true;
				startTime = GlobalTime.Current.Milliseconds;
			}
		}

		public void Check(Vector2D position, IEnumerable<Logo> logoSet)
		{
			if (isActive)
			{
				foreach (var logo in logoSet)
					logo.CheckForSlicing(lastMousePosition, position);

				var segment = new Line2D(lastMousePosition, position, Color.Cyan);
				segment.RenderLayer = (int)GameRenderLayer.Segments;

				segments.Add(segment);
				segment.IsActive = true;

				lastMousePosition = position;
			}
		}
	}
}
