using System.Collections.Generic;

namespace DeltaEngine.Editor.Core
{
	public interface ToolExtensions
	{
		List<string> GetNames();
		string GetImagePath(string toolName);
	}
}
