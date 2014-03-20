using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class CalculateDamageTests : CreepyTowersGameForTests
	{
		[Test, CloseAfterFirstFrame]
		public void CheckResistanceBasedOnImmuneType()
		{
			var creep = new Creep(CreepType.Glass, Vector3D.Zero);
			var resistance = creep.State.GetVulnerabilityValue(TowerType.Acid);
			Assert.AreEqual(0.1f, resistance);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckResistanceBasedOnVulnerableType()
		{
			var creep = new Creep(CreepType.Glass, Vector3D.Zero);
			var resistance = creep.State.GetVulnerabilityValue(TowerType.Impact);
			Assert.AreEqual(3.0f, resistance);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckResistanceBasedOnResistantType()
		{
			var creep = new Creep(CreepType.Sand, Vector3D.Zero);
			var resistance = creep.State.GetVulnerabilityValue(TowerType.Impact);
			Assert.AreEqual(0.5f, resistance);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckResistanceBasedOnHardboiledType()
		{
			var creep = new Creep(CreepType.Cloth, Vector3D.Zero);
			var resistance = creep.State.GetVulnerabilityValue(TowerType.Ice);
			Assert.AreEqual(0.25f, resistance);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckResistanceBasedOnWeakType()
		{
			var creep = new Creep(CreepType.Cloth, Vector3D.Zero);
			var resistance = creep.State.GetVulnerabilityValue(TowerType.Slice);
			Assert.AreEqual(2.0f, resistance);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckResistanceBasedOnNormalDamageType()
		{
			var creep = new Creep(CreepType.Cloth, Vector3D.Zero);
			var resistance = creep.State.GetVulnerabilityValue(TowerType.Water);
			Assert.AreEqual(1.0f, resistance);
		}
	}
}