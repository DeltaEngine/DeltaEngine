using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.GameLogic
{
	//ncrunch: no coverage start
	public class CollisionTrigger : UpdateBehavior
	{
		public CollisionTrigger()
			: base(Priority.High) {}

		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (var entity in entities)
			{
				if (!(entity is Actor))
					continue;
				var data = entity.Get<Data>();
				var foundEntities = GetEntitiesFromSearchTags(data);
				bool isColliding = false;
				foreach (var otherEntity in foundEntities.OfType<Actor>())
					if (entity != otherEntity && !isColliding)
						isColliding = (entity as Actor).IsColliding(otherEntity);
				entity.Set(isColliding ? data.TriggeredColor : data.DefaultColor);
			}
		}

		private static IEnumerable<Entity> GetEntitiesFromSearchTags(Data data)
		{
			var foundEntities = new List<Entity>();
			foreach (var tag in data.SearchTags)
				foreach (var foundEntity in EntitiesRunner.Current.GetEntitiesWithTag(tag))
					foundEntities.Add(foundEntity);
			return foundEntities;
		}

		public class Data
		{
			public Data(List<string> searchTags, Color triggeredColor, Color defaultColor)
			{
				SearchTags = searchTags;
				TriggeredColor = triggeredColor;
				DefaultColor = defaultColor;
			}

			public Data(Color triggeredColor, Color defaultColor) :
				this(new List<string>(), triggeredColor, defaultColor) {}

			public List<string> SearchTags { get; private set; }
			public Color TriggeredColor { get; private set; }
			public Color DefaultColor { get; private set; }
		}
	}
}