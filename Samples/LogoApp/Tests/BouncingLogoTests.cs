using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using NUnit.Framework;

namespace LogoApp.Tests
{
	public class BouncingLogoTests : TestWithMocksOrVisually
	{
		[Test]
		public void Create()
		{
			var logo = new BouncingLogo();
			Assert.IsTrue(logo.Center.X > 0);
			Assert.IsTrue(logo.Center.Y > 0);
			Assert.AreNotEqual(Color.Black, logo.Color);
		}

		[Test]
		public void RunAFewTimesAndCloseGame()
		{
			new BouncingLogo();
			AdvanceTimeAndUpdateEntities(1.0f);
		}

		[Test]
		public void ShowManyLogos()
		{
			for (int i = 0; i < 100; i++)
				new BouncingLogo();
		}

		[Test, CloseAfterFirstFrame, Timeout(5000)]
		public void Drawing100LogosOnlyCauseOneDrawCall()
		{
			for (int i = 0; i < 100; i++)
				new BouncingLogo();
			RunAfterFirstFrame(() =>
			{
				Assert.AreEqual(100, Resolve<Drawing>().NumberOfDynamicDrawCallsThisFrame);
				Assert.AreEqual(100 * 4, Resolve<Drawing>().NumberOfDynamicVerticesDrawnThisFrame);
			});
		}

		[Test]
		public void PressingSpacePausesLogoApp()
		{
			for (int i = 0; i < 10; i++)
				new BouncingLogo();
			new Command(() => Time.IsPaused = !Time.IsPaused).Add(new KeyTrigger(Key.Space));
		}

		[Test]
		public void CallBouncedAction()
		{
			var bouncingLogo = new ActionTesterBouncingLogo();
			Assert.IsFalse(bouncingLogo.BouncedActionCalled);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(bouncingLogo.BouncedActionCalled);
		}

		private class ActionTesterBouncingLogo : BouncingLogo
		{
			public ActionTesterBouncingLogo()
			{
				BouncedActionCalled = false;
				Get<SimplePhysics.Data>().Bounced += () => BouncedActionCalled = true;
				Start<BouncedActionCaller>();
			}

			public bool BouncedActionCalled { get; private set; }

			private class BouncedActionCaller : UpdateBehavior
			{
				public override void Update(IEnumerable<Entity> entities)
				{
					foreach (var entity in entities)
						entity.Get<SimplePhysics.Data>().Bounced();
				}
			}
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void Draw1000LogosToTestPerformance()
		{
			for (int i = 0; i < 1000; i++)
				new BouncingLogo();
		}
	}
}