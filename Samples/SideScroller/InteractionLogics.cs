using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D.Shapes;

namespace SideScroller
{
	public class InteractionLogics : Entity
	{
		public void FireShotByPlayer(Vector2D startPosition)
		{
			var bulletTrail = new Line2D(startPosition, new Vector2D(1, startPosition.Y), Color.Orange)
			{
				RenderLayer = (int)DefRenderLayer.Player - 1
			};
			bulletTrail.Add(new Duration(0.2f)).Start<SelfDestructTimer>();
		}

		public void FireShotByEnemy(Vector2D startPosition)
		{
			var bulletTrail = new Line2D(startPosition, new Vector2D(0, startPosition.Y), Color.Red);
			bulletTrail.Add(new Duration(0.1f)).Start<SelfDestructTimer>();
		}

		private class SelfDestructTimer : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var duration = entity.Get<Duration>();
					duration.Elapsed += Time.Delta;
					if (duration.Elapsed > duration.Value)
						entity.IsActive = false;
					entity.Set(duration);
				}
			}
		}

		internal struct Duration
		{
			public Duration(float duration)
				: this()
			{
				Value = duration;
			}

			public float Value { get; private set; }
			public float Elapsed { get; internal set; }
		}
	}
}