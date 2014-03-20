using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.GameLogic
{
	public class TimeTrigger : UpdateBehavior
	{
		public TimeTrigger()
			: base(Priority.High) {}

		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (var entity in entities)
			{
				var data = entity.Get<Data>();
				if (!Time.CheckEvery(data.Interval))
					continue;
				entity.Set(entity.Get<Color>() == data.FirstColor ? data.SecondColor : data.FirstColor);
			}
		}

		public class Data
		{
			public Data(Color firstColor, Color secondColor, float interval)
			{
				FirstColor = firstColor;
				SecondColor = secondColor;
				Interval = interval;
			}

			public Color FirstColor;
			public Color SecondColor;
			public float Interval;
		}
	}
}