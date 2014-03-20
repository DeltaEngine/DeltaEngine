using System.Collections.Generic;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class InputCommandsTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			inactiveButtonsTagList = new List<string> { "Fire", "Ice" };
			input = new InGameCommands();
			mouse = Resolve<Mouse>() as MockMouse;
		}

		private List<string> inactiveButtonsTagList;
		private InGameCommands input;
		private MockMouse mouse;

		private void ClickMouse()
		{
			mouse.SetButtonState(MouseButton.Left, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, Ignore]
		public void DisplayTowerSelectionPanelWhenClicked()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			ClickMouse();
			ClickMouse();
		}
	}
}