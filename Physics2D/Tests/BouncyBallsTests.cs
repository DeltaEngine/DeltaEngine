using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Physics2D.Farseer;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	public class BouncyBallsTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			physics = new FarseerPhysics();
			mouse = Resolve<Mouse>();
		}

		private Physics physics;
		private Mouse mouse;

		[Test]
		public void SimpleBouncingBalls()
		{
			Rectangle viewport = ScreenSpace.Current.Viewport;
			physics.CreateEdge(viewport.BottomLeft, viewport.BottomRight);
			physics.CreateEdge(viewport.BottomLeft, viewport.TopLeft);
			physics.CreateEdge(viewport.TopRight, viewport.BottomRight);
			physics.CreateEdge(viewport.TopLeft, viewport.TopRight);
			CreateThreeBouncyBalls();
			new Command(OnPressing).Add(new MouseButtonTrigger(State.Pressing));
			new Command(OnPressed).Add(new MouseButtonTrigger(State.Pressed));
			new Command(OnReleasing).Add(new MouseButtonTrigger(State.Releasing));
		}

		PhysicsBody grabbedBall;
		Vector2D lastMousePosition = Vector2D.Zero;

		private void CreateThreeBouncyBalls()
		{
			balls = new PhysicsBody[3];
			balls[0] = CreateBouncyBall(0.05f);
			balls[0].Position = new Vector2D(0.5f, 0.5f);
			balls[0].LinearVelocity = new Vector2D(0.1f, 0.1f);
			balls[1] = CreateBouncyBall(0.03f);
			balls[1].Position = new Vector2D(0.3f, 0.4f);
			balls[1].LinearVelocity = new Vector2D(0.02f, 0.15f);
			balls[2] = CreateBouncyBall(0.04f);
			balls[2].Position = new Vector2D(0.7f, 0.4f);
			balls[2].LinearVelocity = new Vector2D(0.1f, 0.1f);
		}

		private PhysicsBody[] balls;

		private PhysicsBody CreateBouncyBall(float radius)
		{
			PhysicsBody circle = physics.CreateCircle(radius);
			circle.Restitution = 1f;
			circle.Friction = 0.1f;
			new Ellipse(Vector2D.Zero, radius, radius, Color.GetRandomBrightColor()).AffixToPhysics(circle);
			return circle;
		}

		//ncrunch: no coverage start
		private void OnPressing()
		{
			foreach (var ball in balls)
				if (ball.Position.DistanceTo(mouse.Position) < 0.05f)
				{
					grabbedBall = ball;
					break;
				}
		}

		private void OnPressed()
		{
			if (grabbedBall != null)
			{
				lastMousePosition = grabbedBall.Position;
				grabbedBall.Position = mouse.Position;
			}
			else
				foreach (var ball in balls)
					ball.LinearVelocity = ball.Position.DirectionTo(mouse.Position) * 3f;
		}

		private void OnReleasing()
		{
			if (grabbedBall != null)
				grabbedBall.LinearVelocity = lastMousePosition.DirectionTo(grabbedBall.Position) * 4f;
			grabbedBall = null;
		}
	}
}