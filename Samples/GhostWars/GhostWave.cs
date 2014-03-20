using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D;

namespace GhostWars
{
	/// <summary>
	/// Allows to send small groups of ghosts (1-5) from start to target in waves.
	/// </summary>
	public class GhostWave : Entity, Updateable
	{
		public GhostWave(Vector2D start, Vector2D target, int waveSize, Color color)
		{
			this.start = start;
			this.target = target;
			sprites = new Sprite[waveSize];
			var ghostMaterial = ContentLoader.Load<Material>("GhostMaterial");
			for (int num = 0; num < waveSize; num++)
				sprites[num] = CreateSpriteWithOrientation(ghostMaterial, color);
			UpdatePriority = Priority.Low;
		}

		private readonly Vector2D start;
		private readonly Vector2D target;
		private readonly Sprite[] sprites;

		private Sprite CreateSpriteWithOrientation(Material ghostMaterial, Color color)
		{
			var newSprite = new Sprite(ghostMaterial, start) { RenderLayer = 1 };
			newSprite.Color = color;
			if (GameLogic.GhostSize != 1.0f)
				newSprite.Size *= GameLogic.GhostSize;
			if (start.X > target.X)
				newSprite.FlipMode = FlipMode.Horizontal;
			return newSprite;
		}

		public object Attacker { get; set; }

		public void Update()
		{
			runTime += Time.Delta;
			for (int i = 0; i < sprites.Length; i++)
				UpdateDrawAreaAndRotation(i);
			if (ReachedTarget)
				Dispose();
		}

		private float runTime;

		private void UpdateDrawAreaAndRotation(int num)
		{
			sprites[num].DrawArea = CurrentDrawArea(num);
			sprites[num].Rotation = CurrentRotation(num);
		}

		private Rectangle CurrentDrawArea(int num)
		{
			var direction = Vector2D.Normalize(target - start);
			var pos = start + direction * runTime * Speed;
			var vertical = direction.RotateAround(Vector2D.Zero, 90);
			pos += vertical * MathExtensions.Sin(runTime * 300) * 0.0035f;
			pos += vertical * MathExtensions.Sin(runTime * 44 + num * 27) * 0.0135f;
			pos += vertical * SpreadDistance * CalcDistanceFromCenter(num, 1.0f, 90, 90);
			return Rectangle.FromCenter(pos, GameLogic.GhostSize * sprites[0].Material.MaterialRenderSize);
		}

		private const float SpreadDistance = 0.06f;
		private const float Speed = 0.08f;

		private float CalcDistanceFromCenter(int num, float initialDistanceValue, float startSin,
			float targetSin)
		{
			var goalTime = start.DistanceTo(target) / Speed;
			float distanceFromCenter = initialDistanceValue;
			float increaseTime = Math.Min(3, goalTime / 2);
			if (runTime < increaseTime)
				distanceFromCenter = MathExtensions.Sin(startSin * runTime / increaseTime);
			else if (goalTime - runTime < increaseTime)
				distanceFromCenter = MathExtensions.Sin(targetSin * (goalTime - runTime) / increaseTime);
			var normalizedNum = (num - (sprites.Length - 1) / 2.0f) / (sprites.Length / 2.0f);
			return distanceFromCenter * normalizedNum;
		}

		private float CurrentRotation(int num)
		{
			var distanceFromCenter = CalcDistanceFromCenter(num, 0.0f, 180, -180);
			return start.RotationTo(target) + distanceFromCenter * 30 +
				(start.RotationTo(target).Abs() < 90 ? 0 : 180);
		}

		protected bool ReachedTarget
		{
			get { return runTime * Speed > start.DistanceTo(target); }
		}

		public override void Dispose()
		{
			if (TargetReached != null)
				TargetReached(Attacker, sprites.Length);
			TargetReached = null;
			foreach (Sprite sprite in sprites)
				sprite.Dispose();
			base.Dispose();
		}

		public Action<object, int> TargetReached;
	}
}