using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DeltaEngine.Content.Xml;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Content.Disk.Tests
{
	public static class ContentDiskTestsExtensions
	{
		public static void CreateImage(string filePath, Size pixelSize)
		{
			var newBitmap = new Bitmap((int)pixelSize.Width, (int)pixelSize.Height,
				PixelFormat.Format32bppArgb);
			for (int y = 0; y < newBitmap.Height; y++)
				for (int x = 0; x < newBitmap.Width; x++)
					newBitmap.SetPixel(x, y, Color.White);
			if (filePath.EndsWith("DeltaEngineLogo.png"))
				newBitmap.SetPixel(50, 70, Color.FromArgb(0, 0, 0, 0));
			newBitmap.Save(filePath);
		}

		public static void CreateImageAndContentMetaData(string filePath, Size pixelSize,
			XmlData image)
		{
			CreateImage(filePath, pixelSize);
			image.AddAttribute("FileSize", new FileInfo(filePath).Length);
			image.AddAttribute("PixelSize", pixelSize.ToString());
		}
	}
}