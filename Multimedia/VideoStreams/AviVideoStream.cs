using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DeltaEngine.Multimedia.VideoStreams.Avi;

namespace DeltaEngine.Multimedia.VideoStreams
{
	public class AviVideoStream : BaseVideoStream
	{
		public AviVideoStream(string filepath)
		{
			this.filepath = filepath;
			aviManager = new AviFile(filepath);
			video = aviManager.GetVideoStream();
			audio = aviManager.GetAudioStream();
		}

		private AviFile aviManager;
		private readonly string filepath;
		private VideoStream video;
		private AudioStream audio;

		public int Width
		{
			get { return video.Width; }
		}

		public int Height
		{
			get { return video.Height; }
		}

		public int Channels
		{
			get { return audio.Channels; }
		}

		public int Samplerate
		{
			get { return audio.SamplesPerSecond; }
		}

		public float LengthInSeconds
		{
			get { return (float)(video.CountFrames / video.FrameRate); }
		}

		public int ReadMusicBytes(byte[] buffer, int length)
		{
			byte[] bufferData = audio.GetStreamData();
			Array.Copy(bufferData, buffer, bufferData.Length);
			return bufferData.Length;
		}

		public void Rewind()
		{
			Dispose();
			aviManager = new AviFile(filepath);
			video = aviManager.GetVideoStream();
			audio = aviManager.GetAudioStream();
		}

		public unsafe byte[] ReadImageRgbaColors(float delta)
		{
			positionInSeconds += delta;
			var frameIndex = (int)(positionInSeconds * video.FrameRate);
			try
			{
				AviInterop.BitmapInfoHeader bitmapHeader;
				byte[] pixelData = video.GetStreamData(frameIndex, out bitmapHeader);
				Bitmap conversionBmp;
				fixed (byte* ptr = &pixelData[0])
					conversionBmp = new Bitmap(Width, Height, Width * 3, PixelFormat.Format32bppRgb,
						(IntPtr)ptr);
				var result = new byte[Width * Height * 4];
				BitmapData bitmapData = conversionBmp.LockBits(new Rectangle(0, 0, Width, Height),
					ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				Marshal.Copy(bitmapData.Scan0, result, 0, result.Length);
				conversionBmp.UnlockBits(bitmapData);
				return result;
			}
			catch
			{
				return null;
			}
		}

		private float positionInSeconds;

		public void Dispose()
		{
			if (aviManager != null)
				aviManager.Close();
			aviManager = null;
		}

		public void Play()
		{
			video.GetFrameOpen();
		}

		public void Stop()
		{
			video.GetFrameClose();
			positionInSeconds = 0f;
		}
	}
}