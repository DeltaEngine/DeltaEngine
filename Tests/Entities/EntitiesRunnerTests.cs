using System;
using System.Collections.Generic;
using DeltaEngine.Entities;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Entities
{
	public class EntitiesRunnerTests
	{
		[SetUp]
		public void CreateSystem()
		{
			entities = new MockEntitiesRunner(typeof(MockUpdateBehavior), typeof(IncrementCounter),
				typeof(DerivedBehavior), typeof(DrawTest), typeof(LowPriorityBehavior),
				typeof(AddNewUpdateBehaviorTwice), typeof(MockNonPauseableUpdateBehavior));
		}

		private MockEntitiesRunner entities;

		public class DerivedBehavior : MockUpdateBehavior {}

		[Test]
		public void EntityIsCreatedActiveAndAutomaticallyAddedToEntitySystem()
		{
			var entity = new MockEntity();
			Assert.IsTrue(entity.IsActive);
			Assert.AreEqual(1, EntitiesRunner.Current.NumberOfEntities);
		}

		[Test]
		public void TestExceptions()
		{
			var settings = new MockSettings { UpdatesPerSecond = 0 };
			Assert.Throws<EntitiesRunner.InvalidUpdatePerSecondSettings>(
				() => new EntitiesRunner(new MockBehaviorResolver(), settings));
			Assert.Throws<EntitiesRunner.InvalidUpdatePerSecondSettings>(
				() => EntitiesRunner.Current.ChangeUpdateTimeStep(0));
			Assert.Throws<EntitiesRunner.YouAreNotAllowedToDrawOutsideOfTheDrawLoop>(
				() => EntitiesRunner.Current.CheckIfInDrawState());
			Assert.Throws<EntitiesRunner.UnableToResolveBehavior>(
				() => EntitiesRunner.Current.GetDrawBehavior<DrawBehavior>());
		}

		public class MockBehaviorResolver : BehaviorResolver
		{
			public UpdateBehavior ResolveUpdateBehavior(Type behaviorType)
			{
				return null;
			}

			public DrawBehavior ResolveDrawBehavior(Type behaviorType)
			{
				return null;
			}
		}

		[Test]
		public void MockBehaviorResolversUpdateAndDrawAreNull()
		{
			Assert.AreEqual(null,
				new MockBehaviorResolver().ResolveUpdateBehavior(typeof(DerivedBehavior)));
			Assert.AreEqual(null,
				new MockBehaviorResolver().ResolveDrawBehavior(typeof(DerivedBehavior)));
		}

		[Test]
		public void TestUpdates()
		{
			const float NewTime = 2.0f;
			EntitiesRunner.Current.ChangeUpdateTimeStep(NewTime);
			Assert.AreEqual(NewTime, Time.Delta);
		}

		[Test]
		public void InactivateEntity()
		{
			var entity = new MockEntity();
			entity.IsActive = false;
			Assert.IsFalse(entity.IsActive);
			Assert.AreEqual(0, EntitiesRunner.Current.NumberOfEntities);
		}

		[Test]
		public void ActivateEntity()
		{
			var entity = new MockEntity { IsActive = false };
			entity.IsActive = true;
			Assert.AreEqual(1, EntitiesRunner.Current.NumberOfEntities);
		}

		[Test]
		public void ClearEntities()
		{
			new MockEntity();
			new MockEntity().Start<MockUpdateBehavior>();
			entities.RunEntities();
			Assert.AreEqual(2, EntitiesRunner.Current.NumberOfEntities);
			EntitiesRunner.Current.Clear();
			Assert.AreEqual(0, EntitiesRunner.Current.NumberOfEntities);
		}

		[Test]
		public void CallingGetUpdateBehaviorAgainReturnsACachedCopy()
		{
			var behavior = EntitiesRunner.Current.GetUpdateBehavior<MockUpdateBehavior>();
			Assert.AreEqual(behavior, EntitiesRunner.Current.GetUpdateBehavior<MockUpdateBehavior>());
		}

		[Test]
		public void CanCheckEntityHandlersInformation()
		{
			var behavior = EntitiesRunner.Current.GetUpdateBehavior<MockUpdateBehavior>();
			Assert.AreEqual(Priority.Normal, behavior.priority);
		}

		[Test]
		public void CallingGetUnresolvableHandlerFails()
		{
			Assert.Throws<EntitiesRunner.UnableToResolveBehavior>(
				() => EntitiesRunner.Current.GetUpdateBehavior<UpdateBehavior>());
		}

		[Test]
		public void AddingTheSameEntityTwiceIsNotOk()
		{
			var entity1 = new MockEntity();
			var entity2 = new MockEntity().Start<MockUpdateBehavior>();
			Assert.Throws<EntitiesRunner.EntityAlreadyAdded>(() => EntitiesRunner.Current.Add(entity1));
			Assert.Throws<EntitiesRunner.EntityAlreadyAdded>(() => EntitiesRunner.Current.Add(entity2));
			var entity3 = new MockEntity();
			Assert.Throws<EntitiesRunner.EntityAlreadyAdded>(() => EntitiesRunner.Current.Add(entity3));
		}

		[Test]
		public void AddingBehaviorTwiceIsIgnored()
		{
			var entity = new MockEntity().Start<IncrementCounter>().Add(0);
			var behavior = EntitiesRunner.Current.GetUpdateBehavior<IncrementCounter>();
			entities.RunEntities();
			Assert.AreEqual(1, entity.Get<int>());
			entity.Start<IncrementCounter>();
			Assert.AreEqual(behavior, EntitiesRunner.Current.GetUpdateBehavior<IncrementCounter>());
			entities.RunEntities();
			Assert.AreEqual(2, entity.Get<int>());
			Assert.AreEqual(1, EntitiesRunner.Current.NumberOfEntities);
		}

		public class IncrementCounter : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
					entity.Set(entity.Get<int>() + 1);
			}
		}

		[Test]
		public void AddEntityAndAttachHandlerLater()
		{
			var entity = new MockEntity().Add(0);
			entities.RunEntities();
			Assert.AreEqual(0, entity.Get<int>());
			entity.Start<IncrementCounter>();
			entities.RunEntities();
			Assert.AreEqual(1, entity.Get<int>());
			entity.Stop<IncrementCounter>();
			entities.RunEntities();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void AddingAndRemovingTheSameHandlerDoesNothing()
		{
			var entity = new MockEntity().Add(0);
			entity.Start<IncrementCounter>();
			entity.Stop<IncrementCounter>();
			entities.RunEntities();
			Assert.AreEqual(0, entity.Get<int>());
		}

		[Test]
		public void InactiveEntityDoesntRunBehavior()
		{
			var entity = new MockEntity();
			entity.Start<IncrementCounter>().Add(0);
			entity.IsActive = false;
			entities.RunEntities();
			Assert.AreEqual(0, entity.Get<int>());
		}

		[Test]
		public void ActiveEntityRunsBehavior()
		{
			var entity = new MockEntity();
			entity.Start<IncrementCounter>().Add(0);
			entities.RunEntities();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void ReactivatedEntityRunsBehavior()
		{
			var entity = new MockEntity();
			entity.Start<IncrementCounter>().Add(0);
			entity.IsActive = false;
			entity.IsActive = true;
			entities.RunEntities();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void TestIfComponentIsRemovedWhenEntityIsRemoved()
		{
			var entity = new MockEntity().Start<IncrementCounter>();
			entity.IsActive = false;
			entities.RunEntities();
			Assert.AreEqual(0, entities.NumberOfEntities);
		}

		[Test]
		public void GetAllEntitiesWithCertainTag()
		{
			new MockEntity().AddTag("test1");
			new MockEntity().AddTag("test1");
			Assert.AreEqual(0, EntitiesRunner.Current.GetEntitiesWithTag("abc").Count);
			Assert.AreEqual(2, EntitiesRunner.Current.GetEntitiesWithTag("test1").Count);
		}

		[Test]
		public void GetAllEntitiesWithHandlersThatHaveATag()
		{
			new MockEntity().Start<IncrementCounter>().AddTag("abc");
			new MockEntity().Start<IncrementCounter>().AddTag("abc");
			Assert.AreEqual(2, EntitiesRunner.Current.GetEntitiesWithTag("abc").Count);
		}

		[Test]
		public void GetAllEntitiesOfCertainType()
		{
			new MockEntity();
			new MockEntity();
			Assert.AreEqual(2, EntitiesRunner.Current.GetEntitiesOfType<MockEntity>().Count);
		}

		[Test]
		public void GetAllEntities()
		{
			new MockEntity();
			new MockEntity();
			Assert.AreEqual(2, EntitiesRunner.Current.GetAllEntities().Count);
		}

		[Test]
		public void ResolvesCorrectEntityHandler()
		{
			new MockEntity().Start<MockUpdateBehavior>();
			var behavior = EntitiesRunner.Current.GetUpdateBehavior<MockUpdateBehavior>();
			Assert.IsTrue(behavior.GetType() == typeof(MockUpdateBehavior));
		}

		[Test]
		public void WhileUpdatingEntityAddANewUpdateBehaviorShouldForceGettingTheCachedVersionBack()
		{
			new MockEntity().Start<AddNewUpdateBehaviorTwice>();
			entities.RunEntities();
		}

		public class AddNewUpdateBehaviorTwice : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
					entity.Start<IncrementCounter>().Start<IncrementCounter>();
			}
		}

		[Test]
		public void RemovingHandlerWhenNeverAddedExecutesNoCode()
		{
			var entity = new MockEntity();
			entities.RunEntities();
			entity.Stop<IncrementCounter>();
			entities.RunEntities();
			Assert.IsFalse(entity.Contains<string>());
		}

		[Test]
		public void AddingAndRemovingHandlerInTheSameFrameExecutesNoCode()
		{
			var entity = new MockEntity().Start<IncrementCounter>();
			entity.Stop<IncrementCounter>();
			entities.RunEntities();
			Assert.IsFalse(entity.Contains<string>());
		}

		[Test]
		public void RemoveUpdateAndDrawBehaviorWhenRemovingAnEntity()
		{
			var entity = new MockDrawableEntity();
			entity.Start<IncrementCounter>();
			entity.OnDraw<DrawTest>();
			entity.IsActive = false;
			entities.RunEntities();
		}

		[Test]
		public void RemoveUpdateAndDrawBehaviorWhenRemovingAnEntityWithDifferentRenderLayers()
		{
			var entity = new MockDrawableEntity();
			entity.Start<IncrementCounter>();
			entity.Set(2);
			entity.OnDraw<DrawTest>();
			entity.IsActive = false;
			var entity2 = new MockDrawableEntity();
			entity2.Start<IncrementCounter>();
			entity2.Set(-5);
			entity2.OnDraw<DrawTest>();
			entity2.IsActive = false;
			entities.RunEntities();
		}

		public class DrawTest : DrawBehavior
		{
			public void Draw(List<DrawableEntity> visibleEntities) { } //ncrunch: no coverage
		}
		
		[Test]
		public void PrioritiesAreHandledCorrectlyForAnUpdateableEntityWithExtraBehaviors()
		{
			var entity = new UpdateableEntityWithBehaviors();
			Assert.IsTrue(entity.IsPauseable);
			entities.RunEntities();
			entity.Set("");
			entities.RunEntities();
			Assert.AreEqual("[HighPriorityUpdate][LowPriorityBehavior]", entity.Get<string>());
		}

		private class UpdateableEntityWithBehaviors : Entity, Updateable
		{
			public UpdateableEntityWithBehaviors()
			{
				Start<LowPriorityBehavior>();
				UpdatePriority = Priority.High;
			}

			public void Update()
			{
				Set(GetOrDefault("") + "[HighPriorityUpdate]");
			}
		}

		private class LowPriorityBehavior : UpdateBehavior
		{
			public LowPriorityBehavior()
				: base(Priority.Low) {}

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Entity entity in entities)
					entity.Set(entity.GetOrDefault("") + "[LowPriorityBehavior]");
			}
		}

		[Test]
		public void BothUpdateAndBehaviorAreRunOnTheFirstRunLoop()
		{
			var entity = new UpdateableEntityWithBehaviors();
			entities.RunEntities();
			Assert.AreEqual("[HighPriorityUpdate][LowPriorityBehavior]", entity.Get<string>());
		}

		[Test]
		public void RapidEntityPausesWhenAppIsPaused()
		{
			VerifyEntityWasUpdated(new MockRapidEntity(), () => entities.RunEntities());
			VerifyEntityWasNotUpdated(new MockRapidEntity(), () => entities.RunEntitiesPaused());
		}

		// ReSharper disable UnusedParameter.Local
		private static void VerifyEntityWasUpdated(VerifiableUpdate entity, Action run)
		{
			run();
			Assert.IsTrue(entity.WasUpdated);
		}

		private static void VerifyEntityWasNotUpdated(VerifiableUpdate entity, Action run)
		{
			run();
			Assert.IsFalse(entity.WasUpdated);
		}

		[Test]
		public void NonPausableRapidEntityAlwaysRuns()
		{
			VerifyEntityWasUpdated(new MockNonPauseableRapidEntity(), () => entities.RunEntities());
			VerifyEntityWasUpdated(new MockNonPauseableRapidEntity(),
				() => entities.RunEntitiesPaused());
		}

		[Test]
		public void UpdateableEntityPausesWhenAppIsPaused()
		{
			VerifyEntityWasUpdated(new MockEntity(), () => entities.RunEntities());
			VerifyEntityWasNotUpdated(new MockEntity(), () => entities.RunEntitiesPaused());
		}

		[Test]
		public void NonPausableEntityAlwaysRuns()
		{
			VerifyEntityWasUpdated(new MockNonPauseableEntity(), () => entities.RunEntities());
			VerifyEntityWasUpdated(new MockNonPauseableEntity(), () => entities.RunEntitiesPaused());
		}

		[Test]
		public void UpdateBehaviorPausesWhenAppIsPaused()
		{
			VerifyEntityWasUpdated((VerifiableUpdate)new MockEntity().Start<MockUpdateBehavior>(),
				() => entities.RunEntities());
			VerifyEntityWasNotUpdated((VerifiableUpdate)new MockEntity().Start<MockUpdateBehavior>(),
				() => entities.RunEntitiesPaused());
		}

		[Test]
		public void NonPauseableUpdateBehaviorAlwaysRuns()
		{
			VerifyEntityWasUpdated(
				(VerifiableUpdate)new MockEntity().Start<MockNonPauseableUpdateBehavior>(),
				() => entities.RunEntities());
			var entity = new MockEntity().Start<MockNonPauseableUpdateBehavior>();
			VerifyEntityWasUpdated(
				(VerifiableUpdate)entity,
				() => entities.RunEntitiesPaused());
		}

		[Test]
		public void AddVisibleDrawableEntity()
		{
			var drawable = new DrawableEntity { IsVisible = false };
			drawable.OnDraw<DrawTest>();
			drawable.ToggleVisibility();
			Assert.IsTrue(drawable.IsVisible);
		}
	}
}