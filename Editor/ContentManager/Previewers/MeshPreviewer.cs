using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Shapes;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class MeshPreviewer : ContentPreview
	{
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			new Grid3D(new Size(10));
			var meshes = new[] { ContentLoader.Load<Mesh>(contentName) };
			new Model(new ModelData(meshes), new Vector3D(0, 0, 0));
		}
	}
}