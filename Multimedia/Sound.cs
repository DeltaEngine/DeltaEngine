using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;

namespace DeltaEngine.Multimedia
{
	/// <summary>
	/// Provides a way to load and play simple sound effects by creating a SoundInstance per play.
	/// </summary>
	public abstract class Sound : ContentData
	{
		protected Sound(string contentName)
			: base(contentName) {}

		public abstract float LengthInSeconds { get; }
		public int NumberOfInstances
		{
			get { return internalInstances.Count + externalInstances.Count; }
		}

		private readonly List<SoundInstance> internalInstances = new List<SoundInstance>();
		private readonly List<SoundInstance> externalInstances = new List<SoundInstance>();

		public int NumberOfPlayingInstances
		{
			get
			{
				return internalInstances.Count(instance => instance.IsPlaying) + 
					externalInstances.Count(instance => instance.IsPlaying);
			}
		}

		protected override bool AllowCreationIfContentNotFound
		{
			get { return !Debugger.IsAttached; }
		}

		protected override void DisposeData()
		{
			foreach (var instance in internalInstances)
				RemoveChannel(instance);
			internalInstances.Clear();
			foreach (var instance in externalInstances)
				RemoveChannel(instance);
			externalInstances.Clear();
		}

		internal void Remove(SoundInstance instanceToRemove)
		{
			internalInstances.Remove(instanceToRemove);
			externalInstances.Remove(instanceToRemove);
			RemoveChannel(instanceToRemove);
		}

		public void Play()
		{
			Play(Settings.Current.SoundVolume);
		}

		public void Play(float volume, float panning = 0.0f)
		{
			SoundInstance freeInstance = GetInternalNonPlayingInstance();
			//ncrunch: no coverage start
			if (freeInstance == null)
			{
				if (!warnedAboutTooManyInstances)
				{
					warnedAboutTooManyInstances = true;
					Logger.Warning("Too many SoundInstances " + MaxInstances + " of '" + this +
						"' have been started, will not create more and wait until they are free to use again.");
				}
				return;
			}
			//ncrunch: no coverage end
			freeInstance.Volume = volume;
			freeInstance.Panning = panning;
			freeInstance.Play();
		}

		private bool warnedAboutTooManyInstances;

		private SoundInstance GetInternalNonPlayingInstance()
		{
			SoundInstance freeInstance = internalInstances.FirstOrDefault(i => !IsPlaying(i));
			if (freeInstance != null || internalInstances.Count >= MaxInstances)
				return freeInstance;
			createInternalInstance = true;
			return CreateSoundInstance();
		}

		private const int MaxInstances = 16;

		private bool createInternalInstance;

		public SoundInstance CreateSoundInstance()
		{
			var instance = new SoundInstance(this) { Volume = Settings.Current.SoundVolume };
			Add(instance);
			return instance;
		}

		public abstract void PlayInstance(SoundInstance instanceToPlay);

		public void StopAll()
		{
			foreach (var instance in internalInstances)
				instance.Stop();
			foreach (var instance in externalInstances)
				instance.Stop();
		}

		public abstract void StopInstance(SoundInstance instanceToStop);

		protected abstract void CreateChannel(SoundInstance instanceToFill);
		protected abstract void RemoveChannel(SoundInstance instanceToRemove);

		internal void Add(SoundInstance instanceToAdd)
		{
			if (createInternalInstance)
				internalInstances.Add(instanceToAdd);
			else
				externalInstances.Add(instanceToAdd);
			createInternalInstance = false;
			CreateChannel(instanceToAdd);
		}

		public abstract bool IsPlaying(SoundInstance instance);

		public bool IsAnyInstancePlaying
		{
			get { return internalInstances.Any(IsPlaying) || externalInstances.Any(IsPlaying); }
		}

		internal void RaisePlayEvent(SoundInstance instance)
		{
			if (OnPlay != null)
				OnPlay(instance);
		}

		internal void RaiseStopEvent(SoundInstance instance)
		{
			if (OnStop != null)
				OnStop(instance);
		}

		public event Action<SoundInstance> OnPlay;
		public event Action<SoundInstance> OnStop;

		//ncrunch: no coverage start
		public class SoundNotFoundOrAccessible : Exception
		{
			public SoundNotFoundOrAccessible(string soundName, Exception innerException)
				: base(soundName, innerException) { }
		}
	}
}