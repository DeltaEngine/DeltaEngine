using System;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class PanelDemo1 : TestWithMocksOrVisually
	{
		[Test]
		public void DisplayScalingPanel()
		{
			panel = new ScalingPanel(Color.PaleGreen, Color.Blue, new Rectangle(0.2f, 0.3f, 0.5f, 0.4f));
			new Command(BeginDrag).Add(new MouseButtonTrigger());
			new Command(Drag).Add(new MouseDragTrigger());
		}

		private ScalingPanel panel;

		private void BeginDrag(Vector2D position)
		{ //ncrunch: no coverage start
			lastDragPosition = Vector2D.Unused;
			if (!panel.DrawArea.Contains(position) || panel.Interior.DrawArea.Contains(position))
				return;
			lastDragPosition = position;
			CreateOutline();
			SetDraggingStype(position);
		}

		private Vector2D lastDragPosition;
		private Line2D outline;

		private void CreateOutline()
		{
			if (outline != null && outline.IsActive)
				outline.Dispose();
			outline = new Line2D(panel.DrawArea, Color.White) { RenderLayer = int.MaxValue };
		}

		private void SetDraggingStype(Vector2D position)
		{
			draggingStyle = DraggingStyle.Move;
			if (position.X - panel.DrawArea.Left < ScalingPanel.EdgeWidth)
				draggingStyle = DraggingStyle.ExpandLeft;
			if (panel.DrawArea.Right - position.X < ScalingPanel.EdgeWidth)
				draggingStyle = DraggingStyle.ExpandRight;
			if (panel.DrawArea.Bottom - position.Y < ScalingPanel.EdgeWidth)
				draggingStyle |= DraggingStyle.ExpandBottom;
		}

		private DraggingStyle draggingStyle;

		[Flags]
		private enum DraggingStyle
		{
			Move = 0,
			ExpandLeft = 1,
			ExpandRight = 2,
			ExpandBottom = 4
		}

		private void Drag(Vector2D start, Vector2D position, bool done)
		{
			if (lastDragPosition == Vector2D.Unused)
				return;
			UpdatePanelDrawArea(position - lastDragPosition);
			outline.DrawArea = panel.DrawArea;
			lastDragPosition = position;
			if (done)
				outline.Dispose();
		}

		private void UpdatePanelDrawArea(Vector2D delta)
		{
			if (draggingStyle.HasFlag(DraggingStyle.ExpandLeft))
				panel.DrawArea = new Rectangle(panel.DrawArea.Left + delta.X, panel.DrawArea.Top,
					panel.DrawArea.Width - delta.X, panel.DrawArea.Height);
			if (draggingStyle.HasFlag(DraggingStyle.ExpandRight))
				panel.DrawArea = new Rectangle(panel.DrawArea.Left, panel.DrawArea.Top,
					panel.DrawArea.Width + delta.X, panel.DrawArea.Height);
			if (draggingStyle.HasFlag(DraggingStyle.ExpandBottom))
				panel.DrawArea = new Rectangle(panel.DrawArea.Left, panel.DrawArea.Top,
					panel.DrawArea.Width, panel.DrawArea.Height + delta.Y);
			if (draggingStyle == DraggingStyle.Move)
				panel.DrawArea = panel.DrawArea.Move(delta);
		}

		private class ScalingPanel : Control
		{
			public ScalingPanel(Color background, Color border, Rectangle drawArea)
				: base(drawArea)
			{
				CreateBorder(border);
				CreateInterior(background);
				CreateTopLeftButton();
				CreateTopMiddleButton();
				CreateTopRightButton();
				CreateBottomLeftButton();
				CreateBottomRightButton();
				CreateText();
			}

			private void CreateBorder(Color border)
			{
				new Picture(new Theme(), new Material(border, ShaderFlags.Position2DColored), DrawArea)
				{
					LeftMargin = new Margin(this, Edge.Left, 0.0f),
					RightMargin = new Margin(this, Edge.Right, 0.0f),
					TopMargin = new Margin(this, Edge.Top, 0.0f),
					BottomMargin = new Margin(this, Edge.Bottom, 0.0f)
				};
			}

			private void CreateInterior(Color background)
			{
				Interior = new Picture(new Theme(), new Material(background, ShaderFlags.Position2DColored),
					DrawArea)
				{
					LeftMargin = new Margin(this, Edge.Left, EdgeWidth),
					RightMargin = new Margin(this, Edge.Right, EdgeWidth),
					TopMargin = new Margin(this, Edge.Top, EdgeWidth),
					BottomMargin = new Margin(this, Edge.Bottom, EdgeWidth)
				};
			}

			public const float EdgeWidth = 0.02f;
			public Picture Interior { get; private set; }

			private void CreateTopLeftButton()
			{
				topLeft = new InteractiveButton(ControlDrawArea)
				{
					LeftMargin = new Margin(this, Edge.Left, ControlMargin),
					TopMargin = new Margin(this, Edge.Top, ControlMargin),
					Text = "Top\nLeft"
				};
			}

			private InteractiveButton topLeft;
			private const float ControlMargin = 0.04f;
			private static readonly Rectangle ControlDrawArea = new Rectangle(Vector2D.Zero,
				new Size(0.1f));

			private void CreateTopMiddleButton()
			{
				new InteractiveButton(ControlDrawArea)
				{
					LeftMargin = new Margin(topLeft, Edge.Right, 0.005f),
					TopMargin = new Margin(topLeft, Edge.Top, 0.0f),
					Text = "Attached"
				};
			}

			private void CreateTopRightButton()
			{
				new InteractiveButton(ControlDrawArea)
				{
					RightMargin = new Margin(this, Edge.Right, ControlMargin),
					TopMargin = new Margin(this, Edge.Top, ControlMargin),
					Text = "Top\nRight"
				};
			}

			private void CreateBottomLeftButton()
			{
				new InteractiveButton(ControlDrawArea)
				{
					LeftMargin = new Margin(this, Edge.Left, ControlMargin),
					BottomMargin = new Margin(this, Edge.Bottom, ControlMargin),
					Text = "Bottom\nLeft"
				};
			}

			private void CreateBottomRightButton()
			{
				new InteractiveButton(ControlDrawArea)
				{
					RightMargin = new Margin(this, Edge.Right, ControlMargin),
					BottomMargin = new Margin(this, Edge.Bottom, ControlMargin),
					Text = "Bottom\nRight"
				};
			}

			private void CreateText()
			{
				var theme = new Theme();
				theme.Label = new Material(Color.TransparentWhite, ShaderFlags.Position2DColored);
				new Label(theme, DrawArea, "Drag top edge to move\n Drag other edges to resize")
				{
					LeftMargin = new Margin(this, Edge.Left, EdgeWidth),
					RightMargin = new Margin(this, Edge.Right, EdgeWidth),
					TopMargin = new Margin(this, Edge.Top, EdgeWidth),
					BottomMargin = new Margin(this, Edge.Bottom, EdgeWidth)
				};
			}
		}
	}
}