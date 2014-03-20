using System.IO;
using DeltaEngine.Extensions;
using NVorbis;

namespace DeltaEngine.Multimedia.MusicStreams
{
	public class OggMusicStream : BaseMusicStream
	{
		public OggMusicStream(Stream stream)
		{
			baseStream = stream;
			reader = new VorbisReader(stream, false);
		}

		private readonly Stream baseStream;
		private VorbisReader reader;

		public void Dispose()
		{
			reader.Dispose();
			reader = null;
		}

		public int Channels
		{
			get { return reader.Channels; }
		}

		public int Samplerate
		{
			get { return reader.SampleRate; }
		}

		public float LengthInSeconds
		{
			get { return (float)reader.TotalTime.TotalSeconds; }
		}

		public int Read(byte[] buffer, int length)
		{
			float[] sampleBuffer = new float[length / 2];
			int count = reader.ReadSamples(sampleBuffer, 0, sampleBuffer.Length);
			int targetIndex = 0;
			for (int i = 0; i < count; i++)
			{
				short sample = FloatToShortRange(sampleBuffer[i]);
				buffer[targetIndex++] = (byte)(sample & 0x00FF);
				buffer[targetIndex++] = (byte)((sample >> 8) & 0x00FF);
			}

			return count * 2;
		}

		private static short FloatToShortRange(float value)
		{
			int temp = (int)(short.MaxValue * value);
			return (short)temp.Clamp(short.MinValue, short.MaxValue);
		}

		public void Rewind()
		{
			baseStream.Position = 0;
			reader = new VorbisReader(baseStream, false);
		}

		public static bool IsOggStream(Stream stream)
		{
			byte[] magicBytes = new byte[3];
			stream.Read(magicBytes, 0, magicBytes.Length);
			stream.Seek(0, SeekOrigin.Begin);
			return magicBytes[0] == 'O' && magicBytes[1] == 'g' && magicBytes[2] == 'g';
		}
	}
}