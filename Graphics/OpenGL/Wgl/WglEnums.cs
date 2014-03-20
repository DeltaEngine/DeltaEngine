using System;

namespace DeltaEngine.Graphics.OpenGL.Wgl
{
	internal enum WglLayerType
	{
		PfdMainPlane = 0,
	}

	public enum ColorFormatPixelType
	{
		Rgba = 0,
	}

	[Flags]
	internal enum PixelFormatDescriptorFlags
	{
		DoubleBuffer = 0x01,
		DrawToWindow = 0x04,
		SupportOpenGL = 0x20,
	}
}