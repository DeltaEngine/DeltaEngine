using System;
using DeltaEngine.Datatypes;
using System.Collections.Generic;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaNinja.Entities
{
	public class Logo : MovingSprite
	{
		public enum SizeCategory
		{
			Small = 5,
			Medium = 2,
			Big = 1
		}

		[Flags]
		private enum Sides
		{
			None = 0,
			Top = 1,
			Right = 2,
			Bottom = 4,
			Left = 8
		}

		public Logo(string image, Color color, Vector2D position, Size size, SimplePhysics.Data data)
			: base(image, color, position, size)
		{
			RenderLayer = (int)GameRenderLayer.Logos;
			sideStatus = Sides.None;
			Add(data);
			Start<TrajectoryEntityHandler>();
			Start<SimplePhysics.Rotate>();
		}

		private Sides sideStatus;

		public SizeCategory Category
		{
			get
			{
				float logoSize = DrawArea.Width;

				if (logoSize < 0.03f) return SizeCategory.Small;
				if (logoSize < 0.05f) return SizeCategory.Medium;
				return SizeCategory.Big;
			}
		}

		public override void SetPause(bool pause)
		{
			if(pause)
				Remove<SimplePhysics.Rotate>();
			else
				Start<SimplePhysics.Rotate>();

			base.SetPause(pause);
		}

		public void CheckForSlicing(Vector2D start, Vector2D end)
		{
			if (!sideStatus.HasFlag(Sides.Top) && CheckIfLineIntersectsLine(start, end, DrawArea.TopLeft, DrawArea.TopRight)) sideStatus |= Sides.Top;
			if (!sideStatus.HasFlag(Sides.Left) && CheckIfLineIntersectsLine(start, end, DrawArea.TopLeft, DrawArea.BottomLeft)) sideStatus |= Sides.Left;
			if (!sideStatus.HasFlag(Sides.Bottom) && CheckIfLineIntersectsLine(start, end, DrawArea.BottomLeft, DrawArea.BottomRight)) sideStatus |= Sides.Bottom;
			if (!sideStatus.HasFlag(Sides.Right) && CheckIfLineIntersectsLine(start, end, DrawArea.TopRight, DrawArea.BottomRight)) sideStatus |= Sides.Right;
		}

		public void ResetSlicing()
		{
			sideStatus = Sides.None;
		}

		public bool IsSliced
		{
			get
			{
				int count = 0;
				if (sideStatus.HasFlag(Sides.Top)) count++;
				if (sideStatus.HasFlag(Sides.Right)) count++;
				if (sideStatus.HasFlag(Sides.Bottom)) count++;
				if (sideStatus.HasFlag(Sides.Left)) count++;
				return count > 1;
			}
		}

		private bool IsSideSliced(List<Line2D> segments, Vector2D startPoint, Vector2D endPoint)
		{
			foreach (var segment in segments)
			{
				if (CheckIfLineIntersectsLine(segment.StartPoint, segment.EndPoint, startPoint, endPoint)) return true;
			}
			return false;
		}
	}
}