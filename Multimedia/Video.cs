using System;
using System.Diagnostics;
using DeltaEngine.Content;
using DeltaEngine.Entities;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Provides a way to load and play a video file.
	/// </summary>
	public abstract class Video : ContentData, Updateable
	{
		protected Video(string contentName, SoundDevice device)
			: base(contentName)
		{
			this.device = device;
		}

		protected readonly SoundDevice device;

		public void Play(float volume = 1.0f)
		{
			device.RegisterCurrentVideo(this);
			PlayNativeVideo(volume);
		}

		protected abstract void PlayNativeVideo(float volume);

		public void Stop()
		{
			device.RegisterCurrentVideo(null);
			StopNativeVideo();
		}

		protected abstract void StopNativeVideo();
		public abstract bool IsPlaying();
		public abstract void Update();
		public abstract float DurationInSeconds { get; }
		public abstract float PositionInSeconds { get; }

		protected override bool AllowCreationIfContentNotFound
		{
			get { return !Debugger.IsAttached; }
		}

		protected override void DisposeData()
		{
			Stop();
		}

		public class VideoNotFoundOrAccessible : Exception
		{
			public VideoNotFoundOrAccessible(string videoName, Exception innerException)
				: base(videoName, innerException) {}
		}
	}
}