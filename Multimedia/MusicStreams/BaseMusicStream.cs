using System;

namespace DeltaEngine.Multimedia.MusicStreams
{
	/// <summary>
	/// Interface for a simple music stream.
	/// </summary>
	public interface BaseMusicStream : IDisposable
	{
		int Channels { get; }
		int Samplerate { get; }
		float LengthInSeconds { get; }
		int Read(byte[] buffer, int length);
		void Rewind();
	}
}