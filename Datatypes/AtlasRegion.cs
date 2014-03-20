namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Holds the information needed to render an individual sprite from within a texture atlas.
	/// A Sprite can be stripped of its transparent borders when inserted into a texture atlas. 
	/// PadLeft says how much was stripped from the left side etc
	/// </summary>
	public struct AtlasRegion
	{
		public AtlasRegion(Rectangle uv)
			: this()
		{
			UV = uv;
		}

		public Rectangle UV { get; set; }
		public float PadLeft { get; set; }
		public float PadRight { get; set; }
		public float PadTop { get; set; }
		public float PadBottom { get; set; }
		public bool IsRotated { get; set; }
	}
}