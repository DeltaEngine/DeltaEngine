using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaEngine.Rendering3D.Shapes
{
	/// <summary>
	/// Simple 2D map of TileType data to be used for 2D tile games or for game logic tests.
	/// </summary>
	public class LevelDebugRenderer
	{
		public LevelDebugRenderer(Level map)
		{
			this.map = map;
			RecreateTiles();
			map.TileChanged += UpdateTile;
		}

		private readonly Level map;

		public void RecreateTiles()
		{
			Initialize();
			for (int y = 0; y < map.Size.Height; y++)
				for (int x = 0; x < map.Size.Width; x++)
					UpdateTile(new Vector2D(x, y));
		}

		private void Initialize()
		{
			if (map.RenderIn3D)
			{
				if (grid3D != null)
					Dispose3D(); //ncrunch: no coverage
				grid3D = new Grid3D(map.Size) { RenderLayer = 10 };
				models = new Model[(int)map.Size.Width * (int)map.Size.Height];
			}
			else
			{
				if (grid2D != null)
					Dispose2D(); //ncrunch: no coverage
				grid2D = new Grid2D(map.Size, Vector2D.Half, GridScale) { RenderLayer = 10 };
				rectangles = new FilledRect[(int)map.Size.Width * (int)map.Size.Height];
			}
		}

		private Grid3D grid3D;
		private Model[] models;
		private const float GridScale = 0.05f;

		//ncrunch: no coverage start
		public void Dispose3D()
		{
			grid3D.Dispose();
			for (int i = 0; i < models.Length; i++)
				if (models[i] != null)
					models[i].Dispose();
		}

		public void Dispose2D()
		{
			grid2D.Dispose();
			for (int i = 0; i < rectangles.Length; i++)
				if (rectangles[i] != null)
					rectangles[i].Dispose();
		} 

		private Grid2D grid2D;
		private FilledRect[] rectangles;

		public void UpdateTileAt(Vector2D position, int index)
		{
			var color = map.GetColor(map.MapData[index]);
			UpdateTile3D(index, position, color);
		} //ncrunch: no coverage end

		public void UpdateTile(Vector2D position)
		{
			var index = (int)position.X + (int)position.Y * (int)map.Size.Width;
			var color = map.GetColor(map.MapData[index]);
			if (map.RenderIn3D)
				UpdateTile3D(index, map.GetWorldCoordinates(position), color);
			else
				UpdateTile2D(index, position, color);
		}

		private void UpdateTile3D(int index, Vector2D position, Color color)
		{
			if (models[index] != null && models[index].Get<Color>() == color)
				return; //ncrunch: no coverage
			if (models[index] != null)
				models[index].IsActive = false; //ncrunch: no coverage
			if (color == Color.Black)
				color.A = 0;
			models[index] = CreateModel(position, color);
			models[index].Set(color);
		}

		private static Model CreateModel(Vector2D position, Color color)
		{
			var planeQuad = new PlaneQuad(Size.One, new Material(color, ShaderFlags.Colored));
			Quaternion orientation = Quaternion.FromAxisAngle(Vector3D.UnitX, -90);
			return new Model(new ModelData(planeQuad), new Vector3D(position), orientation);
		}

		private void UpdateTile2D(int index, Vector2D position, Color color)
		{
			if (rectangles[index] != null && rectangles[index].Get<Color>() == color)
				return; //ncrunch: no coverage
			if (rectangles[index] != null)
				rectangles[index].IsActive = false; //ncrunch: no coverage
			color.A = (byte)(color == Color.Black ? 0 : 100);
			var rect = CreateRectangle(position, color);
			rect.RenderLayer = 10;
			rectangles[index] = rect;
		}

		private FilledRect CreateRectangle(Vector2D position, Color color)
		{
			var drawArea = new Rectangle(GetRectangleDrawPosition(position), new Size(0.05f));
			return new FilledRect(drawArea, color);
		}

		public Vector2D GetRectangleDrawPosition(Vector2D position)
		{
			return new Vector2D((int)position.X * ZoomFactor + M * map.Size.Width + B,
				(int)position.Y * ZoomFactor + M * map.Size.Height + B);
		}

		private const float ZoomFactor = 0.05f;
		private const float M = -0.025f;
		private const float B = 0.5f;

		//ncrunch: no coverage start
		public void RemoveCommands()
		{
			map.TileChanged -= UpdateTile;
		}

		public Rectangle GetBorder()
		{
			return new Rectangle(grid2D.GetTopLeft(), grid2D.GetSize());
		}
	}
}