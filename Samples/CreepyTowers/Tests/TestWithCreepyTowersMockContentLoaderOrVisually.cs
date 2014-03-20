using DeltaEngine.Content;
using DeltaEngine.Extensions;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class TestWithCreepyTowersMockContentLoaderOrVisually : TestWithMocksOrVisually
	{
		[SetUp]
		public void InitializeCreepyTowersMockContentLoader()
		{
			if (StackTraceExtensions.ForceUseOfMockResolver())
				ContentLoader.Use<CreepyTowersMockContentLoader>();
		}
	}
}