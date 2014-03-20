using System;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D.Fonts;
using NUnit.Framework;

namespace CreepyTowers.Tests.Sounds
{
	public class SoundTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			new FontText(Font.Default, "Press Space to play",
				Rectangle.FromCenter(Vector2D.Half, new Size(0.5f, 0.3f))) { RenderLayer = 20 };
			new GameCamera(1 / 5.0f);
		}

		[TestCase("MenuMusic"), TestCase("GameMusic")]
		public void PlayMusic(string name)
		{
			AttachActionToSpaceButton(PlayMusic);
			fileName = name;
		}

		private static void AttachActionToSpaceButton(Action action)
		{
			new Command(action).Add(new KeyTrigger(Key.Space, State.Pressed));
		}

		private string fileName;

		private void PlayMusic()
		{
			var music = ContentLoader.Load<Music>(fileName);
			music.Loop = false;
			music.Play();
		}

		[TestCase("MenuButtonClick"), TestCase("PauseOpen"), TestCase("PauseClose"),
		 TestCase("Resume"), TestCase("LowHealth"), TestCase("CreepExits"),
		 TestCase("DragonAuraCannon"), TestCase("DragonBreathOfFire")]
		public void PlayMenuButtonClickSound(string name)
		{
			AttachActionToSpaceButton(PlaySound);
			fileName = name;
		}

		private void PlaySound()
		{
			var sound = ContentLoader.Load<Sound>(fileName);
			sound.Play();
		}

		[TestCase(TowerType.Fire), TestCase(TowerType.Water), TestCase(TowerType.Ice),
		 TestCase(TowerType.Acid), TestCase(TowerType.Impact), TestCase(TowerType.Slice)]
		public void BuildTowerAndPlayTowerBuiltSound(TowerType type)
		{
			towerType = type;
			AttachActionToSpaceButton(ShowTowerAndPlaySound);
			fileName = "Tower" + Enum.Parse(typeof(TowerType), towerType.ToString()) + "Built";
		}

		private TowerType towerType;

		private void ShowTowerAndPlaySound()
		{
			var tower = new Tower(towerType, Vector3D.Zero, 180);
			tower.RenderModel();
			PlaySound();
		}

		[TestCase(TowerType.Fire), TestCase(TowerType.Water), TestCase(TowerType.Ice),
		 TestCase(TowerType.Acid), TestCase(TowerType.Impact), TestCase(TowerType.Slice)]
		public void BuildTowerAndPlayAttackSound(TowerType type)
		{
			towerType = type;
			AttachActionToSpaceButton(ShowTowerAndPlaySound);
			fileName = "Tower" + Enum.Parse(typeof(TowerType), towerType.ToString()) + "Attack";
		}

		[TestCase(TowerType.Fire), TestCase(TowerType.Water), TestCase(TowerType.Ice),
		 TestCase(TowerType.Acid), TestCase(TowerType.Impact), TestCase(TowerType.Slice)]
		public void BuildTowerAndPlayUpgradeSound(TowerType type)
		{
			towerType = type;
			AttachActionToSpaceButton(ShowTowerAndPlaySound);
			fileName = "Tower" + Enum.Parse(typeof(TowerType), towerType.ToString()) + "Upgrade";
		}

		[TestCase(TowerType.Fire), TestCase(TowerType.Water), TestCase(TowerType.Ice),
		 TestCase(TowerType.Acid), TestCase(TowerType.Impact), TestCase(TowerType.Slice)]
		public void BuildTowerAndPlaySoldSound(TowerType type)
		{
			towerType = type;
			fileName = "Tower" + Enum.Parse(typeof(TowerType), towerType.ToString()) + "Sold";
			AttachActionToSpaceButton(ShowTowerAndPlaySound);
		}

		[Test]
		public void PlayTowerCantBeBuiltHereSound()
		{
			AttachActionToSpaceButton(PlaySound);
			fileName = "TowerCantBuildHere";
		}

		[Test]
		public void PlaySandCreepTransformsToGlassSound()
		{
			new Player();
			creep = new Creep(CreepType.Sand, Vector3D.Zero, 180);
			creep.RenderModel();
			AttachActionToSpaceButton(TranformSandToGlass);
			fileName = "CreepTransformToGlass";
		}

		private Creep creep;

		private void TranformSandToGlass()
		{
			creep.Dispose();
			new Creep(CreepType.Glass, Vector3D.Zero, 180).RenderModel();
			PlaySound();
		}
	}
}