using System.Collections.Generic;

namespace DeltaEngine.Editor.Emulator.Helpers
{
	public class DesignViewportControl
	{
		//ncrunch: no coverage start
		public DesignViewportControl()
		{
			Tools = new List<ToolboxEntry>();
			Tools.Add(new ToolboxEntry("Tool 1", "../Images/UIEditor/CreateLabel.png", "ToolTip 1"));
			Tools.Add(new ToolboxEntry("Tool 2", "../Images/UIEditor/CreateRadioDialog.png", "ToolTip 1"));
			Tools.Add(new ToolboxEntry("Tool 3", "../Images/UIEditor/CreateTilemap.png", "ToolTip 1"));
		}

		public List<ToolboxEntry> Tools { get; private set; }
	}
}