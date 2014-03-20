using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Platforms;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Graphics.Tests
{
	/// <summary>
	/// Checks if raw performance of rendering lines and sprites is good. Fast GPU recommended. Please
	/// note that the test here simulate how the rendering classes work, you can get worse values if
	/// you draw many small batches and cause overhead the Rendering namespace optimizes already.
	/// </summary>
	public class DrawingPerformanceTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		/// <summary>
		/// Proof that 30k lines can be rendered with up to 3000fps in a small window, fast GPU. Even
		/// 1000fps means that 30 million lines are drawn each second. More than 30k lines are possible
		/// with multiple draw calls, but causes multiple CircularBuffer flushes and thus not faster.
		/// </summary>
		[Test, Category("Slow")]
		public void Draw30000LinesPerFrame()
		{
			var manyLines = new DrawingTests.Line(Vector2D.Zero, new Vector2D(1280, 720), Color.Red);
			const int NumberOfRandomLines = 30000;
			var vertices = new VertexPosition2DColor[2 * NumberOfRandomLines];
			var random = Randomizer.Current;
			var viewport = Resolve<Window>().ViewportPixelSize;
			for (int i = 0; i < NumberOfRandomLines; i++)
			{
				var startPoint = new Vector2D(random.Get(0, viewport.Width), random.Get(0, viewport.Height));
				var endPoint = startPoint + new Vector2D(random.Get(-50, 50), random.Get(-50, 50));
				vertices[i * 2 + 0] = new VertexPosition2DColor(startPoint, Color.GetRandomColor());
				vertices[i * 2 + 1] = new VertexPosition2DColor(endPoint, Color.GetRandomColor());
			}
			manyLines.Set(vertices);
		}

		/// <summary>
		/// Draws 100*100 small images (=10000 images =20000 polygons) 50 times a frame to reach 1 million
		/// polygons drawn per frame. Can reach 100fps or more, which means 100 million polygons per second.
		/// </summary>
		[Test, Category("Slow")]
		public void DrawImagesWithOneMillionPolygonsPerFrame()
		{
			var verticesAndIndices = CreateVerticesAndIndices();
			verticesAndIndices.OnDraw<RenderOneMillionPolygons>();
		}

		private List<VertexPosition2DColorUV> imagesVertices;
		private List<short> imagesIndices;

		private DrawableEntity CreateVerticesAndIndices()
		{
			imagesVertices = new List<VertexPosition2DColorUV>();
			imagesIndices = new List<short>();
			for (int y = 0; y < 100; y++)
				for (int x = 0; x < 100; x++)
				{
					CreateVertices(x, y);
					CreateIndicesForTwoPolygonsPerQuad(x, y);
				}
			var entity = new DrawableEntity();
			entity.Add(imagesVertices.ToArray());
			entity.Add(imagesIndices.ToArray());
			return entity;
		}

		private void CreateVertices(int x, int y)
		{
			var margin = new Vector2D(5, 5);
			imagesVertices.Add(new VertexPosition2DColorUV(new Vector2D(x, y) * 10 + margin,
				Color.GetRandomColor(), Vector2D.Zero));
			imagesVertices.Add(new VertexPosition2DColorUV(new Vector2D(x + 1, y) * 10 + margin,
				Color.GetRandomColor(), Vector2D.UnitX));
			imagesVertices.Add(new VertexPosition2DColorUV(new Vector2D(x + 1, y + 1) * 10 + margin,
				Color.GetRandomColor(), Vector2D.One));
			imagesVertices.Add(new VertexPosition2DColorUV(new Vector2D(x, y + 1) * 10 + margin,
				Color.GetRandomColor(), Vector2D.UnitY));
		}

		private void CreateIndicesForTwoPolygonsPerQuad(int x, int y)
		{
			int quadIndex = (y * 100 + x) * 4;
			imagesIndices.Add((short)quadIndex);
			imagesIndices.Add((short)(quadIndex + 1));
			imagesIndices.Add((short)(quadIndex + 2));
			imagesIndices.Add((short)(quadIndex));
			imagesIndices.Add((short)(quadIndex + 2));
			imagesIndices.Add((short)(quadIndex + 3));
		}

#pragma warning disable 1570
		/// <summary>
		/// Draw 100x100 quads 50 times to reach 1 million polygons per frame still at high frame rates.
		/// See forum discussion: http://deltaengine.net/Forum/default.aspx?g=posts&t=1459
		/// </summary>
		private class RenderOneMillionPolygons : DrawBehavior
		{
			public RenderOneMillionPolygons(Drawing drawing, Window window)
			{
				this.drawing = drawing;
				this.window = window;
				logo = new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogoOpaque");
			}

			private readonly Drawing drawing;
			private readonly Window window;
			private readonly Material logo;

			public void Draw(List<DrawableEntity> visibleEntities)
			{
				window.Title = "DrawImagesWithOneMillionPolygonsPerFrame Fps: " + GlobalTime.Current.Fps;
				foreach (DrawableEntity entity in visibleEntities)
				{
					var vertices = entity.Get<VertexPosition2DColorUV[]>();
					var indices = entity.Get<short[]>();
					for (int num = 0; num < 50; num++)
						drawing.Add(logo, vertices, indices);
				}
			}
		}
	}
}