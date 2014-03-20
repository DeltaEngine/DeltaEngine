using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Emulator.Helpers
{
	public class DesignToolboxEntry : Tool
	{
		//ncrunch: no coverage start
		public string ShortName
		{
			get { return "Test Toolbox Entry"; }
		}

		public string Icon
		{
			get { return "../Images/UIEditor/CreateButton.png"; }
		}

		public string ToolTipText 
		{ 
			get { return "Test ToolTip"; }
		}
	}
}