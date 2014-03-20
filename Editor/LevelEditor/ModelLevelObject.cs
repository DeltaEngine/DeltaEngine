using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering3D;

namespace DeltaEngine.Editor.LevelEditor
{
	public class ModelLevelObject : DrawableEntity
	{
		public ModelLevelObject(Vector2D position, string selectedLevelObject, int levelObjectIndex)
		{
			Position = position;
			LevelObjectIndex = levelObjectIndex;
			SelectedLevelObject = selectedLevelObject;
			rotationAngel = 0.0f;
			Create();
		}

		public Vector2D Position { get; private set; }
		public int LevelObjectIndex { get; private set; }
		public string SelectedLevelObject { get; private set; }
		public string Name { get; private set; }

		private float rotationAngel;

		private void Create()
		{
			Name = SelectedLevelObject;
			var modelData = ContentLoader.Load<ModelData>(SelectedLevelObject);
			levelObject = new Model(modelData, new Vector3D(Position.X, Position.Y, 0));
		}

		private Model levelObject;

		public override void Dispose()
		{
			levelObject.Dispose();
			base.Dispose();
		}

		public void Rotate()
		{
			rotationAngel += 15.0f;
			levelObject.Orientation = Quaternion.FromAxisAngle(Vector3D.UnitZ, rotationAngel);
		}
	}
}