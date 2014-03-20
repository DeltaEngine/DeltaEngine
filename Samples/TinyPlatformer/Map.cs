using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Content.Json;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;

namespace TinyPlatformer
{
	/// <summary>
	/// Loads and parses a map
	/// </summary>
	public class Map
	{
		public Map(string mapContentName = DefaultMapContentName)
			: this(ContentLoader.Load<JsonContent>(mapContentName).Data) {}

		private const string DefaultMapContentName = "Level";

		public Map(JsonNode root)
		{
			CreateBlocks(root);
			CreateActors(root);
			CreateBackgroundGraphics();
			scoreText = new FontText(ContentLoader.Load<Font>("Verdana12"), "Score: 0", ScoreDrawArea);
		}

		internal int score;
		internal readonly FontText scoreText;
		private static readonly Rectangle ScoreDrawArea =
			Rectangle.FromCenter(0.5f, 0.25f, 0.2f, 0.2f);

		private void CreateBlocks(JsonNode root)
		{
			width = root.Get<int>("width");
			height = root.Get<int>("height");
			Blocks = new BlockType[width,height];
			var blocksData = root["layers"][0]["data"].GetIntArray();
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
					Blocks[x, y] = (BlockType)blocksData[x + y * width];
		}

		internal int width;
		internal int height;
		internal BlockType[,] Blocks { get; private set; }

		private void CreateActors(JsonNode root)
		{
			var entityArray = root["layers"][1]["objects"];
			for (int entity = 0; entity < entityArray.NumberOfNodes; entity++)
				CreateActor(entityArray[entity]);
		}

		private void CreateActor(JsonNode entityData)
		{
			JsonNode properties = entityData["properties"];
			var position = new Vector2D(entityData.Get<int>("x"), entityData.Get<int>("y"));
			var type = entityData.Get<string>("type");
			actorList.Add(new Actor(this, position, type)
			{
				MaxVelocityX = Meter * properties.GetOrDefault("maxdx", DefaultMaxVelocityX),
				WantsToGoLeft = properties.GetOrDefault("left", false),
				WantsToGoRight = properties.GetOrDefault("right", false)
			});
		}

		public const int Meter = 32;
		public const int DefaultMaxVelocityX = 15;
		internal readonly List<Actor> actorList = new List<Actor>();

		private void CreateBackgroundGraphics()
		{
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
					if (Blocks[x, y] != BlockType.None)
						new FilledRect(
							new Rectangle(ScreenGap.Width + BlockSize * x, ScreenGap.Height + BlockSize * y,
								BlockSize, BlockSize), GetColor(Blocks[x, y]));
		}

		public static Size ScreenGap = new Size(0.1f, 0.2f);
		public const float BlockSize = 0.0125f;

		internal static Color GetColor(BlockType blockType)
		{
			switch (blockType)
			{
			case BlockType.Gold:
				return Color.Gold;
			case BlockType.GroundBrick:
				return Color.Orange;
			case BlockType.PlatformBrick:
				return Color.Red;
			case BlockType.PlatformTop:
				return Color.Purple;
			case BlockType.LevelBorder:
				return Color.Teal;
			default:
				return Color.TransparentBlack;
			}
		}

		public Actor Player
		{
			get { return player ?? (player = actorList.Find(actor => actor.IsPlayer)); }
		}
		private Actor player;

		public void AddToScore(string type)
		{
			if (type == "monster")
				score += 3;
			else if (type == "treasure")
				score++;
			scoreText.Text = "Score: " + score;
		}
	}
}