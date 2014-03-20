using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Tests
{
	public class MockEditorPluginView : EditorPluginView
	{
		public void Init(Service service)
		{
			Logger.Info("MockEditorPlugin initialized");
		}

		public void Activate()
		{
			Logger.Info("MockEditorPlugin activated");
		}

		public void Deactivate()
		{
			Logger.Info("MockEditorPlugin deactivated");
		}

		public string ShortName
		{
			get { return "Mock Plugin"; }
		}

		public string Icon
		{
			get { return "Mock.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void Send(IList<string> arguments) {}
	}
}