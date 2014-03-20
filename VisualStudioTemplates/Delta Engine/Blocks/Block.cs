using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;

namespace $safeprojectname$
{
	/// <summary>
	/// Holds the individual bricks making up a block and handles rotating them
	/// </summary>
	public class Block : Entity
	{
		public Block(Orientation displayMode, BlocksContent content, Vector2D topLeft)
		{
			this.content = content;
			CreateBricks();
			Left = topLeft.X;
			Top = topLeft.Y;
			this.displayMode = displayMode;
		}

		private readonly BlocksContent content;

		private void CreateBricks()
		{
			int numberOfBricks = content.AreFiveBrickBlocksAllowed
				? GetNumberOfBricks() : NormalNumberOfBricks;
			var material = new Material(ShaderFlags.Position2DColoredTextured,
				content.Prefix + "Block" + Randomizer.Current.Get(1, 8));
			var newBrick = new Brick(material, Vector2D.Zero, displayMode);
			Bricks = new List<Brick> { newBrick };
			for (int i = 1; i < numberOfBricks; i++)
				AddBrick(Bricks[i - 1], material);
			ShiftToTopLeft();
		}

		private const int NormalNumberOfBricks = 4;
		public List<Brick> Bricks { get; private set; }

		private static int GetNumberOfBricks()
		{
			return Randomizer.Current.Get() < 0.9f ? NormalNumberOfBricks : NormalNumberOfBricks + 1;
		}

		private void AddBrick(Brick lastBrick, Material material)
		{
			Brick newBrick;
			bool anyBrickAtNewOffset;
			do
			{
				newBrick = new Brick(material, lastBrick.Offset + GetRandomOffset(), displayMode)
				{
					IsActive = false
				};
				anyBrickAtNewOffset = false;
				foreach (Brick brick in Bricks)
					if (brick.Offset == newBrick.Offset)
					{
						anyBrickAtNewOffset = true;
						break;
					}
			} while (anyBrickAtNewOffset);
			Bricks.Add(newBrick);
			newBrick.IsActive = true;
		}

		private static Vector2D GetRandomOffset()
		{
			return Randomizer.Current.Get(0, 2) == 0
				? new Vector2D(Randomizer.Current.Get(0, 2) * 2 - 1, 0)
				: new Vector2D(0, Randomizer.Current.Get(0, 2) * 2 - 1);
		}

		private void ShiftToTopLeft()
		{
			float left = float.MaxValue;
			float top = float.MaxValue;
			foreach (var brick in Bricks)
			{
				if (brick.Offset.X < left)
					left = brick.Offset.X;
				if (brick.Offset.Y < top)
					top = brick.Offset.Y;
			}
			foreach (Brick brick in Bricks)
				brick.Offset = new Vector2D(brick.Offset.X - left, brick.Offset.Y - top);
			UpdateCenter();
		}

		private void UpdateCenter()
		{
			float minX = float.MaxValue;
			float maxX = float.MinValue;
			float minY = float.MaxValue;
			float maxY = float.MinValue;
			foreach (var brick in Bricks)
			{
				if (brick.Offset.X < minX)
					minX = brick.Offset.X;
				if (brick.Offset.Y < minY)
					minY = brick.Offset.Y;
				if (brick.Offset.X > maxX)
					maxX = brick.Offset.X;
				if (brick.Offset.Y > maxY)
					maxY = brick.Offset.Y;
			}
			center = new Vector2D((minX + maxX + 1) / 2, (minY + maxY + 1) / 2);
		}

		private Vector2D center;

		public Vector2D Center
		{
			get { return center; }
		}

		public float Left
		{
			get { return Bricks[0].TopLeftGridCoord.X; }
			set
			{
				foreach (Brick brick in Bricks)
					brick.TopLeftGridCoord.X = value;
			}
		}

		public float Top
		{
			get { return Bricks[0].TopLeftGridCoord.Y; }
			set
			{
				foreach (Brick brick in Bricks)
					brick.TopLeftGridCoord.Y = value;
			}
		}

		private readonly Orientation displayMode;

		public void RotateClockwise()
		{
			Vector2D oldCenter = center;
			foreach (Brick brick in Bricks)
				brick.Offset = new Vector2D(-brick.Offset.Y, brick.Offset.X);
			ShiftToTopLeft();
			Left += (int)oldCenter.X - (int)center.X;
		}

		public void RotateAntiClockwise()
		{
			Vector2D oldCenter = center;
			foreach (Brick brick in Bricks)
				brick.Offset = new Vector2D(brick.Offset.Y, -brick.Offset.X);
			ShiftToTopLeft();
			Left += (int)oldCenter.X - (int)center.X;
		}

		public void UpdateBrickDrawAreas(float fallSpeed)
		{
			Top += MathExtensions.Min(fallSpeed * Time.Delta, 1.0f);
			foreach (var brick in Bricks)
				brick.UpdateDrawArea();
		}

		/// <summary>
		/// For debugging purposes the current brick can be displayed with ASCII characters.
		/// </summary>
		public override string ToString()
		{
			string result = "";
			for (int y = 0; y < Bricks.Count; y++)
				result += LineToString(y);
			return result;
		}

		private string LineToString(int y)
		{
			string line = y > 0 ? "/" : "";
			for (int x = 0; x < Bricks.Count; x++)
			{
				bool any = false;
				foreach (Brick brick in Bricks)
					if (brick.Offset == new Vector2D(x, y))
					{
						any = true;
						break;
					}
				line += any ? "O" : ".";
			}
			return line;
		}

		public override bool IsActive
		{
			get
			{
				return base.IsActive;
			}
			set
			{
				if (!value && IsActive)
				{
					foreach (var brick in Bricks)
					{
						brick.Dispose();
					}
				}
				base.IsActive = value;
			}
		}
	}
}