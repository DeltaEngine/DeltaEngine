using System.Collections.Generic;

namespace DeltaEngine.Editor.Core
{
	/// <summary>
	/// Editor Plugin User Controls should derive this interface to appear in the editor list and get
	/// the Service injected in the constructor. Plugins are loaded dynamically in the editor folder,
	/// or you put your project in the DeltaEngine/Editor/ folder to be automatically picked up.
	/// </summary>
	public interface EditorPluginView
	{
		void Init(Service service);
		void Activate();
		void Deactivate();
		string ShortName { get; }
		string Icon { get; }
		bool RequiresLargePane { get; }
		void Send(IList<string> arguments);
	}
}