using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Editor.LevelEditor
{
	public class SpriteLevelObject : DrawableEntity
	{
		public SpriteLevelObject(Vector2D position, string selectedLevelObject)
		{
			this.position = position;
			this.selectedLevelObject = selectedLevelObject;
			rotationAngel = 0.0f;
			Create();
		}

		private readonly Vector2D position;
		private readonly string selectedLevelObject;
		public string Name { get; private set; }
		private float rotationAngel;

		private void Create()
		{
			Name = selectedLevelObject;
			var image = ContentLoader.Load<Material>(selectedLevelObject);
			levelObject =
				new Sprite(new Material(ShaderFlags.Position2DColoredTextured, image.DiffuseMap.Name),
					new Rectangle(position, new Size(0.05f)));
		}

		private Sprite levelObject;

		public override void Dispose()
		{
			levelObject.Dispose();
			base.Dispose();
		}

		public void Rotate()
		{
			rotationAngel += 90.0f;
			levelObject.Rotation = rotationAngel;
		}
	}
}