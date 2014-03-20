using System.Collections.Generic;

namespace DeltaEngine.Editor.Helpers
{
	public interface SingleInstanceApp
	{
		void SignalExternalCommandLineArguments(IList<string> args);
	}
}