using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace CarGame2D
{
	/// <summary>
	/// Track class for the game, handles painting and helps with collision checking.
	/// </summary>
	public class Track : Entity2D
	{
		public Track(Vector2D[] positions)
		{
			Positions = positions;
			Material = new Material(ShaderFlags.Position2DTextured, "TrackImage");
			OnDraw<TrackRenderer>();
			CenterAllPositions(positions);
			TrackStart = positions[0];
			CalculateInnerAndOuterBounds(positions);
		}

		public Vector2D[] Positions { get; private set; }
		public Material Material { get; private set; }

		private static void CenterAllPositions(Vector2D[] positions)
		{
			Vector2D center = positions[0];
			for (int index = 1; index < positions.Length; index++)
				center += positions[index];
			center /= positions.Length;
			Vector2D offset = new Vector2D(0.5f, 0.49f) - center;
			for (int index = 0; index < positions.Length; index++)
				positions[index] += offset;
		}

		public Vector2D TrackStart { get; private set; }

		private void CalculateInnerAndOuterBounds(Vector2D[] positions)
		{
			TrackInnerBounds = new Vector2D[positions.Length];
			TrackOuterBounds = new Vector2D[positions.Length];
			for (int index = 0; index < Positions.Length; index++)
			{
				var previousPos = Positions[index - 1 == -1 ? Positions.Length - 1 : index - 1];
				var nextPos = Positions[(index + 1) % Positions.Length];
				var dir = Vector2D.Normalize(nextPos - previousPos);
				var normal = Vector2D.Normalize(dir.Rotate(90));
				TrackOuterBounds[index] = Positions[index] - normal * TrackWidth * 0.5f;
				TrackInnerBounds[index] = Positions[index] + normal * TrackWidth * 0.5f;
			}
		}

		public Vector2D[] TrackInnerBounds { get; private set; }
		public Vector2D[] TrackOuterBounds { get; private set; }
		public const float TrackWidth = 0.12f;
	}
}