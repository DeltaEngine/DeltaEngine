using System;
using DeltaEngine.Extensions;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Provides a way to play the same sound effect multiple times at once and keeping track of
	/// the playing sounds in the Device. Instance can change its volume or panning and be replayed.
	/// </summary>
	public sealed class SoundInstance : IDisposable
	{
		internal SoundInstance(Sound sound)
		{
			this.sound = sound;
			Volume = 1.0f;
			Panning = 0.0f;
			Pitch = 1.0f;
		}

		public float Panning { get; set; }
		public float Volume { get; set; }
		public float Pitch { get; set; }
		private readonly Sound sound;
		public object Handle { get; set; }

		public void Dispose()
		{
			sound.Remove(this);
		}

		public void Play()
		{
			sound.PlayInstance(this);
			sound.RaisePlayEvent(this);
			startPlayTime = DateTime.Now;
		}

		private DateTime startPlayTime;

		public bool IsPlaying
		{
			get { return sound.IsPlaying(this); }
		}

		public float PositionInSeconds
		{
			get
			{
				if (!IsPlaying)
					return 0f;

				float elapsedSeconds = (float)DateTime.Now.Subtract(startPlayTime).TotalSeconds;
				return elapsedSeconds.Clamp(0f, sound.LengthInSeconds);
			}
		}

		public void Stop()
		{
			sound.StopInstance(this);
			sound.RaiseStopEvent(this);
		}
	}
}