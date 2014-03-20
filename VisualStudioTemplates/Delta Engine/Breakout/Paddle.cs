using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;

namespace $safeprojectname$
{
	/// <summary>
	/// Holds the paddle position
	/// </summary>
	public class Paddle : Sprite
	{
		public Paddle()
			: base(new Material(ShaderFlags.Position2DColoredTextured, "Paddle"), Rectangle.One)
		{
			RegisterInputCommands();
			Start<RunPaddle>();
			RenderLayer = 5;
		}

		private void RegisterInputCommands()
		{
			RegisterButtonCommands();
			new Command(DoMovementByMouseClick).Add(
				new MouseButtonTrigger(MouseButton.Left, State.Pressed));
		}

		private void DoMovementByMouseClick(Vector2D clickPosition)
		{
			var moveDistAbsolute = PaddleMovementSpeed * Time.Delta;
			var distance = clickPosition.X - xPosition;
			if (distance.Abs() < moveDistAbsolute)
			{
				xPosition = clickPosition.X;
				return;
			}
			xPosition += distance > 0 ? moveDistAbsolute : -moveDistAbsolute;
		}

		private void RegisterButtonCommands()
		{
			var left = new Command(() => xPosition -= PaddleMovementSpeed * Time.Delta);
			left.Add(new KeyTrigger(Key.CursorLeft, State.Pressed));
			left.Add(new GamePadButtonTrigger(GamePadButton.Left, State.Pressed));
			var right = new Command(() => xPosition += PaddleMovementSpeed * Time.Delta);
			right.Add(new KeyTrigger(Key.CursorRight, State.Pressed));
			right.Add(new GamePadButtonTrigger(GamePadButton.Right, State.Pressed));
		}

		private float xPosition = 0.5f;
		private const float PaddleMovementSpeed = 1.5f;

		public class RunPaddle : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var paddle = (Paddle)entity;
					var xPosition = paddle.xPosition.Clamp(HalfWidth, 1.0f - HalfWidth);
					paddle.xPosition = xPosition;
					paddle.DrawArea = Rectangle.FromCenter(xPosition, YPosition, Width, Height);
				}
			}
		}

		private const float YPosition = 0.9f;
		internal const float HalfWidth = Width / 2.0f;
		private const float Width = 0.2f;
		private const float Height = 0.04f;

		public Vector2D Position
		{
			get { return new Vector2D(DrawArea.Center.X, DrawArea.Top); }
		}
	}
}