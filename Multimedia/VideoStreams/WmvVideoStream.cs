using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using AsfMojo.File;
using AsfMojo.Media;
using DeltaEngine.Multimedia.VideoStreams.Wmv;

namespace DeltaEngine.Multimedia.VideoStreams
{
	public class WmvVideoStream : BaseVideoStream
	{
		public WmvVideoStream(string filepath)
		{
			this.filepath = filepath;
			asf = new AsfFile(filepath);
			var audioStream = new AsfStream(asf, AsfStreamType.asfAudio, 0);
			soundStream = new AsfAudio(audioStream).GetWaveStream();
			video = new AsfImageLoader(asf);
		}

		private readonly string filepath;
		private WaveMemoryStream soundStream;
		private AsfFile asf;
		private AsfImageLoader video;

		public int Width
		{
			get { return asf.PacketConfiguration.ImageWidth; }
		}

		public int Height
		{
			get { return asf.PacketConfiguration.ImageHeight; }
		}

		public int Channels
		{
			get { return asf.PacketConfiguration.AudioChannels; }
		}

		public int Samplerate
		{
			get { return (int)asf.PacketConfiguration.AudioSampleRate; }
		}

		public float LengthInSeconds
		{
			get { return (float)asf.PacketConfiguration.Duration; }
		}

		public int ReadMusicBytes(byte[] buffer, int length)
		{
			return soundStream.Read(buffer, 0, length);
		}

		public void Rewind()
		{
			Dispose();
			asf = new AsfFile(filepath);
			var audioStream = new AsfStream(asf, AsfStreamType.asfAudio, 0);
			soundStream = new AsfAudio(audioStream).GetWaveStream();
			video = new AsfImageLoader(asf);
		}

		public void Dispose()
		{
			if (soundStream != null)
				soundStream.Dispose();
			soundStream = null;
			if (video != null)
				video.Dispose();
			video = null;
			if (asf != null)
				asf.Dispose();
			asf = null;
		}

		public byte[] ReadImageRgbaColors(float delta)
		{
			Bitmap conversionBmp = video.GetImageData(delta);
			if (conversionBmp == null)
				return null;
			var result = new byte[Width * Height * 4];
			BitmapData bitmapData = conversionBmp.LockBits(new Rectangle(0, 0, Width, Height),
				ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			Marshal.Copy(bitmapData.Scan0, result, 0, result.Length);
			conversionBmp.UnlockBits(bitmapData);
			return result;
		}

		public void Play() {}
		public void Stop() {}

		public static bool IsWmvStream(Stream stream)
		{
			byte[] magicBytes = new byte[3];
			stream.Read(magicBytes, 0, magicBytes.Length);
			stream.Seek(0, SeekOrigin.Begin);
			return magicBytes[0] == 48 && magicBytes[1] == 38 && magicBytes[2] == 178;
		}
	}
}