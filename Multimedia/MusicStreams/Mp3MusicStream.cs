using System.IO;
using NAudio.Wave;

namespace DeltaEngine.Multimedia.MusicStreams
{
	public class Mp3MusicStream : BaseMusicStream
	{
		public Mp3MusicStream(Stream stream)
		{
			baseStream = stream;
			mp3 = new Mp3FileReader(stream);
			lengthInSeconds = CalculateLengthInSeconds();
		}

		private Stream baseStream;
		private Mp3FileReader mp3;
		private readonly float lengthInSeconds;

		private float CalculateLengthInSeconds()
		{
			int bytesPerSample = mp3.WaveFormat.BitsPerSample / 8;
			long numberOfSamples = mp3.Length / bytesPerSample;
			long samplesPerChannel = numberOfSamples / mp3.WaveFormat.Channels;
			return (float)samplesPerChannel / mp3.WaveFormat.SampleRate;
		}

		public void Dispose()
		{
			baseStream = null;
			mp3 = null;
		}

		public int Channels
		{
			get { return mp3.WaveFormat.Channels; }
		}

		public int Samplerate
		{
			get { return mp3.WaveFormat.SampleRate; }
		}

		public float LengthInSeconds
		{
			get { return lengthInSeconds; }
		}

		public int Read(byte[] buffer, int length)
		{
			int bytesReaded = mp3.Read(buffer, 0, length);
			return bytesReaded;
		}

		public void Rewind()
		{
			baseStream.Position = 0;
			mp3.Position = 0;
		}
	}
}