using System;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Mocks;
using DeltaEngine.Tests.Content;
using NUnit.Framework;

namespace DeltaEngine.Tests.Commands
{
	public class CommandTests
	{
		[SetUp]
		public void InitializeResolver()
		{
			ContentLoader.Use<FakeContentLoader>();
			entities = new MockEntitiesRunner(typeof(MockUpdateBehavior));
		}

		private MockEntitiesRunner entities;

		[TearDown]
		public void RunTestAndDisposeResolverWhenDone()
		{
			entities.Dispose();
			ContentLoader.DisposeIfInitialized();
		}

		[Test]
		public void CreateCommandWithManualTrigger()
		{
			customTrigger = new MockTrigger();
			new Command(() => pos += Time.Delta).Add(customTrigger);
			InvokingTriggerWithoutRunningEntitiesDoesNotCauseAction();
			InvokingTriggerOnceWithMultipleRunsOnlyCausesOneAction();
		}

		private MockTrigger customTrigger;
		private float pos;

		private void InvokingTriggerWithoutRunningEntitiesDoesNotCauseAction()
		{
			customTrigger.Invoke();
			Assert.AreEqual(0.0f, pos);
		}

		[Test]
		public void CommandIsNotPausable()
		{
			Assert.IsFalse(new Command(() => { }).IsPauseable);
		}

		private void InvokingTriggerOnceWithMultipleRunsOnlyCausesOneAction()
		{
			customTrigger.Invoke();
			entities.RunEntities();
			Assert.AreEqual(0.05f, pos);
			entities.RunEntities();
			Assert.AreEqual(0.05f, pos);
		}

		[Test]
		public void UnableToRegisterCommandWithoutNameOrTriggers()
		{
			Assert.Throws<ArgumentNullException>(() => Command.Register("", null));
			Assert.Throws<Command.UnableToRegisterCommandWithoutTriggers>(
				() => Command.Register("a", null));
		}

		[Test]
		public void RegisteringSameCommandTwiceOverwritesIt()
		{
			var exitTrigger = new MockTrigger();
			Command.Register("Exit", exitTrigger);
			Command.Register("Exit", exitTrigger);
		}

		[Test]
		public void CommandNameMustBeRegisteredToCreateANewCommand()
		{
			Assert.Throws<Command.CommandNameWasNotRegistered>(
				() => new Command("UnregisteredCommand", (Action)null));
		}

		[Test]
		public void CommandWithPositionAction()
		{
			var trigger = new MockPositionTrigger();
			var commandName = RegisterCommand(trigger);
			new Command(commandName, delegate(Vector2D point) { actionPerformed = true; });
			AssertActionPerformed(trigger);
		}

		private bool actionPerformed;

		private void AssertActionPerformed(Trigger trigger)
		{
			Assert.IsFalse(actionPerformed);
			trigger.Invoke();
			entities.RunEntities();
			Assert.IsTrue(actionPerformed);
		}

		[Test]
		public void CommandWithDragAction()
		{
			var trigger = new MockDragTrigger();
			var commandName = RegisterCommand(trigger);
			new Command(commandName, delegate(Vector2D point, Vector2D dims, bool test)
				{ actionPerformed = true; });
			AssertActionPerformed(trigger);
		}

		[Test]
		public void CommandWithZoomAction()
		{
			var trigger = new MockZoomTrigger();
			var commandName = RegisterCommand(trigger);
			new Command(commandName, delegate(float zoom) { actionPerformed = true; });
			AssertActionPerformed(trigger);
		}

		[Test]
		public void CommandWithPositionctionMock()
		{
			var trigger = new MockTrigger();
			var commandName = RegisterCommand(trigger);
			new Command(commandName, delegate(Vector2D point) { actionPerformed = true; });
			AssertActionPerformed(trigger);
		}

		[Test]
		public void CommandWithDragActionMock()
		{
			var trigger = new MockTrigger();
			var commandName = RegisterCommand(trigger);
			new Command(commandName, delegate(Vector2D point, Vector2D dims, bool test)
				{ actionPerformed = true; });
			AssertActionPerformed(trigger);
		}

		private string RegisterCommand(Trigger trigger)
		{
			const string CommandName = "PositionActionCommand";
			Command.Register(CommandName, trigger);
			actionPerformed = false;
			return CommandName;
		}

		[Test]
		public void CommandWithZoomActionMock()
		{
			var trigger = new MockTrigger();
			const string CommandName = "PositionActionCommand";
			Command.Register(CommandName, trigger);
			actionPerformed = false;
			new Command(CommandName, delegate(float zoom) { actionPerformed = true; });
			AssertActionPerformed(trigger);
		}

		public class MockPositionTrigger : InputTrigger, PositionTrigger
		{
			protected override void StartInputDevice() {}
			public Vector2D Position { get; set; }
		}

		public class MockDragTrigger : DragTrigger
		{
			protected override void StartInputDevice() {}
		}

		public class MockZoomTrigger : InputTrigger, ZoomTrigger
		{
			protected override void StartInputDevice() {}
			public float ZoomAmount { get; set; }
		}

		[Test]
		public void RegisterCommandWithSeveralTriggers()
		{
			const string CommandName = "CommandWithSeveralTriggers";
			var trigger1 = new MockTrigger();
			var trigger2 = new MockTrigger();
			Command.Register(CommandName, trigger1, trigger2);
			var command = new Command(CommandName, (Action)null);
			Assert.AreEqual(2, command.GetTriggers().Count);
		}

		[Test]
		public void InvokeOnTrigger()
		{
			customTrigger = new MockTrigger();
			customTrigger.Invoked = () => { customTrigger.UpdatePriority = Priority.Normal; };
			customTrigger.Invoke();
			Assert.AreEqual(customTrigger.UpdatePriority, Priority.Normal);
			Assert.IsFalse(customTrigger.IsPauseable);
		}
	}
}