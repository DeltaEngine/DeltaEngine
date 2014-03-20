using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering3D;

namespace DeltaEngine.Editor.LevelEditor
{
	public class MaterialLevelObject : DrawableEntity
	{
		public MaterialLevelObject(Vector2D position, string selectedLevelObject)
		{
			this.position = position;
			this.selectedLevelObject = selectedLevelObject;
			rotationAngel = 0.0f;
			Create();
		}

		private Vector2D position;
		private readonly string selectedLevelObject;
		public string Name { get; private set; }
		private float rotationAngel;

		private void Create()
		{
			Name = selectedLevelObject;
			var plane = new PlaneQuad(Size.One, ContentLoader.Load<Material>(selectedLevelObject));
			levelObject = new Model(new ModelData(plane), new Vector3D(position.X, position.Y, 0.0f));
			levelObject.Orientation = Quaternion.FromAxisAngle(Vector3D.UnitX, -90.0f);
		}

		private Model levelObject;

		public override void Dispose()
		{
			levelObject.Dispose();
			base.Dispose();
		}

		public void Rotate()
		{
			rotationAngel += 90.0f;
			//levelObject.Orientation = Quaternion.FromAxisAngle(Vector3D.UnitY, rotationAngel);
		}
	}
}