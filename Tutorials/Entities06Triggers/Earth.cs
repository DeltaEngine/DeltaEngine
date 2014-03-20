using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Tutorials.Entities06Triggers
{
	public class Earth : Sprite, Updateable
	{
		public Earth(Vector2D position, Vector2D velocity)
			: base(ContentLoader.Load<Material>("Earth"), position)
		{
			var data = new SimplePhysics.Data
			{
				Gravity = new Vector2D(0.0f, 0.1f),
				Velocity = velocity
			};
			Add(data);
			Start<SimplePhysics.BounceIfAtScreenEdge>();
			Start<SimplePhysics.Move>();
		}

		public void Update()
		{
			var allEarths = EntitiesRunner.Current.GetEntitiesOfType<Earth>();
			bool isCollidingWithAnotherEarth = false;
			foreach (Earth otherEarth in allEarths)
				if (this != otherEarth && Center.DistanceTo(otherEarth.Center) < Size.Width)
				{
					isCollidingWithAnotherEarth = true;
					break;
				}
			var newColor = isCollidingWithAnotherEarth ? Color.Yellow : Color.White;
			Set(newColor);
		}

		public bool IsPauseable { get { return true; } }
	}
}