using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DeltaEngine.Core;

namespace DeltaEngine.Graphics.OpenGL
{
	public class OpenGL20ScreenshotCapturer : ScreenshotCapturer
	{
		private readonly OpenGLDevice device;
		private readonly Window window;
		private int width;
		private int height;

		public OpenGL20ScreenshotCapturer(Device device, Window window)
		{
			this.device = (OpenGLDevice)device;
			this.window = window;
		}

		public void MakeScreenshot(string fileName)
		{
			if (device == null)
				return;
			width = (int)window.ViewportPixelSize.Width;
			height = (int)window.ViewportPixelSize.Height;
			byte[] rgbData = new byte[width * height * 3];
			device.ReadPixels(new DeltaEngine.Datatypes.Rectangle(0.0f, 0.0f, width, height), rgbData);
			using (Bitmap bitmap = CopyRgbIntoBitmap(rgbData))
				using (Stream stream = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite))
					bitmap.Save(stream, ImageFormat.Png);
		}

		private unsafe Bitmap CopyRgbIntoBitmap(byte[] rgbData)
		{
			Bitmap bitmap = new Bitmap(width, height);
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			byte* bitmapPointer = (byte*)bitmapData.Scan0.ToPointer();
			SwitchTopToBottomAndRgbToBgr(bitmapPointer, rgbData);
			bitmap.UnlockBits(bitmapData);
			return bitmap;
		}

		private unsafe void SwitchTopToBottomAndRgbToBgr(byte* bitmapPointer, byte[] rgbData)
		{
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
				{
					int targetIndex = (y * width + x) * 3;
					int sourceIndex = (((height - 1) - y) * width + x) * 3;
					bitmapPointer[targetIndex] = rgbData[sourceIndex + 2];
					bitmapPointer[targetIndex + 1] = rgbData[sourceIndex + 1];
					bitmapPointer[targetIndex + 2] = rgbData[sourceIndex];
				}
		}
	}
}