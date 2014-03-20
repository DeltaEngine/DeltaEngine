using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.Rendering3D;

namespace DeltaEngine.Editor.LevelEditor
{
	public class LevelObjectHandler
	{
		public LevelObjectHandler(int levelSize)
		{
			LevelSize = levelSize;
			ObjectList = new DrawableEntity[levelSize];
		}

		public LevelObjectHandler(Level level)
			: this((int)level.Size.Width * (int)level.Size.Height) {}

		public int LevelSize
		{
			set { ObjectList = new DrawableEntity[value]; }
		}

		public DrawableEntity[] ObjectList { get; private set; }

		public void SetBackgroundModel(string selectedModel)
		{
			if (backgroundModel != null)
				backgroundModel.Dispose();
			if (selectedModel == "" || selectedModel == "None")
				return;
			var modelData = ContentLoader.Load<ModelData>(selectedModel);
			backgroundModel = new Model(modelData, Vector3D.Zero);
		}

		private Model backgroundModel;

		public void PlaceLevelObject(Vector2D position, string selectedLevelObject,
			int levelObjectIndex)
		{
			if (selectedLevelObject == "None")
				return;
			if (ContentLoader.Exists(selectedLevelObject, ContentType.Model))
				PlaceModel(position, selectedLevelObject, levelObjectIndex);
			else if (ContentLoader.Exists(selectedLevelObject, ContentType.ParticleEmitter))
				PlaceParticle(position, selectedLevelObject, levelObjectIndex);
			else if (ContentLoader.Exists(selectedLevelObject, ContentType.Material))
				PlaceMaterial(position, selectedLevelObject, levelObjectIndex);
		}

		private void PlaceModel(Vector2D position, string selectedLevelObject, int levelObjectIndex)
		{
			var model = new ModelLevelObject(position, selectedLevelObject, levelObjectIndex);
			ObjectList[levelObjectIndex] = model;
		}

		private void PlaceParticle(Vector2D position, string selectedLevelObject, int levelObjectIndex)
		{
			var particle = new ParticleLevelObject(position, selectedLevelObject);
			ObjectList[levelObjectIndex] = particle;
		}

		private void PlaceMaterial(Vector2D position, string selectedLevelObject,
			int levelObjectIndex)
		{
			DrawableEntity entity;
			if (Is3D)
				entity = new MaterialLevelObject(position, selectedLevelObject);
			else
				entity = new SpriteLevelObject(position, selectedLevelObject);
			ObjectList[levelObjectIndex] = entity;
		}

		public bool Is3D { private get; set; }

		public void RecreateObjects()
		{
			for (int i = 0; i < ObjectList.Length; i++)
			{
				if (ObjectList[i] == null)
					continue;
				if (ObjectList[i] is ModelLevelObject)
				{
					var modelObject = (ModelLevelObject)ObjectList[i];
					ObjectList[i].Dispose();
					ObjectList[i] = new ModelLevelObject(modelObject.Position, modelObject.SelectedLevelObject,
						modelObject.LevelObjectIndex);
				}
			}
		}

		public void MoveObject(Vector2D position, int startDragIndex, int levelPositionIndex)
		{
			var name = GetLevelObjectName(startDragIndex);
			ObjectList[levelPositionIndex] = ObjectList[startDragIndex];
			PlaceLevelObject(position, name, levelPositionIndex);
			ObjectList[startDragIndex].Dispose();
			ObjectList[startDragIndex] = null;
		}

		private string GetLevelObjectName(int startDragIndex)
		{
			if (ObjectList[startDragIndex] is ModelLevelObject)
			{
				var model = (ModelLevelObject)ObjectList[startDragIndex];
				return model.Name;
			}
			if (ObjectList[startDragIndex] is ParticleLevelObject)
			{
				var particle = (ParticleLevelObject)ObjectList[startDragIndex];
				return particle.Name;
			}
			if (ObjectList[startDragIndex] is MaterialLevelObject)
			{
				var material = (MaterialLevelObject)ObjectList[startDragIndex];
				return material.Name;
			}
			if (ObjectList[startDragIndex] is SpriteLevelObject)
			{
				var sprite = (SpriteLevelObject)ObjectList[startDragIndex];
				return sprite.Name;
			}
			return "None";
		}

		public void RotateObject(int levelObjectIndex)
		{
			if (ObjectList[levelObjectIndex] is ModelLevelObject)
			{
				var model = (ModelLevelObject)ObjectList[levelObjectIndex];
				model.Rotate();
			}
			if (ObjectList[levelObjectIndex] is MaterialLevelObject)
			{
				var material = (MaterialLevelObject)ObjectList[levelObjectIndex];
				material.Rotate();
			}
			if (ObjectList[levelObjectIndex] is SpriteLevelObject)
			{
				var sprite = (SpriteLevelObject)ObjectList[levelObjectIndex];
				sprite.Rotate();
			}
		}
	}
}