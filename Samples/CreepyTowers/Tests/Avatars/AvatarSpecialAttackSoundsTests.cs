using CreepyTowers.Avatars;
using CreepyTowers.Content;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace CreepyTowers.Tests.Avatars
{
	public class AvatarSpecialAttackSoundsTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			drawArea = Rectangle.FromCenter(Vector2D.Half, new Size(0.1f));
			ContentLoader.Load<XmlContent>("BuffProperties");
		}

		private Rectangle drawArea;

		[TestCase(GameHud.DragonAttackBreathMat), TestCase(GameHud.DragonAttackCannonMat),
		 TestCase(GameHud.PenguinAttackBigFireworkMat),
		 TestCase(GameHud.PenguinAttackCarpetBombingMat), TestCase(GameHud.PiggyAttackCoinRainMat),
		 TestCase(GameHud.PiggyAttackPaydayMat), CloseAfterFirstFrame]
		public void TestDragonBreathOfFireAttackSound(GameHud material)
		{
			var buttonSpecialAttack = new InteractiveButton(CreateTheme(material),
				drawArea);
			AvatarSpecialAttackSoundSelector.PlaySpecialAttackSound(buttonSpecialAttack);
		}

		private static Theme CreateTheme(GameHud buttonMaterial)
		{
			var material = ContentLoader.Load<Material>(buttonMaterial.ToString());
			return new Theme
			{
				Button = material,
				ButtonMouseover = material,
				ButtonDisabled = material,
				ButtonPressed = material,
			};
		}
	}
}