using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering3D
{
	/// <summary>
	/// Collection of static meshes to be rendered, makes it easier rendering complex things like
	/// Levels, 3D Models with animation and collections of meshes. Should be combined with Actor.
	/// </summary>
	public class Model : HierarchyEntity3D
	{
		public Model(string modelContentName, Vector3D position)
			: this(ContentLoader.Load<ModelData>(modelContentName), position) {}

		public Model(ModelData data, Vector3D position)
			: this(data, position, Quaternion.Identity) {}

		public Model(ModelData data, Vector3D position, Quaternion orientation)
			: base(position, orientation)
		{
			Add(data);
			OnDraw<ModelRenderer>();
			Start<AnimationUpdater>();
		}

		private class AnimationUpdater : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var model in entities.OfType<Model>())
					UpdateModelMeshes(model.Get<ModelData>());
			}

			private static void UpdateModelMeshes(ModelData data)
			{
				foreach (var mesh in data.Meshes)
					if (mesh.IsAnimated)
						UpdateMeshAnimation(mesh);
			}

			private static void UpdateMeshAnimation(Mesh mesh)
			{
				mesh.Animation.UpdateFrameTime();
				mesh.Geometry.TransformsOfBones = mesh.Animation.CurrentFrame.TransformsOfBones;
			}
		}
	}
}