using CreepyTowers.Effects;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic.PathFinding;
using DeltaEngine.Rendering3D.Particles;

namespace CreepyTowers
{
	internal class PathMarker : Entity, Updateable
	{
		public PathMarker(ReturnedPath path)
		{
			Path = path;
			effect = EffectLoader.GetTrailMarkerEffect();
		}

		public ReturnedPath Path { get; set; }

		private readonly ParticleSystem effect;

		public void Update()
		{
			if (Time.CheckEvery(0.4f))
			{
				var nextPosition = GetNextPathPosition(lastPosition);
				effect.FireBurstAtRelativePositionAndRotation(nextPosition, GetRotation());
				lastPosition = nextPosition;
			}
		}

		private Vector2D lastPosition;

		private Vector2D GetNextPathPosition(Vector2D formerPosition)
		{
			var nextPosition = formerPosition +
				formerPosition.DirectionTo(Path.Path[nextPathIndex].Position);
			if (nextPosition == Path.Path[nextPathIndex].Position)
				nextPathIndex++;
			if (nextPathIndex >= Path.Path.Count)
			{
				nextPathIndex = 1;
				nextPosition = Path.Path[0].Position;
			}
			lastPosition = nextPosition;
			return nextPosition + Vector2D.Half;
		}

		private float GetRotation()
		{
			return lastPosition.DirectionTo(Path.Path[nextPathIndex].Position).GetRotation() + 90.0f;
		}

		private int nextPathIndex;
	}
}