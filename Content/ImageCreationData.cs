using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Provides a way to create an <see cref="Image"/> dynamically and fill it with custom pixels.
	/// </summary>
	public class ImageCreationData : ContentCreationData
	{
		public ImageCreationData(Size pixelSize)
		{
			PixelSize = pixelSize;
		}

		public Size PixelSize { get; private set; }
		public BlendMode BlendMode { get; set; }
		public bool UseMipmaps { get; set; }
		public bool AllowTiling { get; set; }
		public bool DisableLinearFiltering { get; set; }
	}
}