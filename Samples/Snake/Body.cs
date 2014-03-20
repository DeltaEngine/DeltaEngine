using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Shapes;

namespace Snake
{
	public class Body
	{
		public Body(int gridSize, Color color)
		{
			this.gridSize = gridSize;
			blockSize = 1.0f / gridSize;
			this.color = color;
			SpawnSnake();
		}

		private readonly int gridSize;
		private readonly float blockSize;
		private Color color;

		private void SpawnSnake()
		{
			BodyParts = new List<FilledRect>();
			Direction = new Vector2D(0, -blockSize);
			PlaceSnakeHead();
			AddSnakeBody();
		}

		public List<FilledRect> BodyParts { get; private set; }
		public Vector2D Direction { get; set; }
		public Vector2D HeadPosition
		{
			get
			{
				if (BodyParts.Count == 0)
					return Vector2D.Half;
				return BodyParts[0].Center;
			}
		}

		private void PlaceSnakeHead()
		{
			var startPosition = blockSize * (float)Math.Floor(gridSize / 2.0f);
			var firstPart = new FilledRect(CalculateHeadDrawArea(startPosition), color) {RenderLayer = 3};
			BodyParts.Add(firstPart);
		}

		private Rectangle CalculateHeadDrawArea(float startPosition)
		{
			return new Rectangle(new Vector2D(startPosition, startPosition), new Size(1.0f / gridSize));
		}

		public void AddSnakeBody()
		{
			var snakeHead = BodyParts[BodyParts.Count - 1].DrawArea;
			var newTail = new FilledRect(CalculateBodyDrawArea(snakeHead), color){RenderLayer = 3};
			BodyParts.Add(newTail);
		}

		private Rectangle CalculateBodyDrawArea(Rectangle snakeHead)
		{
			return new Rectangle(snakeHead.Left, snakeHead.Top + blockSize, blockSize, blockSize);
		}

		internal void MoveBody()
		{
			trailingVector = GetTrailingVector();
			MoveBodyTowardsHead();
			MoveHeadInDesiredDirection();
		}

		private Vector2D trailingVector;

		public Vector2D GetTrailingVector()
		{
			var tail = BodyParts[BodyParts.Count - 1].DrawArea.TopLeft;
			var partBeforeTail = BodyParts[BodyParts.Count - 2].DrawArea.TopLeft;
			return new Vector2D(tail.X - partBeforeTail.X, tail.Y - partBeforeTail.Y);
		}

		private void MoveBodyTowardsHead()
		{
			for (int count = BodyParts.Count - 1; count >= 1; count--)
				BodyParts[count].DrawArea = BodyParts[count - 1].DrawArea;
		}

		private void MoveHeadInDesiredDirection()
		{
			var newHeadPos = new Vector2D(BodyParts[0].DrawArea.Left + Direction.X,
				BodyParts[0].DrawArea.Top + Direction.Y);
			BodyParts[0].DrawArea = new Rectangle(newHeadPos, new Size(blockSize));
		}

		internal void CheckSnakeCollidesWithChunk()
		{
			if (DetectSnakeCollisionWithChunk != null)
				DetectSnakeCollisionWithChunk(trailingVector);
		}

		public event Action<Vector2D> DetectSnakeCollisionWithChunk;

		internal void CheckSnakeCollisionWithBorderOrItself()
		{
			if (SnakeCollidesWithBorderOrItself == null)
				return; //ncrunch: no coverage
			var snakeHead = BodyParts[0];
			if (SnakeCollidesWithItself(snakeHead) || SnakeCollidesWithBorders(snakeHead))
				SnakeCollidesWithBorderOrItself();
		}

		private bool SnakeCollidesWithItself(FilledRect snakeHead)
		{
			for (int count = 3; count < BodyParts.Count; count++)
				if (snakeHead.DrawArea.TopLeft == BodyParts[count].DrawArea.TopLeft)
					return true;
			return false;
		}

		private bool SnakeCollidesWithBorders(FilledRect snakeHead)
		{
			if ((snakeHead.DrawArea.Left < blockSize - 0.01f ||
				snakeHead.DrawArea.Top < blockSize - 0.01f ||
				snakeHead.DrawArea.Left > 1 - blockSize - 0.01f ||
				snakeHead.DrawArea.Top > 1 - blockSize - 0.01f))
				return true;
			return false;
		}

		public event Action SnakeCollidesWithBorderOrItself;
	}
}