using DeltaEngine.Core;
using DeltaEngine.Entities;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Holds the audio device and automatically disposes all finished playing sound instances.
	/// </summary>
	public abstract class SoundDevice : Entity, RapidUpdateable
	{
		public virtual void RapidUpdate()
		{
			if (currentPlayingMusic != null)
				currentPlayingMusic.Run();
			if (currentPlayingVideo != null)
				currentPlayingVideo.Update();
		}

		private Music currentPlayingMusic;
		private Video currentPlayingVideo;

		public void RegisterCurrentMusic(Music music)
		{
			if (IsStopNeededForRegister(music, currentPlayingMusic))
				currentPlayingMusic.Stop();
			currentPlayingMusic = music;
		}

		private static bool IsStopNeededForRegister<T>(T newInstance, T currentInstance)
			where T : class
		{
			return newInstance != null && currentInstance != null;
		}

		public void RegisterCurrentVideo(Video video)
		{
			if (IsStopNeededForRegister(video, currentPlayingVideo))
				currentPlayingVideo.Stop();
			currentPlayingVideo = video;
		}

		public override void Dispose()
		{
			if (currentPlayingMusic != null)
				currentPlayingMusic.Dispose();
			currentPlayingMusic = null;
			if (currentPlayingVideo != null)
				currentPlayingVideo.Dispose();
			currentPlayingVideo = null;
		}

		public override bool IsPauseable
		{
			get { return false; }
		}

		public float MusicVolume
		{
			get { return Settings.Current.MusicVolume; }
			set
			{
				Settings.Current.MusicVolume = value;
				if (currentPlayingMusic != null)
					currentPlayingMusic.Volume = value; //ncrunch: no coverage
			}
		}
	}
}