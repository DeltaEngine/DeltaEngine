using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D.Particles;

namespace Blocks
{
	/// <summary>
	/// Represents a grid of bricks: the blocks that have come to rest and not yet been removed
	/// </summary>
	public class Grid
	{
		public Grid(BlocksContent content)
		{
			this.content = content;
			zoomBrickData = new ParticleEmitterData
			{
				LifeTime = 0.2f,
				Color = new RangeGraph<Color>(Color.White, Color.TransparentWhite),
				MaximumNumberOfParticles = 10,
				SpawnInterval = -1,
				StartVelocity = new RangeGraph<Vector3D>(Vector2D.Zero, Vector2D.Zero),
				StartPosition = new RangeGraph<Vector3D>(Vector2D.Zero, Vector2D.Zero),
			};
		}

		private readonly BlocksContent content;
		private readonly ParticleEmitterData zoomBrickData;

		public int AffixBlock(Block block)
		{
			foreach (Brick brick in block.Bricks.Where(brick => !IsOccupied(brick)))
				AffixBrick(brick);
			RemoveFilledRows();
			return removedRows;
		}

		private bool IsOccupied(Brick brick)
		{
			return bricks[(int)brick.Position.X, (int)brick.Position.Y] != null;
		}

		internal readonly Brick[,] bricks = new Brick[Width,Height];

		private void AffixBrick(Brick brick)
		{
			brick.TopLeftGridCoord = new Vector2D((int)brick.TopLeftGridCoord.X,
				(int)brick.TopLeftGridCoord.Y + 1);
			bricks[(int)brick.Position.X, (int)brick.Position.Y - 1] = brick;
			brick.UpdateDrawArea();
			AddZoomingBrick(brick);
		}

		internal const int Width = 12;
		internal const int Height = 19;

		private void RemoveFilledRows()
		{
			removedRows = 0;
			for (int y = 0; y < Height; y++)
				if (IsRowFilled(y))
					RemoveRow(y); //ncrunch: no coverage
		}

		private int removedRows;

		private bool IsRowFilled(int y)
		{
			for (int x = 0; x < Width; x++)
				if (bricks[x, y] == null)
					return false;
			return true; //ncrunch: no coverage
		}

		//ncrunch: no coverage start
		private void RemoveRow(int row)
		{
			for (int x = 0; x < Width; x++)
				RemoveBrick(x, row);

			for (int x = 0; x < Width; x++)
				for (int y = row; y > 0; y--)
					MoveBrickDown(x, y);

			removedRows++;
		} //ncrunch: no coverage end

		private void RemoveBrick(int x, int y)
		{
			var brick = bricks[x, y];
			brick.Dispose();
			bricks[x, y] = null;
			if (content.DoBricksSplitInHalfWhenRowFull)
				AddPairOfFallingBricks(brick); //ncrunch: no coverage
			else
				AddFallingBrick(brick, brick.Material);
		}

		//ncrunch: no coverage start
		private static void AddPairOfFallingBricks(Brick brick)
		{
			AddTopFallingBrick(brick);
			AddBottomFallingBrick(brick);
		}
		
		private static void AddTopFallingBrick(Sprite brick)
		{
			var material = new Material(ShaderFlags.Position2DColoredTextured,
				brick.Material.DiffuseMap.Name + "_Top");
			AddFallingBrick(brick, material);
		}

		private static void AddBottomFallingBrick(Sprite brick)
		{
			var material = new Material(ShaderFlags.Position2DColoredTextured,
				brick.Material.DiffuseMap.Name + "_Bottom");
			AddFallingBrick(brick, material);
		} //ncrunch: no coverage end

		private static void AddFallingBrick(Entity2D brick, Material material)
		{
			var fallingBrick = new Sprite(material, brick.DrawArea)
			{
				Color = brick.Color,
				RenderLayer = (int)BlocksRenderLayer.FallingBrick,
			};
			var random = Randomizer.Current;
			var data = new SimplePhysics.Data
			{
				Velocity = new Vector2D(random.Get(-0.5f, 0.5f), random.Get(-1.0f, 0.0f)),
				RotationSpeed = random.Get(-360, 360),
				Duration = 5.0f,
				Gravity = new Vector2D(0.0f, 2.0f)
			};
			fallingBrick.Add(data);
			fallingBrick.Start<SimplePhysics.Move>();
		}

		//ncrunch: no coverage start
		private void MoveBrickDown(int x, int y)
		{
			bricks[x, y] = bricks[x, y - 1];
			if (bricks[x, y] == null)
				return;
			bricks[x, y].TopLeftGridCoord.Y++;
			bricks[x, y].UpdateDrawArea();
		} //ncrunch: no coverage end

		private void AddZoomingBrick(Sprite brick)
		{
			zoomBrickData.ParticleMaterial = brick.Material;
			zoomBrickData.Size = new RangeGraph<Size>(brick.Size, brick.Size * 2);
			var zoomBrickEmitter = new ParticleEmitter(zoomBrickData, brick.Center);
			zoomBrickEmitter.RenderLayer = 16;
			zoomBrickEmitter.SpawnAndDispose();
		}

		public bool IsValidPosition(Block block)
		{
			foreach (Brick brick in block.Bricks)
				if (IsOutsideTheGrid(brick) || IsOccupied(brick))
					return false;
			return true;
		}

		private static bool IsOutsideTheGrid(Brick brick)
		{
			return brick.Position.X < 0 || brick.Position.X >= Width || brick.Position.Y < 1 ||
				brick.Position.Y >= Height;
		}

		public List<int> GetValidStartingColumns(Block block)
		{
			block.Top = 1;
			List<int> validStartingColumns = content.DoBlocksStartInARandomColumn
				? GetAllValidStartingColumns(block) : GetMiddleColumnIfValid(block);
			return validStartingColumns;
		}

		//ncrunch: no coverage start
		private List<int> GetAllValidStartingColumns(Block block)
		{
			var validStartingColumns = new List<int>();
			for (int x = 0; x < Width; x++)
				if (IsAValidStartingColumn(block, x))
					validStartingColumns.Add(x);
			return validStartingColumns;
		} //ncrunch: no coverage end

		private bool IsAValidStartingColumn(Block block, int column)
		{
			block.Left = column;
			return IsValidPosition(block);
		}

		private List<int> GetMiddleColumnIfValid(Block block)
		{
			var validStartingColumns = new List<int>();
			if (IsAValidStartingColumn(block, Middle))
				validStartingColumns.Add(Middle - (int)block.Center.X);

			return validStartingColumns;
		}

		private const int Middle = Width / 2;

		public bool IsABrickOnFirstRow()
		{
			for (int x = 0; x < Width; x++)
				if (bricks[x, 0] != null)
					return true;
			return false;
		}

		public void Clear()
		{
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					if (bricks[x, y] != null)
						RemoveBrick(x, y);
			var remainingBricks = EntitiesRunner.Current.GetEntitiesOfType<Brick>();
			foreach (var brick in remainingBricks)
				brick.Dispose();
		}
	}
}