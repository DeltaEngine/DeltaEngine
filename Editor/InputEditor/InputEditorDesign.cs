using System.Collections.Generic;

namespace DeltaEngine.Editor.InputEditor
{
	public class InputEditorDesign
	{
		public InputEditorDesign()
		{
			NewCommand = "New Command";
			AvailableTriggers = new List<string> { "Trigger" };
			SelectedTrigger = "Trigger";
			CommandList = new List<string> { "Command 1", "Command 2", "..." };
			TriggerList = new List<string> { "Trigger 1", "Trigger 2", "..." };
		}

		public string NewCommand { get; set; }
		public List<string> AvailableTriggers { get; set; }
		public string SelectedTrigger { get; set; }
		public List<string> CommandList { get; set; }
		public List<string> TriggerList { get; set; }
	}
}
