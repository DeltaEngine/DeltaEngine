using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Tutorials.Entities10LogoRacer
{
	public class ScoreDisplay : FontText, Updateable
	{
		public ScoreDisplay(Player player, EnemySpawner spawner)
			: base(Font.Default, "Score: ", Rectangle.FromCenter(0.5f, 0.25f, 0.2f, 0.1f))
		{
			this.player = player;
			this.spawner = spawner;
			RenderLayer = 1;
		}

		private readonly Player player;
		private readonly EnemySpawner spawner;

		public void Update()
		{
			if (Text.StartsWith("Game Over"))
				return;
			if (player.Color == Color.White)
				Text = "Score: " + spawner.EnemiesSpawned;
			else
				Text = "Game Over! " + Text;
		}

		public bool IsPauseable { get { return false; } }
	}
}