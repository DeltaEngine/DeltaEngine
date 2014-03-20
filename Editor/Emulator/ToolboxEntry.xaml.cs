using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Emulator
{
	public partial class ToolboxEntry : Tool
	{
		public ToolboxEntry()
		{
			InitializeComponent();
		}

		public ToolboxEntry(string name, string icon, string toolTip)
			: this()
		{
			ShortName = name;
			Icon = icon;
			ToolTipText = toolTip;
		}

		public string ShortName { get; private set; }
		public string Icon { get; private set; }
		public string ToolTipText { get; private set; }
	}
}