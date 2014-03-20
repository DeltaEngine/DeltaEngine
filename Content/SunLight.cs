using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Content
{
	public class SunLight
	{
		public static SunLight Current
		{
			get { return ThreadStaticSunLight.Current; }
			set { ThreadStaticSunLight.Use(value); }
		}

		private static readonly ThreadStatic<SunLight> ThreadStaticSunLight =
			new ThreadStatic<SunLight>(new SunLight(new Vector3D(1.0f, 1.0f, 1.5f), Color.White));

		public SunLight(Vector3D direction, Color color)
		{
			Direction = Vector3D.Normalize(direction);
			Color = color;
		}

		public Vector3D Direction
		{
			get { return direction; }
			set { direction = Vector3D.Normalize(value); }
		}
		private Vector3D direction;

		public Color Color { get; set; }
	}
}