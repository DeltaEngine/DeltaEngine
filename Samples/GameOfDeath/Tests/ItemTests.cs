using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using GameOfDeath.Items;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	internal class ItemTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void PrepareUI()
		{
			userInterface = new UserInterface();
		}

		private UserInterface userInterface;

		[Test]
		public void ShowMallet()
		{
			userInterface.Money = 1;
			userInterface.SelectItemIfSufficientFunds(0);
			Assert.AreEqual(ItemType.Mallet, userInterface.CurrentItem.ItemType);
		}

		[Test]
		public void ShowFire()
		{
			userInterface.Money = 5;
			userInterface.SelectItemIfSufficientFunds(1);
			Assert.AreEqual(ItemType.Fire, userInterface.CurrentItem.ItemType);
		}

		[Test]
		public void ShowToxic()
		{
			userInterface.Money = 20;
			userInterface.SelectItemIfSufficientFunds(2);
			Assert.AreEqual(ItemType.Toxic, userInterface.CurrentItem.ItemType);
		}

		[Test]
		public void ShowAtomic()
		{
			userInterface.Money = 50;
			userInterface.SelectItemIfSufficientFunds(3);
			Assert.AreEqual(ItemType.Atomic, userInterface.CurrentItem.ItemType);
		}

		[Test]
		public void AllowToCastAnyItemInGame() {}

		[Test]
		public void SimulateClicksInField()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
			RunAfterFirstFrame(
				() => { Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing); });
		}

		[Test]
		public void SimulateClicksOnIcons()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			Resolve<MockMouse>().SetNativePosition(new Vector2D(0.5f, 0.2f));
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
			AdvanceTimeAndUpdateEntities(0.1f);
			Resolve<MockMouse>().SetNativePosition(Vector2D.Half);
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
		}

		[Test]
		public void SimulateMalletClick()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
			AdvanceTimeAndUpdateEntities(0.1f);
			Resolve<MockMouse>().SetNativePosition(Vector2D.One);
			Resolve<MockMouse>().SetButtonState(MouseButton.Left, State.Pressing);
		}
	}
}