using System.Collections.Generic;
using System.Linq;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Levels;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class GameLevelTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			player = new Player();
			new GameCamera(1 / 25.0f, MaxZoom, 0.01f);
			gameLevel = ContentLoader.Load<GameLevel>("DummyLevelInfo");
		}

		private const float MaxZoom = 1 / 10.0f;
		private GameLevel gameLevel;
		private Player player;

		[Test]
		public void DisplayCreepWalkingInALevel()
		{
			gameLevel.ModelName = "N1C1ChildsRoom";
			gameLevel.RenderLevel();
			var creep = new Creep(CreepType.Cloth, new Vector2D(9, 5) + Vector2D.Half);
			creep.Path = new List<Vector2D>
			{
				(new Vector2D(9, 12) + Vector2D.Half),
				(new Vector2D(13, 12) + Vector2D.Half),
				(new Vector2D(13, 6) + Vector2D.Half),
				(new Vector2D(9, 6) + Vector2D.Half)
			};
			creep.FinalTarget = creep.Path[creep.Path.Count - 1];
		}

		[Test]
		public void DisplayCreepWalkingViaPathfinding()
		{
			var creep = new Creep(CreepType.Cloth, gameLevel.SpawnPoints[0]);
			var path =
				gameLevel.GetPath(gameLevel.SpawnPoints[0], gameLevel.GoalPoints[0]).GetListOfCoordinates();
			creep.Path = path.Select(element => element + Vector2D.Half).ToList();
		}

		//ncrunch: no coverage start
		[Test, CloseAfterFirstFrame, Ignore]
		public void RestartLevel()
		{
			Assert.Less(player.Gold, 4000);
			gameLevel.Restart();
			Assert.AreEqual(4000, player.Gold);
		}
	}
}