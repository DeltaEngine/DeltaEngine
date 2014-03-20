using DeltaEngine.Datatypes;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Returned by RenderingCalculator and tells how an image should be rendered.
	/// </summary>
	public class RenderingData : Lerp<RenderingData>
	{
		public Rectangle RequestedUserUV { get; internal set; }
		public Rectangle RequestedDrawArea { get; internal set; }
		public FlipMode FlipMode { get; internal set; }
		public Rectangle AtlasUV { get; internal set; }
		public Rectangle DrawArea { get; internal set; }
		public bool HasSomethingToRender { get; internal set; }
		public bool IsAtlasRotated { get; internal set; }

		public RenderingData Lerp(RenderingData other, float interpolation)
		{
			return new RenderingData
			{
				RequestedUserUV = other.RequestedUserUV,
				RequestedDrawArea = other.RequestedDrawArea,
				FlipMode = FlipMode,
				AtlasUV = AtlasUV.Lerp(other.AtlasUV, interpolation),
				DrawArea = DrawArea.Lerp(other.DrawArea, interpolation),
				HasSomethingToRender = HasSomethingToRender && other.HasSomethingToRender,
				IsAtlasRotated = IsAtlasRotated && other.IsAtlasRotated
			};
		}
	}
}