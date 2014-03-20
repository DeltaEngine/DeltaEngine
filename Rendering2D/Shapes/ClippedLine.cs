using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Shapes
{
	/// <summary>
	/// Helps with the Line2D.Clip method to limit lines by a clipping bounds rectangle.
	/// </summary>
	internal class ClippedLine
	{
		public ClippedLine(Vector2D startPoint, Vector2D endPoint, Rectangle clippingBounds)
		{
			StartPoint = startPoint;
			EndPoint = endPoint;
			direction = endPoint - startPoint;
			this.clippingBounds = clippingBounds;
			isStartInside = InclusiveContains(startPoint);
			isEndInside = InclusiveContains(endPoint);
			ClipLine();
		}

		public Vector2D StartPoint { get; private set; }
		public Vector2D EndPoint { get; private set; }
		private readonly Vector2D direction;
		private readonly Rectangle clippingBounds;
		private readonly bool isStartInside;
		private readonly bool isEndInside;

		private bool InclusiveContains(Vector2D point)
		{
			return point.X >= clippingBounds.Left && point.X <= clippingBounds.Right &&
				point.Y >= clippingBounds.Top && point.Y <= clippingBounds.Bottom;
		}

		private void ClipLine()
		{
			if (isStartInside && isEndInside)
			{
				IsVisible = true;
				return;
			}

			if (CantIntersect())
			{
				IsVisible = false;
				return;
			}

			UpdateIntersects();
			bool wasStartClipped = (!isStartInside && ClipStart());
			bool wasEndClipped = (!isEndInside && ClipEnd());
			IsVisible = wasStartClipped || wasEndClipped;
		}

		public bool IsVisible { get; private set; }

		private bool CantIntersect()
		{
			if (StartPoint.X < clippingBounds.Left && EndPoint.X < clippingBounds.Left)
				return true;

			if (StartPoint.X > clippingBounds.Right && EndPoint.X > clippingBounds.Right)
				return true;

			if (StartPoint.Y < clippingBounds.Top && EndPoint.Y < clippingBounds.Top)
				return true;

			return StartPoint.Y > clippingBounds.Bottom && EndPoint.Y > clippingBounds.Bottom;
		}

		private void UpdateIntersects()
		{
			intersects[0] = Intersects(clippingBounds.TopLeft, clippingBounds.BottomLeft);
			intersects[1] = Intersects(clippingBounds.TopRight, clippingBounds.BottomRight);
			intersects[2] = Intersects(clippingBounds.TopLeft, clippingBounds.TopRight);
			intersects[3] = Intersects(clippingBounds.BottomLeft, clippingBounds.BottomRight);
		}

		private readonly Vector2D?[] intersects = new Vector2D?[4];

		private Vector2D? Intersects(Vector2D corner1, Vector2D corner2)
		{
			Vector2D edge = corner2 - corner1;
			float dotProduct = direction.X * edge.Y - direction.Y * edge.X;
			if (dotProduct == 0)
				return null;

			Vector2D lineToLine = corner1 - StartPoint;
			float t = (lineToLine.X * edge.Y - lineToLine.Y * edge.X) / dotProduct;
			if (t < 0 || t > 1)
				return null;

			float u = (lineToLine.X * direction.Y - lineToLine.Y * direction.X) / dotProduct;
			if (u < 0 || u > 1)
				return null;

			return StartPoint + t * direction;
		}

		private bool ClipStart()
		{
			Vector2D? intersect = GetClosestIntersectTo(StartPoint);
			if (intersect == null)
				return false;

			StartPoint = (Vector2D)intersect;
			return true;
		}

		private Vector2D? GetClosestIntersectTo(Vector2D point)
		{
			closestIntersect = null;
			for (int i = 0; i < 4; i++)
				if (intersects[i] != null)
					UpdateClosestIntersect(point, (Vector2D)intersects[i]);

			return closestIntersect;
		}

		private Vector2D? closestIntersect;

		private void UpdateClosestIntersect(Vector2D point, Vector2D intersectCandidate)
		{
			float distance = point.DistanceToSquared(intersectCandidate);
			if (closestIntersect != null && distance >= shortestDistance)
				return;

			closestIntersect = intersectCandidate;
			shortestDistance = distance;
		}

		private float shortestDistance;

		private bool ClipEnd()
		{
			Vector2D? intersect = GetClosestIntersectTo(EndPoint);
			if (intersect == null)
				return false;

			EndPoint = (Vector2D)intersect;
			return true;
		}
	}
}