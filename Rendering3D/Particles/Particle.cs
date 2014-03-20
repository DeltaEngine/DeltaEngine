using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering3D.Particles
{
	public struct Particle : Lerp<Particle>
	{
		public Particle Lerp(Particle other, float interpolation)
		{
			return new Particle
			{
				Position = Position.Lerp(other.Position, interpolation),
				Acceleration = Acceleration.Lerp(other.Acceleration, interpolation),
				Rotation = Rotation.Lerp(other.Rotation, interpolation),
				CurrentMovement = CurrentMovement.Lerp(other.CurrentMovement, interpolation),
				ElapsedTime = ElapsedTime.Lerp(other.ElapsedTime, interpolation),
				Size = Size.Lerp(other.Size, interpolation),
				Color = Color.Lerp(other.Color, interpolation),
				IsActive = IsActive && other.IsActive && ElapsedTime < other.ElapsedTime,
				CurrentUV = other.CurrentUV,
				Material = other.Material,
				CurrentFrame = other.CurrentFrame,
			};
		}

		public Vector3D Position { get; set; }
		public Vector3D Acceleration { get; set; }
		public float Rotation { get; set; }
		public Vector3D CurrentMovement { get; set; }
		public float ElapsedTime { get; set; }
		public Size Size { get; set; }
		public Color Color { get; set; }
		public bool IsActive { get; set; }
		public Rectangle CurrentUV { get; set; }
		public Material Material { get; set; }
		public int CurrentFrame { get; set; }

		//ncrunch: no coverage start
		public VertexPosition3DColorUV[] GetVertices(Size size, Color color)
		{
			float halfWidth = size.Width * 0.5f;
			float halfHeight = size.Height * 0.5f;
			return new[]
			{
				new VertexPosition3DColorUV(GetPosition(-halfWidth, halfHeight), color, Vector2D.Zero),
				new VertexPosition3DColorUV(GetPosition(halfWidth, halfHeight), color, Vector2D.UnitX),
				new VertexPosition3DColorUV(GetPosition(halfWidth, -halfHeight), color, Vector2D.One),
				new VertexPosition3DColorUV(GetPosition(-halfWidth, -halfHeight), color, Vector2D.UnitY)
			};
		}

		private Vector3D GetPosition(float halfWidth, float halfHeight)
		{
			return Rotation == 0
				? new Vector3D(halfWidth, 0, halfHeight)
				: new Vector3D(halfWidth, 0, halfHeight).RotateAround(Vector3D.UnitY, Rotation);
		} //ncrunch: no coverage end

		public void SetStartVelocityRandomizedFromRange(Vector3D startVelocity,
			Vector3D startVelocityVariance)
		{
			CurrentMovement = startVelocity;
			if (startVelocityVariance.X == 0 && startVelocityVariance.Y == 0 &&
				startVelocityVariance.Z == 0)
				return;
			var varianceVector = CalculateVarianceVector(startVelocityVariance);
			CurrentMovement += varianceVector;
		}

		private static Vector3D CalculateVarianceVector(Vector3D startVelocityVariance)
		{
			var xValue = Randomizer.Current.Get(-startVelocityVariance.X, startVelocityVariance.X);
			var yValue = Randomizer.Current.Get(-startVelocityVariance.Y, startVelocityVariance.Y);
			var zValue = Randomizer.Current.Get(-startVelocityVariance.Z, startVelocityVariance.Z);
			var delta = GetDivisionBetweenSquares(startVelocityVariance.X, xValue) +
				GetDivisionBetweenSquares(startVelocityVariance.Y, yValue) +
				GetDivisionBetweenSquares(startVelocityVariance.Z, zValue);
			if (delta - 1 > MathExtensions.Epsilon)
				delta = 1 / delta; //ncrunch: no coverage
			return new Vector3D(xValue * delta, yValue * delta, zValue * delta);
		}

		private static float GetDivisionBetweenSquares(float variance, float xValue)
		{
			return variance != 0 ? (xValue * xValue) / (variance * variance) : 0.0f;
		}

		public bool UpdateIfStillActive(ParticleEmitterData data)
		{
			if (!UpdateParticle(data))
				return false; //ncrunch: no coverage
			Position += CurrentMovement * Time.Delta;
			return true;
		}

		private bool UpdateParticle(ParticleEmitterData data)
		{
			ElapsedTime += Time.Delta;
			if (ElapsedTime > data.LifeTime && data.LifeTime > 0.0f)
				return IsActive = false; //ncrunch: no coverage
			CurrentMovement += Acceleration * Time.Delta;
			Rotation += data.RotationSpeed.GetInterpolatedValue(ElapsedTime / data.LifeTime).
				GetRandomValue() * Time.Delta;
			return true;
		}

		//ncrunch: no coverage start
		public bool UpdateEscapingParticleIfStillActive(ParticleEmitterData data, Vector3D position)
		{
			if (!UpdateParticle(data))
				return false;
			var magnitude = CurrentMovement.Length;
			var vector = Vector3D.Normalize(Position - position);
			Position += magnitude * vector * Time.Delta;
			return true;
		}

		public bool UpdateRoundingParticleIfStillActive(ParticleEmitterData data, Vector3D position)
		{
			if (!UpdateParticle(data))
				return false;
			var formerZ = Position.Z;
			var axis = position.GetVector2D();
			var vector = Position.GetVector2D().RotateAround(axis, CurrentMovement.Length * 10);
			Position = new Vector3D(vector, formerZ);
			return true;
		}

		internal VertexPosition2DColorUV GetTopLeftVertex()
		{
			var topLeft = new Vector2D(Position.X - Size.Width / 2, Position.Y - Size.Height / 2);
			if (Rotation == 0)
				return new VertexPosition2DColorUV(ScreenSpace.Current.ToPixelSpace(topLeft), Color,
					CurrentUV.TopLeft);
			return
				new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpace(topLeft.RotateAround(Position2D, Rotation)), Color,
					CurrentUV.TopLeft);
		}

		private Vector2D Position2D { get {return new Vector2D(Position.X, Position.Y);} }

		internal VertexPosition2DColorUV GetTopRightVertex()
		{
			var topRight = new Vector2D(Position.X + Size.Width / 2, Position.Y - Size.Height / 2);
			if (Rotation == 0)
				return new VertexPosition2DColorUV(ScreenSpace.Current.ToPixelSpace(topRight), Color,
					CurrentUV.TopRight);
			return
				new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpace(topRight.RotateAround(Position2D, Rotation)), Color,
					CurrentUV.TopRight);
		}

		internal VertexPosition2DColorUV GetBottomRightVertex()
		{
			var bottomRight = new Vector2D(Position.X + Size.Width / 2, Position.Y + Size.Height / 2);
			if (Rotation == 0)
				return new VertexPosition2DColorUV(ScreenSpace.Current.ToPixelSpace(bottomRight), Color,
					CurrentUV.BottomRight);
			return
				new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpace(bottomRight.RotateAround(Position2D, Rotation)), Color,
					CurrentUV.BottomRight);
		}

		internal VertexPosition2DColorUV GetBottomLeftVertex()
		{
			var bottomLeft = new Vector2D(Position.X - Size.Width / 2, Position.Y + Size.Height / 2);
			if (Rotation == 0)
				return new VertexPosition2DColorUV(ScreenSpace.Current.ToPixelSpace(bottomLeft), Color,
					CurrentUV.BottomLeft);
			return
				new VertexPosition2DColorUV(
					ScreenSpace.Current.ToPixelSpace(bottomLeft.RotateAround(Position2D, Rotation)), Color,
					CurrentUV.BottomLeft);
		}
	}
}