using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaNinja.UI
{
	public class LogoLink : Sprite
	{
		public LogoLink(string image, string url, float size)
			: base(image, new Rectangle(0, 0, size, size))
		{
			Url = url;
			reduceSize = new Size(size * 0.18f);
		}

		public string Url { get; private set; }
		private readonly Size reduceSize;
		
		public bool IsHover(Vector2D point)
		{
			var hoverZone = DrawArea.Reduce(reduceSize);
			return hoverZone.Contains(point);
		}
	}
}