using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaNinja.Entities
{
	public class MovingSprite : Sprite
	{
		private static int ID = 0;
		public const float Gravity = 0.5f; 
		
		public MovingSprite(string image, Color color, Vector2D position, Size size)
			: base(image, Rectangle.One)
		{
			Size = size;
			Center = position;
			ID += 1;
		}

		public bool IsPaused { get; protected set; }

		public virtual void SetPause(bool pause)
		{
			IsPaused = pause;
		}

		private bool CheckBounds(Rectangle view)
		{
			if (view.Left > DrawArea.Left) return false;
			if (view.Right < DrawArea.Right) return false;
			if (DrawArea.Top < 0) return false;
			return view.Top + 0.05f <= DrawArea.Center.Y;
		}

		protected static bool CheckIfLineIntersectsLine(Vector2D l1p1, Vector2D l1p2, Vector2D l2p1,
			Vector2D l2p2)
		{
			// See http://stackoverflow.com/questions/5514366/how-to-know-if-a-line-intersects-a-rectangle
			float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
			float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);
			if (d == 0)
				return false;
			float r = q / d;
			q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
			float s = q / d;
			return r >= 0 && r <= 1 && s >= 0 && s <= 1;
		}

		public bool IsOutside(Rectangle view)
		{
			return DrawArea.Top > view.Bottom;
		}
	}
}