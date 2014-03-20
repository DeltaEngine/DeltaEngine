using System.Windows.Controls;

namespace DeltaEngine.Editor.InputEditor
{
	public class TriggerLayoutData
	{
		public int NumberOfAddingItems { get; set; }
		public int NumberOfRemovingItems { get; set; }
		public string AddingItem { get; set; }
		public string RemovingItem { get; set; }
		public ComboBox KeyBox { get; set; }
		public ComboBox TypeBox { get; set; }
	}
}