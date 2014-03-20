using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using DeltaEngine.Multimedia.VlcToTexture;

namespace DeltaEngine.Multimedia.VideoStreams
{
	public class VlcVideoStream : BaseVideoStream
	{
		public VlcVideoStream(string filepath)
		{
			this.filepath = filepath;
			if (player == null)
			{
				factory = new Factory();
				player = factory.CreatePlayer();
				renderer = player.Renderer;
			}
			media = factory.CreateMedia(filepath);
		}

		private readonly string filepath;
		private static Factory factory;
		private static VideoPlayer player;
		private static Renderer renderer;
		private Media media;

		public int Width
		{
			get { return 800; }
		}

		public int Height
		{
			get { return 600; }
		}

		public int Channels
		{
			get { return 2; }
		}

		public int Samplerate
		{
			get { return 44100; }
		}

		public float LengthInSeconds
		{
			get { return media.Duration * 0.001f; }
		}

		public int ReadMusicBytes(byte[] buffer, int length)
		{
			buffer[0] = 0;
			return 1;
		}

		public void Rewind()
		{
			Dispose();
			media = factory.CreateMedia(filepath);
		}

		public void Dispose()
		{
			if (media != null)
				media.Dispose();
			media = null;
		}

		public byte[] ReadImageRgbaColors(float delta)
		{
			Bitmap conversionBmp = renderer.CurrentFrame;
			if (conversionBmp == null || media.State == MediaState.Ended)
				return null;
			byte[] result = new byte[Width * Height * 4];
			BitmapData bitmapData = conversionBmp.LockBits(new Rectangle(0, 0, Width, Height),
				ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			Marshal.Copy(bitmapData.Scan0, result, 0, result.Length);
			conversionBmp.UnlockBits(bitmapData);
			conversionBmp.Dispose();
			return result;
		}

		public void Play()
		{
			player.Open(media);
			renderer.SetFormat(new BitmapFormat(800, 600, ChromaType.RV32));
			player.Play();
		}

		public void Stop()
		{
			player.Stop();
		}

		public static bool IsMp4Stream(Stream stream)
		{
			byte[] magicBytes = new byte[8];
			stream.Read(magicBytes, 0, magicBytes.Length);
			stream.Seek(0, SeekOrigin.Begin);
			return magicBytes[0] == 0 && magicBytes[1] == 0 && magicBytes[2] == 0 && magicBytes[3] != 0 &&
				magicBytes[4] == 102 && magicBytes[5] == 116 && magicBytes[6] == 121 &&
				magicBytes[7] == 112;
		}
	}
}