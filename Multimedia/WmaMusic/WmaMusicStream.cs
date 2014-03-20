using System.IO;
using AsfMojo.File;
using AsfMojo.Media;
using DeltaEngine.Multimedia.MusicStreams;

namespace DeltaEngine.Multimedia.WmaMusic
{
	public class WmaMusicStream : BaseMusicStream
	{
		public WmaMusicStream(string filepath)
		{
			this.filepath = filepath;
			var asf = new AsfFile(filepath);
			Channels = asf.PacketConfiguration.AudioChannels;
			Samplerate = (int)asf.PacketConfiguration.AudioSampleRate;
			LengthInSeconds = (float)asf.PacketConfiguration.Duration;
			asf.Open();
			soundStream = new AsfAudio(new AsfStream(asf, AsfStreamType.asfAudio, 0)).GetWaveStream();
			asf.Close();
		}

		private readonly string filepath;
		private WaveMemoryStream soundStream;
		public int Channels { get; private set; }
		public int Samplerate { get; private set; }
		public float LengthInSeconds { get; private set; }

		public void Dispose()
		{
			soundStream.Dispose();
			soundStream = null;
		}

		public int Read(byte[] buffer, int length)
		{
			return soundStream.Read(buffer, 0, length);
		}

		public void Rewind()
		{
			var asf = new AsfFile(filepath);
			asf.Open();
			soundStream = new AsfAudio(new AsfStream(asf, AsfStreamType.asfAudio, 0)).GetWaveStream();
			asf.Close();
		}

		public static bool IsWmaStream(Stream stream)
		{
			byte[] magicBytes = new byte[3];
			stream.Read(magicBytes, 0, magicBytes.Length);
			stream.Seek(0, SeekOrigin.Begin);
			return magicBytes[0] == 48 && magicBytes[1] == 38 && magicBytes[2] == 178;
		}
	}
}