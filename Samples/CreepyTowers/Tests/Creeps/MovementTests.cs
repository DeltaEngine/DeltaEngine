using System.Collections.Generic;
using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class MovementTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			creep = new Creep(CreepType.Cloth, Vector2D.Zero)
			{
				Path = new List<Vector2D> { new Vector2D(1, 0), new Vector2D(2, 0) },
				FinalTarget = new Vector2D(2, 0)
			};
		}

		private Creep creep;

		[Test, CloseAfterFirstFrame]
		public void ParalysedCreepDoesntMove()
		{
			creep.State.Paralysed = true;
			AdvanceTimeAndUpdateEntities(1);
			Assert.AreEqual(Vector3D.Zero, creep.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void FrozenCreepDoesntMove()
		{
			creep.State.Frozen = true;
			AdvanceTimeAndUpdateEntities(1);
			Assert.AreEqual(Vector3D.Zero, creep.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void DelayedCreepMovesAtThirdSpeed()
		{
			creep.State.Delayed = true;
			AdvanceTimeAndUpdateEntities(1);
			Assert.AreEqual(0.333f, creep.Position.X, 0.001f);
		}

		[Test, CloseAfterFirstFrame]
		public void SlowCreepMovesAtHalfSpeed()
		{
			creep.State.Slow = true;
			AdvanceTimeAndUpdateEntities(1);
			Assert.AreEqual(0.5f, creep.Position.X, 0.001f);
		}

		[Test, CloseAfterFirstFrame]
		public void FastCreepMovesAtDoubleSpeed()
		{
			creep.State.Fast = true;
			AdvanceTimeAndUpdateEntities(1);
			Assert.AreEqual(2.0f, creep.Position.X, 0.001f);
		}

		[Test, CloseAfterFirstFrame]
		public void UnchangedCreepMovesAtDefaultSpeed()
		{
			AdvanceTimeAndUpdateEntities(1);
			Assert.AreEqual(1.0f, creep.Position.X, 0.001f);
		}

		[Test, CloseAfterFirstFrame]
		public void CreepWithNoPathDoesntMove()
		{
			creep.Path = new List<Vector2D>();
			AdvanceTimeAndUpdateEntities(1);
			Assert.AreEqual(0.0f, creep.Position.X);
		}
	}
}