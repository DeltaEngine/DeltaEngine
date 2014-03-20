using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering3D
{
	public class ModelRenderer : DrawBehavior
	{
		public ModelRenderer(Drawing drawing)
		{
			this.drawing = drawing;
		}

		private readonly Drawing drawing;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			foreach (DrawableEntity entity in visibleEntities)
				DrawModel(entity as Model);
		}

		private void DrawModel(Model model)
		{
			if (model == null)
				return;
			var modelTransform = Matrix.CreateScale(model.Scale);
			modelTransform *= Matrix.FromQuaternion(model.Orientation);
			modelTransform.Translation = model.Position;
			var data = model.Get<ModelData>();
			foreach (var mesh in data.Meshes)
				drawing.AddGeometry(mesh.Geometry, mesh.Material, mesh.LocalTransform * modelTransform);
		}
	}
}