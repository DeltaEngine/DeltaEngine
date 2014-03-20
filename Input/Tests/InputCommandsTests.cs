using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class InputCommandsTests : TestWithMocksOrVisually
	{
		[Test, Timeout(5000)]
		public void CountPressingAndReleasing()
		{
			int pressed = 0;
			int released = 0;
			var fontText = new FontText(Font.Default,
				"MouseLeft pressed: " + pressed + " released: " + released, Rectangle.One);
			//ncrunch: no coverage start
			new Command(
				() => fontText.Text = "MouseLeft pressed: " + ++pressed + " released: " + released).Add(
					new MouseButtonTrigger());
			new Command(
				() => fontText.Text = "MouseLeft pressed: " + pressed + " released: " + ++released).Add(
					new MouseButtonTrigger(MouseButton.Left, State.Releasing));
			//ncrunch: no coverage end
		}

		[Test, CloseAfterFirstFrame]
		public void GetInputCommands()
		{
			var inputCommands = ContentLoader.Load<InputCommands>("DefaultCommands");
			if (inputCommands.MetaData != null)
				Assert.AreEqual(ContentType.InputCommand, inputCommands.MetaData.Type);
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void ZoomOnLines()
		{
			var line1 = new Line2D(new List<Vector2D>
			{
				new Vector2D(0.4f, 0.4f),
				new Vector2D(0.6f, 0.4f),
				new Vector2D(0.4f, 0.6f),
				new Vector2D(0.6f, 0.6f),
				new Vector2D(0.4f, 0.5f),
				new Vector2D(0.5f, 0.5f)
			}, Color.Red);
			new Command("Zoom", zoomAmount => ZoomLinePoints(line1.Points, zoomAmount));
		}

		private static void ZoomLinePoints(List<Vector2D> points, float zoomAmount)
		{
			Vector2D center = Vector2D.Half;
			for (int i = 0; i < points.Count; i++)
				points[i] = center + center.DirectionTo(points[i]) * (1 + zoomAmount);
		}
	}
}