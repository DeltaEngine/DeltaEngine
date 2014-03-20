using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D;

namespace $safeprojectname$
{
	/// <summary>
	/// Handle our car and the simple physics we need for driving around the track.
	/// </summary>
	public sealed class Car : Sprite, Updateable
	{
		public Car(Vector2D startPosition, Track setTrack)
			: base("CarImage", new Rectangle(startPosition, new Size(0.075f, 0.05f)))
		{
			track = setTrack;
			velocity = new Vector2D(1f, 0f);
			collisionRadius = DrawArea.Width / 2;
			NextTrackIndex = 0;
			new Command(Command.MoveLeft,
				() => velocity = velocity.Rotate(-speed * RotationStep * Time.Delta));
			new Command(Command.MoveRight,
				() => velocity = velocity.Rotate(speed * RotationStep * Time.Delta));
			new Command(Command.MoveUp, () => speed += Time.Delta * 2);
		}

		private readonly Track track;
		private Vector2D velocity;
		private readonly float collisionRadius;
		public int NextTrackIndex { get; private set; }
		private float speed;
		private const float RotationStep = 100.0f;

		public void Update()
		{
			ReduceSpeedAutomaticallyAndLimitIt();
			CalculateNextTrackIndex();
			velocity = CalculateVelocity();
			Center += (velocity * speed) * Time.Delta * 0.2f;
			float oldRotation = Rotation;
			Rotation = velocity.GetRotation();
			if ((oldRotation - Rotation).Abs() > 50)
				SetWithoutInterpolation(Rotation);
		}

		private void ReduceSpeedAutomaticallyAndLimitIt()
		{
			speed -= Time.Delta;
			speed = speed.Clamp(0f, MaxSpeed);
		}

		private const float MaxSpeed = 1.75f;

		private void CalculateNextTrackIndex()
		{
			float distanceToNext = (Center - track.Positions[NextTrackIndex]).LengthSquared;
			if (distanceToNext <= lastDistanceToNext)
				return;
			NextTrackIndex++;
			NextTrackIndex %= track.Positions.Length;
			lastDistanceToNext = (Center - track.Positions[NextTrackIndex]).LengthSquared;
		}

		private float lastDistanceToNext;

		private Vector2D CalculateVelocity()
		{
			Vector2D carBackward = Center - (velocity * collisionRadius);
			Vector2D carForward = Center + (velocity * collisionRadius);
			for (int index = 0; index < track.Positions.Length - 1; index++)
			{
				Vector2D inner1 = track.TrackInnerBounds[index];
				Vector2D inner2 = track.TrackInnerBounds[index + 1];
				if (MathExtensions.IsLineIntersectingWith(carBackward, carForward, inner1, inner2))
					return GetVelocityAfterTrackLineCollision(inner1, inner2);
				Vector2D outer1 = track.TrackOuterBounds[index];
				Vector2D outer2 = track.TrackOuterBounds[index + 1];
				if (MathExtensions.IsLineIntersectingWith(carBackward, carForward, outer1, outer2))
					return GetVelocityAfterTrackLineCollision(outer1, outer2);
			}
			return velocity;
		}

		private Vector2D GetVelocityAfterTrackLineCollision(Vector2D inner1, Vector2D inner2)
		{
			SlowDownIfAboveOneThirdOfMaxSpeed();
			return Vector2D.Normalize(inner2 - inner1);
		}

		private void SlowDownIfAboveOneThirdOfMaxSpeed()
		{
			if (speed > MaxSpeed / 3)
				speed /= 2.0f;
		}
	}
}