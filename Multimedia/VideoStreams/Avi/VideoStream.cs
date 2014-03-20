using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Multimedia.VideoStreams.Avi
{
	/// <summary>
	/// Streams AVI video playback.
	/// </summary>
	public class VideoStream : AviStream
	{
		public VideoStream(int filePtr, IntPtr streamPtr)
			: base(filePtr, streamPtr)
		{
			var bih = new AviInterop.BitmapInfoHeader();
			int size = Marshal.SizeOf(bih);
			AviInterop.AVIStreamReadFormat(StreamPointer, 0, ref bih, ref size);
			AviInterop.StreamInfo streamInfo = GetStreamInfo();

			FrameRate = streamInfo.dwRate / (float)streamInfo.dwScale;
			Width = streamInfo.rcFrame.right;
			Height = streamInfo.rcFrame.bottom;
			FrameSize = bih.biSizeImage;
			CountBitsPerPixel = CalculateBitsPerPixel(bih.biBitCount);
			FirstFrame = AviInterop.AVIStreamStart(StreamPointer);
			CountFrames = AviInterop.AVIStreamLength(StreamPointer);
		}

		public double FrameRate { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int FrameSize { get; private set; }
		public short CountBitsPerPixel { get; private set; }
		public int FirstFrame { get; private set; }
		public int CountFrames { get; private set; }

		private static short CalculateBitsPerPixel(short bitCount)
		{
			if (bitCount > 24)
				return 32;
			if (bitCount > 16)
				return 24;
			if (bitCount > 8)
				return 16;

			return (short)(bitCount > 4 ? 8 : 4);
		}

		public void GetFrameOpen()
		{
			var bih = new AviInterop.BitmapInfoHeader
			{
				biBitCount = CountBitsPerPixel,
				biPlanes = 1
			};
			bih.biSize = Marshal.SizeOf(bih);
			getFrameObject = AviInterop.AVIStreamGetFrameOpen(StreamPointer, ref bih);
			if (getFrameObject == 0)
				throw new CodecNotFoundException();

			isFrameOpen = true;
		}

		private int getFrameObject;
		private bool isFrameOpen;

		private class CodecNotFoundException : Exception {}

		public byte[] GetStreamData(int position, out AviInterop.BitmapInfoHeader header)
		{
			if (!isFrameOpen)
				throw new Exception("GetFrameOpen needs to be called before GetStreamData!");

			int dib = AviInterop.AVIStreamGetFrame(getFrameObject, FirstFrame + position);
			header = new AviInterop.BitmapInfoHeader();
			header = (AviInterop.BitmapInfoHeader)Marshal.PtrToStructure((IntPtr)dib, header.GetType());
			if (header.biSizeImage < 1)
				throw new Exception("Exception in VideoStreamGetFrame");

			int bihSize = Marshal.SizeOf(header);
			var bitmapData =
				new byte[header.biSizeImage + (header.biBitCount < 16 ? PaletteSize : 0)];
			Marshal.Copy((IntPtr)(dib + bihSize), bitmapData, 0, bitmapData.Length);

			return bitmapData;
		}

		/// <summary>
		/// RGBQUAD * 256 colours
		/// </summary>
		private const int PaletteSize = 4 * 256;

		public void GetFrameClose()
		{
			isFrameOpen = false;
			if (getFrameObject != 0)
			{
				AviInterop.AVIStreamGetFrameClose(getFrameObject);
				getFrameObject = 0;
			}
		}
	}
}