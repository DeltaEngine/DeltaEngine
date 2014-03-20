using DeltaEngine.Entities;

namespace DeltaEngine.Tutorials.Entities10LogoRacer
{
	public class EnemySpawner : Entity, Updateable
	{
		public void Update()
		{
			if (!Time.CheckEvery(spawnTime))
				return;
			EnemiesSpawned++;
			if (spawnTime > 0.5f)
				spawnTime -= 0.15f;
			new Enemy();
		}

		private float spawnTime = 2.5f;
		public int EnemiesSpawned { get; private set; }
		public bool IsPauseable { get { return true; } }
	}
}