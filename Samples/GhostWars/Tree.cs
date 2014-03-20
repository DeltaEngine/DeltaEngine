using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;

namespace GhostWars
{
	public class Tree : Sprite, Updateable
	{
		public Tree(Vector2D position, Team team)
			: base(new Material(ShaderFlags.Position2DColoredTextured, TreeImageName[0]), position)
		{
			Level = 1;
			CurrentTeam = team;
			NumberText = new FontText(MainMenu.Font, "",
				Rectangle.FromCenter(position + NumberTextPositionPerLevel[0], new Size(0.1f)))
			{
				RenderLayer = 4
			};
			NumberOfGhosts = team == Team.None ? 0 : 25;
			RenderLayer = 3;
			UpdateImage();
		}

		private static readonly string[] TreeImageName = { "Tree1", "Tree2", "Tree3" };
		public int Level { get; set; }
		public Team CurrentTeam { get; private set; }

		public override bool IsActive
		{
			get
			{
				return base.IsActive;
			}
			set
			{
				NumberText.IsActive = value;
				base.IsActive = value;
			}
		}

		public int NumberOfGhosts
		{
			get { return numberOfGhosts; }
			set
			{
				numberOfGhosts = value;
				if (numberOfGhosts > Level * GameLogic.GhostsToUpgrade)
					numberOfGhosts = Level * GameLogic.GhostsToUpgrade;
				NumberText.Color = CurrentTeam.ToColor();
				NumberText.Text = numberOfGhosts + "";
			}
		}

		private int numberOfGhosts;
		public readonly FontText NumberText;
		private static readonly Vector2D[] NumberTextPositionPerLevel =
		{
			new Vector2D(0.002f, -0.0495f), new Vector2D(0.002f, -0.0775f),
			new Vector2D(0.0015f, -0.0875f)
		};

		public bool IsAi
		{
			get { return CurrentTeam == Team.ComputerPurple || CurrentTeam == Team.ComputerTeal; }
		}

		public void Update()
		{
			if (CurrentTeam == Team.None || MainMenu.State != GameState.Game ||
				!Time.CheckEvery(Level == 1 ? 1.5f : Level == 2 ? 1.0f : 0.75f))
				return;
			NumberOfGhosts++;
			Effects.CreateSparkleEffect(CurrentTeam, Center +
				new Vector2D(Randomizer.Current.Get(-0.04f, 0.04f), Randomizer.Current.Get(-0.04f, 0.04f)),
				1);
		}

		private void UpdateImage()
		{
			Material = new Material(ShaderFlags.Position2DColoredTextured, TreeImageName[Level - 1]);
			Color = CurrentTeam.ToColor();
			DrawArea =
				Rectangle.FromCenter(Center, GameLogic.TreeSize * Material.DiffuseMap.PixelSize / 1920.0f);
			NumberText.Center = Center + NumberTextPositionPerLevel[Level - 1];
		}

		public void Attack(Team attackerTeam, int numberOfAttackerGhosts)
		{
			if (CurrentTeam == Team.None)
				GrabEmptyTree(attackerTeam);
			else if (CurrentTeam == attackerTeam)
				MoveToOwnTree(numberOfAttackerGhosts);
			else
				AttackEnemyTree(attackerTeam, numberOfAttackerGhosts);
		}

		private void GrabEmptyTree(Team attackerTeam)
		{
			CurrentTeam = attackerTeam;
			if (attackerTeam == Team.HumanYellow)
				ContentLoader.Load<Sound>("WinningTree").Play();
			UpdateImage();
			Effects.CreateHitEffect(Center);
		}

		private void MoveToOwnTree(int numberOfAttackerGhosts)
		{
			NumberOfGhosts += numberOfAttackerGhosts;
			if (CurrentTeam == Team.HumanYellow)
				ContentLoader.Load<Sound>("GhostWaveStart").Play(0.7f);
			Effects.CreateSparkleEffect(CurrentTeam, Center +
				new Vector2D(Randomizer.Current.Get(-0.04f, 0.04f), Randomizer.Current.Get(-0.04f, 0.04f)),
				numberOfAttackerGhosts);
		}

		private void AttackEnemyTree(Team attackerTeam, int numberOfAttackerGhosts)
		{
			ShowAttackEffects(numberOfAttackerGhosts);
			NumberOfGhosts -= DeterminateAttackerGhostsLeft(numberOfAttackerGhosts);
			if (NumberOfGhosts < 0)
			{
				if (attackerTeam == Team.HumanYellow)
					ContentLoader.Load<Sound>("WinningTree").Play();
				else if (CurrentTeam == Team.HumanYellow)
					ContentLoader.Load<Sound>("LosingTree").Play();
				CurrentTeam = attackerTeam;
				UpdateImage();
				NumberOfGhosts = 0;
			}
			else
				ContentLoader.Load<Sound>("MalletHit").Play(0.5f);
		}

		private void ShowAttackEffects(int numberOfAttackerGhosts)
		{
			for (int num = 0; num < numberOfAttackerGhosts; num++)
				Effects.CreateDeathEffect(Center +
					new Vector2D(Randomizer.Current.Get(-0.03f, 0.03f),
						Randomizer.Current.Get(-0.03f, 0.03f)));
			Effects.CreateHitEffect(Center);
		}

		private int DeterminateAttackerGhostsLeft(int numberOfAttackerGhosts)
		{
			if (Time.Total - lastAttackerTime > 1)
				numberOfAttackerGhosts--;
			lastAttackerTime = Time.Total;
			if (NumberOfGhosts > 20)
				numberOfAttackerGhosts--;
			if (NumberOfGhosts >= 60)
				numberOfAttackerGhosts--;
			return numberOfAttackerGhosts;
		}

		private float lastAttackerTime;

		public void TryToUpgrade()
		{
			if (Level > 2 || NumberOfGhosts < Level * GameLogic.GhostsToUpgrade ||
				CurrentTeam != Team.HumanYellow)
				return;
			NumberOfGhosts -= Level * GameLogic.GhostsToUpgrade;
			Level++;
			UpdateImage();
			ContentLoader.Load<Sound>("Upgrading").Play();
			Effects.CreateSparkleEffect(CurrentTeam, Center +
				new Vector2D(Randomizer.Current.Get(-0.04f, 0.04f), Randomizer.Current.Get(-0.04f, 0.04f)),
				8);
		}
	}
}