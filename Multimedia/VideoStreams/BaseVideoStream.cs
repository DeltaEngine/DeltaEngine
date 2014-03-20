using System;

namespace DeltaEngine.Multimedia.VideoStreams
{
	/// <summary>
	/// Interface for a simple video stream.
	/// </summary>
	public interface BaseVideoStream : IDisposable
	{
		int Width { get; }
		int Height { get; }
		int Channels { get; }
		int Samplerate { get; }
		float LengthInSeconds { get; }

		int ReadMusicBytes(byte[] buffer, int length);
		byte[] ReadImageRgbaColors(float delta);
		void Rewind();
		void Play();
		void Stop();
	}
}