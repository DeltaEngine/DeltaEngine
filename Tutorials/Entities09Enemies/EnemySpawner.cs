using DeltaEngine.Entities;

namespace DeltaEngine.Tutorials.Entities09Enemies
{
	public class EnemySpawner : Entity, Updateable
	{
		public void Update()
		{
			if (Time.CheckEvery(2.5f))
				new Enemy();
		}

		public bool IsPauseable { get { return true; } }
	}
}