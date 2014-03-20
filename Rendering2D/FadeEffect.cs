using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering2D
{
	//ncrunch: no coverage start
	public class FadeEffect : UpdateBehavior
	{
		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (Sprite sprite in entities.OfType<Sprite>())
			{
				var transitionData = sprite.Get<TransitionData>();
				if (!transitionData.FadeActive)
					continue;
				if(transitionData.FadeReverted)
					FadeReverse(sprite,transitionData);
				else
					FadeForward(sprite, transitionData);
			}
		}

		private static void FadeForward(Sprite entity, TransitionData data)
		{
			data.ElapsedTime += Time.Delta;
			entity.Color = data.Colour.Start.Lerp(data.Colour.End, data.ElapsedTime / data.Duration);
			if (data.ElapsedTime > data.Duration)
			{
				data.FadeReverted = true;
				data.ElapsedTime = data.Duration;
				entity.Material = data.ReplaceMaterialReverting;
			}
			entity.Set(data);
		}

		private static void FadeReverse(Sprite entity, TransitionData data)
		{
			data.ElapsedTime -= Time.Delta;
			entity.Color = data.Colour.Start.Lerp(data.Colour.End, data.ElapsedTime / data.Duration);
			if (data.ElapsedTime < 0)
			{
				data.FadeReverted = false;
				data.ElapsedTime = 0;
				data.FadeActive = false;
			}
			entity.Set(data);
		}

		public class TransitionData
		{
			public RangeGraph<Color> Colour;
			public float Duration;
			public float ElapsedTime;
			public bool FadeReverted;
			public bool FadeActive;
			public Material ReplaceMaterialReverting;
		}
	}
}