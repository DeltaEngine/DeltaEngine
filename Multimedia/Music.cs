using System;
using System.Diagnostics;
using DeltaEngine.Content;
using DeltaEngine.Core;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Provides a way to load and play music files. Usually setup in Scenes.
	/// </summary>
	public abstract class Music : ContentData
	{
		protected Music(string contentName, SoundDevice device)
			: base(contentName)
		{
			this.device = device;
			Loop = false;
			cachedVolume = Settings.Current.MusicVolume;
		}

		protected readonly SoundDevice device;
		protected float cachedVolume;

		public void Play()
		{
			device.RegisterCurrentMusic(this);
			PlayNativeMusic();
			SetPlayingVolume(cachedVolume);
		}

		public void Play(float volume)
		{
			device.RegisterCurrentMusic(this);
			PlayNativeMusic();
			Volume = volume;
		}

		protected abstract void PlayNativeMusic();

		public void Stop()
		{
			device.RegisterCurrentMusic(null);
			StopNativeMusic();
		}

		protected abstract void StopNativeMusic();
		public abstract bool IsPlaying();
		public abstract void Run();

		public abstract float DurationInSeconds { get; }
		public abstract float PositionInSeconds { get; }
		protected const int NumberOfBuffers = 2;

		protected override bool AllowCreationIfContentNotFound
		{
			get { return !Debugger.IsAttached; }
		}

		protected override void DisposeData()
		{
			Stop();
		}

		protected void HandleStreamFinished()
		{
			if (StreamFinished != null)
				StreamFinished();
			if (Loop)
				Play(cachedVolume);
			else
				Stop();
		}

		public Action StreamFinished;

		public float Volume
		{
			get { return cachedVolume; }
			set
			{
				cachedVolume = value;
				if (IsPlaying())
					SetPlayingVolume(cachedVolume);
			}
		}

		protected abstract void SetPlayingVolume(float value);

		public bool Loop { protected get; set; }

		//ncrunch: no coverage start
		public class CouldNotLoadMusicFromFilestream : Exception
		{
			public CouldNotLoadMusicFromFilestream(string musicName, Exception innerException)
				: base(musicName, innerException) {}
		}
	}
}