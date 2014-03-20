namespace DeltaEngine.Editor.Core
{
	public interface Tool
	{
		string ShortName { get; }
		string Icon { get; }
		string ToolTipText { get; }
	}
}