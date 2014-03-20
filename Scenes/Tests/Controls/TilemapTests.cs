using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class TilemapTests : TestWithMocksOrVisually
	{
		[Test, Category("Slow")] //ncrunch: no coverage start
		public void RenderColoredLogoTilemap()
		{
			CreateBorder();
			var tilemap = new ColoredLogoTilemap(World, Map) { DrawArea = Center };
			tilemap.Add(new FontText(Font.Default, "", new Rectangle(0.3f, 0.6f, 0.2f, 0.2f))
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				RenderLayer = 2
			});
			tilemap.Start<RenderDragInfo>();
		}

		private static readonly Size World = new Size(100, 100);
		private static readonly Size Map = new Size(10, 10);
		private static readonly Rectangle Center = new Rectangle(Left, Top, Width, Height);
		private const float Left = 0.3f;
		private const float Top = 0.3f;
		private const float Width = 0.4f;
		private const float Height = 0.4f;

		private static void CreateBorder()
		{
			new FilledRect(
				new Rectangle(Left - 2 * TileWidth, Top - 2 * TileHeight, BorderWidth, 2 * TileHeight),
				Color.Black) { RenderLayer = 1 };
			new FilledRect(
				new Rectangle(Left - 2 * TileWidth, Top - 2 * TileHeight, 2 * TileWidth, BorderHeight),
				Color.Black) { RenderLayer = 1 };
			new FilledRect(
				new Rectangle(Left - 2 * TileWidth, Top + (Map.Height - 1) * TileHeight, BorderWidth,
					2 * TileHeight), Color.Black) { RenderLayer = 1 };
			new FilledRect(
				new Rectangle(Left + (Map.Width - 1) * TileWidth, Top - 2 * TileHeight, 2 * TileWidth,
					BorderHeight), Color.Black) { RenderLayer = 1 };
		}

		private static readonly float TileWidth = Center.Width / Map.Width;
		private static readonly float TileHeight = Center.Height / Map.Height;
		private static readonly float BorderWidth = (Map.Width + 3) * TileWidth;
		private static readonly float BorderHeight = (Map.Height + 3) * TileHeight;

		private class RenderDragInfo : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Tilemap tileMap in entities)
					tileMap.Get<FontText>().Text = "Delta: " + tileMap.State.DragDelta + "\nDragStart:" +
						tileMap.State.DragStart + "\nDragEnd:" + tileMap.State.DragEnd;
			}
		} //ncrunch: no coverage end

		[Test]
		public void TestWithMouseDrag()
		{
			var tilemap = new ColoredLogoTilemap(World, Map);
			tilemap.State.DragStart = new Vector2D(0.5f, 0.5f);
			tilemap.State.DragEnd = new Vector2D(0.7f, 0.9f);
			tilemap.Update();
			Assert.IsTrue(tilemap.State.DragDelta.IsNearlyEqual(new Vector2D(0.2f, 0.4f)));
		}

		[Test]
		public void TestWithMockKeyboard()
		{
			new ColoredLogoTilemap(World, Map);
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.A, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			keyboard.SetKeyboardState(Key.D, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			keyboard.SetKeyboardState(Key.W, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			keyboard.SetKeyboardState(Key.S, State.Pressed);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void TestUpdate()
		{
			var tilemap = new ColoredLogoTilemap(World, Map);
			tilemap.DrawArea = Rectangle.One;
			tilemap.State.DragStart = new Vector2D(10.5f, 10.5f);
			tilemap.State.DragEnd = new Vector2D(0.5f, 0.5f);
			tilemap.Update();
			Assert.AreEqual(new Vector2D(-10, -10), tilemap.State.DragDelta);
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void MoveToLeft()
		{
			new ColoredLogoTilemap(World, Map);
			if (!IsMockResolver)
				return;
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.D, State.Pressed);
			AdvanceTimeAndUpdateEntities(1);
		}

		[Test, Category("Slow")]
		public void TestBorderIntersection()
		{
			new ColoredLogoTilemap(new Size(1), new Size(1));
			if (!IsMockResolver)
				return;
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.D, State.Pressed);
			AdvanceTimeAndUpdateEntities(1);
			keyboard.SetKeyboardState(Key.S, State.Pressed);
			AdvanceTimeAndUpdateEntities(1);
		} // ncrunch: no coverage end
	}
}