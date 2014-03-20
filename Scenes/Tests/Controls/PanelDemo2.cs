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
	public class PanelDemo2 : TestWithMocksOrVisually
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
			if (position.X - panel.DrawArea.Left < ScalingPanel.EdgeWidth)
				draggingStyle = DraggingStyle.ExpandLeft;
			if (panel.DrawArea.Right - position.X < ScalingPanel.EdgeWidth)
				draggingStyle = DraggingStyle.ExpandRight;
			if (panel.DrawArea.Bottom - position.Y < ScalingPanel.EdgeWidth)
				draggingStyle |= DraggingStyle.ExpandBottom;
			if (position.Y - panel.DrawArea.Top < ScalingPanel.EdgeWidth)
				draggingStyle = DraggingStyle.Move;
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

		private class ScalingPanel : Scene
		{
			public ScalingPanel(Color backgroundColor, Color borderColor, Rectangle drawArea)
			{
				CreateBorder(borderColor);
				CreateInterior(backgroundColor);
				CreateTopLeftButton();
				CreateTopMiddleButton();
				CreateTopRightButton();
				CreateBottomLeftButton();
				CreateBottomRightButton();
				CreateUnanchoredButton();
				CreateText();
				DrawArea = drawArea;
			}

			private void CreateBorder(Color borderColor)
			{
				Add(new Picture(new Theme(), new Material(borderColor, ShaderFlags.Position2DColored), 
					Rectangle.One)
				{
					LeftMargin = new Margin(Edge.Left, 0.0f),
					RightMargin = new Margin(Edge.Right, 0.0f),
					TopMargin = new Margin(Edge.Top, 0.0f),
					BottomMargin = new Margin(Edge.Bottom, 0.0f)
				});
			}

			private void CreateInterior(Color backgroundColor)
			{
				Add(Interior = new Picture(new Theme(), 
					new Material(backgroundColor, ShaderFlags.Position2DColored), Rectangle.One)
				{
					LeftMargin = new Margin(Edge.Left, EdgeWidth),
					RightMargin = new Margin(Edge.Right, EdgeWidth),
					TopMargin = new Margin(Edge.Top, EdgeWidth),
					BottomMargin = new Margin(Edge.Bottom, EdgeWidth)
				});
			}

			public const float EdgeWidth = 0.02f;
			public Picture Interior { get; private set; }

			private void CreateTopLeftButton()
			{
				Add(topLeft = new InteractiveButton(ControlDrawArea)
				{
					LeftMargin = new Margin(Edge.Left, ControlMargin),
					TopMargin = new Margin(Edge.Top, ControlMargin),
					Text = "Top\nLeft"
				});
			}

			private InteractiveButton topLeft;
			private const float ControlMargin = 0.04f;
			private static readonly Rectangle ControlDrawArea = new Rectangle(Vector2D.Zero,
				new Size(0.2f));

			private void CreateTopMiddleButton()
			{
				Add(new InteractiveButton(ControlDrawArea)
				{
					LeftMargin = new Margin(topLeft, Edge.Right, 0.005f),
					TopMargin = new Margin(topLeft, Edge.Top, 0.0f),
					Text = "Attached"
				});
			}

			private void CreateTopRightButton()
			{
				Add(new InteractiveButton(ControlDrawArea)
				{
					RightMargin = new Margin(Edge.Right, ControlMargin),
					TopMargin = new Margin(Edge.Top, ControlMargin),
					Text = "Top\nRight"
				});
			}

			private void CreateBottomLeftButton()
			{
				Add(new InteractiveButton(ControlDrawArea)
				{
					LeftMargin = new Margin(Edge.Left, ControlMargin),
					BottomMargin = new Margin(Edge.Bottom, ControlMargin),
					Text = "Bottom\nLeft"
				});
			}

			private void CreateBottomRightButton()
			{
				Add(new InteractiveButton(ControlDrawArea)
				{
					RightMargin = new Margin(Edge.Right, ControlMargin),
					BottomMargin = new Margin(Edge.Bottom, ControlMargin),
					Text = "Bottom\nRight"
				});
			}

			private void CreateUnanchoredButton()
			{
				Add(new InteractiveButton(new Rectangle(0.4f, 0.65f, 0.2f, 0.2f)) { Text = "Unanchored" });
			}

			private void CreateText()
			{
				var theme = new Theme();
				theme.Label = new Material(Color.TransparentWhite, ShaderFlags.Position2DColored);
				Add(new Label(theme, DrawArea, "Drag top edge to move\n Drag other edges to resize")
				{
					LeftMargin = new Margin(Edge.Left, EdgeWidth),
					RightMargin = new Margin(Edge.Right, EdgeWidth),
					TopMargin = new Margin(Edge.Top, EdgeWidth),
					BottomMargin = new Margin(Edge.Bottom, EdgeWidth)
				});
			}
		}
	}
}