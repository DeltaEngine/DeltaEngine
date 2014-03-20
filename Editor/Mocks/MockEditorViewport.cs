using DeltaEngine.Editor.Core;
using DeltaEngine.Mocks;

namespace DeltaEngine.Editor.Mocks
{
	public class MockEditorViewport : EditorViewport
	{
		public MockEditorViewport()
			: base(new MockWindow()) {}
	}
}