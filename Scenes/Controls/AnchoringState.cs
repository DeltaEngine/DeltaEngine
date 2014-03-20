using System;
using DeltaEngine.Datatypes;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Scenes.Controls
{
	public class AnchoringState
	{
		public AnchoringState()
		{
			TopMargin = new Margin(Edge.Top);
			BottomMargin = new Margin(Edge.Bottom);
			LeftMargin = new Margin(Edge.Left);
			RightMargin = new Margin(Edge.Right);
			PercentageSpan = -1;
		}

		public Margin TopMargin { get; set; }
		public Margin BottomMargin { get; set; }
		public Margin LeftMargin { get; set; }
		public Margin RightMargin { get; set; }
		public float PercentageSpan { get; set; }
		public float NoIdea { get; set; }

		public Rectangle CalculateDrawArea(Control control)
		{
			this.control = control;
			UpdateDrawAreaHint(GetAnchoringDrawArea(control));
			if (HasNoMargins())
				return drawAreaHint;
			if (HasOppositeMargins())
				return AlignOppositeMargins();
			if (NumberOfMargins <= 2)
				return AlignAdjacentMargins();
			if (NumberOfMargins == 3)
				return AlignThreeMargins();
			return AlignFourMargins();
		}

		[NonSerialized]
		private Control control;

		private static Rectangle GetAnchoringDrawArea(Control control)
		{
			var size = control.AnchoringSize;
			return (size == Size.Unused
				? control.DrawArea : Rectangle.FromCenter(control.DrawArea.Center, size));
		}

		[NonSerialized]
		private Rectangle drawAreaHint;

		private void UpdateDrawAreaHint(Rectangle currentDrawArea)
		{
			if (PercentageSpan == -1)
			{
				drawAreaHint = currentDrawArea;
				return;
			}
			float aspect = currentDrawArea.Aspect;
			if (ScreenSpace.Scene.Viewport.Aspect / aspect > 1)
				SetWorkingDrawAreaBasedOnHeight(aspect);
			else
				SetWorkingDrawAreaBasedOnWidth(aspect);
			drawAreaHint = CenteredDrawAreaHint();
		}

		private void SetWorkingDrawAreaBasedOnHeight(float aspect)
		{
			drawAreaHint.Width = Height * PercentageSpan * aspect;
			drawAreaHint.Height = Height * PercentageSpan;
		}

		private float Height
		{
			get { return BottomEdge - TopEdge; }
		}

		private float BottomEdge
		{
			get
			{
				return BottomMargin.Other == null
					? Viewport.Bottom
					: BottomMargin.OthersEdge == Edge.Top
						? GetAnchoringDrawArea(BottomMargin.Other).Top
						: GetAnchoringDrawArea(BottomMargin.Other).Bottom;
			}
		}

		private Rectangle Viewport
		{
			get
			{
				return control.SceneDrawArea == Rectangle.Unused
					? ScreenSpace.Current.Viewport : control.SceneDrawArea;
			}
		}

		private float TopEdge
		{
			get
			{
				return TopMargin.Other == null
					? Viewport.Top
					: TopMargin.OthersEdge == Edge.Top
						? GetAnchoringDrawArea(TopMargin.Other).Top
						: GetAnchoringDrawArea(TopMargin.Other).Bottom;
			}
		}

		private void SetWorkingDrawAreaBasedOnWidth(float aspect)
		{
			drawAreaHint.Width = Width * PercentageSpan;
			drawAreaHint.Height = Width * PercentageSpan / aspect;
		}

		private float Width
		{
			get { return RightEdge - LeftEdge; }
		}

		private float RightEdge
		{
			get
			{
				return RightMargin.Other == null
					? Viewport.Right
					: RightMargin.OthersEdge == Edge.Left
						? GetAnchoringDrawArea(RightMargin.Other).Left
						: GetAnchoringDrawArea(RightMargin.Other).Right;
			}
		}

		private float LeftEdge
		{
			get
			{
				return LeftMargin.Other == null
					? Viewport.Left
					: LeftMargin.OthersEdge == Edge.Left
						? GetAnchoringDrawArea(LeftMargin.Other).Left
						: GetAnchoringDrawArea(LeftMargin.Other).Right;
			}
		}

		private Rectangle CenteredDrawAreaHint()
		{
			float width = drawAreaHint.Width;
			float height = drawAreaHint.Height;
			float left = LeftEdge + (Width - width) / 2;
			float top = TopEdge + (Height - height) / 2;
			return new Rectangle(left, top, width, height);
		}

		private bool HasNoMargins()
		{
			return LeftMargin.Distance == -1 && RightMargin.Distance == -1 && TopMargin.Distance == -1 &&
			       BottomMargin.Distance == -1;
		}

		private bool HasOppositeMargins()
		{
			return NumberOfMargins == 2 &&
			       ((LeftMargin.Distance >= 0 && RightMargin.Distance >= 0) ||
			        (TopMargin.Distance >= 0 && BottomMargin.Distance >= 0));
		}

		private int NumberOfMargins
		{
			get
			{
				int margins = 0;
				if (LeftMargin.Distance >= 0)
					margins++;
				if (RightMargin.Distance >= 0)
					margins++;
				if (TopMargin.Distance >= 0)
					margins++;
				if (BottomMargin.Distance >= 0)
					margins++;
				return margins;
			}
		}

		private Rectangle AlignOppositeMargins()
		{
			if (LeftMargin.Distance >= 0)
				return AlignLeftRightMargins();
			return AlignTopBottomMargins();
		}

		private Rectangle AlignLeftRightMargins()
		{
			float left = LeftEdge + LeftMargin.Distance;
			float width = Width - LeftMargin.Distance - RightMargin.Distance;
			float height = width / drawAreaHint.Aspect;
			float top = 0.5f - height / 2;
			return new Rectangle(left, top, width, height);
		}

		private Rectangle AlignTopBottomMargins()
		{
			float top = TopEdge + TopMargin.Distance;
			float height = Height - TopMargin.Distance - BottomMargin.Distance;
			float width = height * drawAreaHint.Aspect;
			float left = 0.5f - width / 2;
			return new Rectangle(left, top, width, height);
		}

		private Rectangle AlignAdjacentMargins()
		{
			Rectangle drawArea = CenteredDrawAreaHint();
			if (TopMargin.Distance >= 0)
				drawArea.Top = TopEdge + TopMargin.Distance;
			if (BottomMargin.Distance >= 0)
				drawArea.Bottom = BottomEdge - BottomMargin.Distance;
			if (LeftMargin.Distance >= 0)
				drawArea.Left = LeftEdge + LeftMargin.Distance;
			if (RightMargin.Distance >= 0)
				drawArea.Right = RightEdge - RightMargin.Distance;
			return drawArea;
		}

		private Rectangle AlignThreeMargins()
		{
			if (LeftMargin.Distance < 0)
				return AlignNoLeftMargin();
			if (RightMargin.Distance < 0)
				return AlignNoRightMargin();
			if (TopMargin.Distance < 0)
				return AlignNoTopMargin();
			return AlignNoBottomMargin();
		}

		private Rectangle AlignNoLeftMargin()
		{
			float height = Height - TopMargin.Distance - BottomMargin.Distance;
			float width = height * drawAreaHint.Aspect;
			float right = RightEdge - RightMargin.Distance;
			float top = TopEdge + TopMargin.Distance;
			return new Rectangle(right - width, top, width, height);
		}

		private Rectangle AlignNoRightMargin()
		{
			float left = LeftEdge + LeftMargin.Distance;
			float top = TopEdge + TopMargin.Distance;
			float height = Height - TopMargin.Distance - BottomMargin.Distance;
			float width = height * drawAreaHint.Aspect;
			return new Rectangle(left, top, width, height);
		}

		private Rectangle AlignNoTopMargin()
		{
			float width = Width - LeftMargin.Distance - RightMargin.Distance;
			float height = width / drawAreaHint.Aspect;
			float left = LeftEdge + LeftMargin.Distance;
			float bottom = BottomEdge - BottomMargin.Distance;
			return new Rectangle(left, bottom - height, width, height);
		}

		private Rectangle AlignNoBottomMargin()
		{
			float left = LeftEdge + LeftMargin.Distance;
			float top = TopEdge + TopMargin.Distance;
			float width = Width - LeftMargin.Distance - RightMargin.Distance;
			float height = width / drawAreaHint.Aspect;
			return new Rectangle(left, top, width, height);
		}

		private Rectangle AlignFourMargins()
		{
			float left = LeftEdge + LeftMargin.Distance;
			float top = TopEdge + TopMargin.Distance;
			float width = Width - LeftMargin.Distance - RightMargin.Distance;
			float height = Height - TopMargin.Distance - BottomMargin.Distance;
			return new Rectangle(left, top, width, height);
		}
	}
}