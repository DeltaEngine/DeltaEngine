using System.Collections.Generic;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Helpers
{
	public class DesignEditorPlugin : EditorPluginView
	{
		public void Init(Service service) {}

		public void Activate() {}

		public void Deactivate() {}

		public string ShortName
		{
			get { return "Test Plugin"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Content.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void Send(IList<string> arguments) {}
	}
}