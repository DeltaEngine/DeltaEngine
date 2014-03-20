using System.IO;

namespace DeltaEngine.Multimedia.MusicStreams
{
	public sealed class MusicStreamFactory
	{
		public BaseMusicStream Load(Stream stream)
		{
			if (OggMusicStream.IsOggStream(stream))
				return new OggMusicStream(stream);
			return new Mp3MusicStream(stream);
		}
	}
}