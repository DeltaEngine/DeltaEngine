using DeltaEngine.Core;
using Drench.Logics;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Drench.Tests.Logics
{
	public class PlayerVsSmartAiLogicTests
	{
		[SetUp]
		public void SetUp()
		{
			Randomizer.Use(new FixedRandom(new[] { 0.1f, 0.6f, 0.7f, 0.2f }));
			var logic = new HumanVsSmartAiLogic(BoardTests.Width, BoardTests.Height);
			humanVsAiLogicTests = new HumanVsAiLogicTests(logic);
		}

		private HumanVsAiLogicTests humanVsAiLogicTests;

		[Test]
		public void AiMove()
		{
			humanVsAiLogicTests.AiMove();
		}

		[Test]
		public void PlayUntilFinished()
		{
			humanVsAiLogicTests.PlayUntilFinished();
		}

		[Test]
		public void WhenPlayerPassesAiPlays()
		{
			humanVsAiLogicTests.WhenPlayerPassesAiPlays();
		}
	}
}