using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class CreepDyingSoundSelectorTests : TestWithMocksOrVisually
	{
		[TestCase(CreepType.Cloth), TestCase(CreepType.Glass), TestCase(CreepType.Iron),
		 TestCase(CreepType.Paper), TestCase(CreepType.Plastic), TestCase(CreepType.Sand),
		 TestCase(CreepType.Wood), CloseAfterFirstFrame, Category("Slow")]
		public void PlayClothCreepDestroyedSound(CreepType type)
		{
			CreepDyingSoundSelector.PlayDestroyedSound(type);
		}
	}
}