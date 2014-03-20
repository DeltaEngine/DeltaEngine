using DeltaEngine.Editor.Core;
using DeltaEngine.Scenes;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class SceneCodeGeneratorSpy : SceneCodeGenerator
	{
		public SceneCodeGeneratorSpy(Service service, Scene scene, string sceneName)
			: base(service, scene, sceneName) {}

		protected override void CreateNewSourceCodeClass()
		{
			if (!CreatedSourceClass)
				CreatedSourceClass = true;
			CreatedClassString = ClassCodeString;
			csprojString += "<Compile Include=" + '"' + sceneClassName + ".cs" + '"' + " />";
		}

		public bool CreatedSourceClass;
		public string csprojString;
		public string CreatedClassString;
	}
}