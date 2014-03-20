using System.IO;

namespace DeltaEngine.Multimedia.VideoStreams
{
	public sealed class VideoStreamFactory
	{
		public BaseVideoStream Load(Stream stream, string filepath)
		{
			if (WmvVideoStream.IsWmvStream(stream))
				return new WmvVideoStream(filepath + ".wmv");
			if (VlcVideoStream.IsMp4Stream(stream))
				return new VlcVideoStream(filepath + ".mp4");
			return new VlcVideoStream(filepath + ".avi");
		}
	}
}