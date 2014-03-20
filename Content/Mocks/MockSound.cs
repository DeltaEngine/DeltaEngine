using System.Collections.Generic;
using System.IO;
using DeltaEngine.Multimedia;

namespace DeltaEngine.Content.Mocks
{
	/// <summary>
	/// Mocks sound in unit tests.
	/// </summary>
	public class MockSound : Sound
	{
		public MockSound(string contentName)
			: base(contentName) {}

		protected override void LoadData(Stream fileData) { }

		public override float LengthInSeconds
		{
			get { return 0.48f; }
		}

		public override void PlayInstance(SoundInstance instanceToPlay)
		{
			playingInstances.Add(instanceToPlay);
		}

		private readonly List<SoundInstance> playingInstances = new List<SoundInstance>();

		public override void StopInstance(SoundInstance instanceToStop)
		{
			playingInstances.Remove(instanceToStop);
		}

		protected override void CreateChannel(SoundInstance instanceToFill) { }
		protected override void RemoveChannel(SoundInstance instanceToRemove) { }

		public override bool IsPlaying(SoundInstance instance)
		{
			return playingInstances.Contains(instance);
		}

		protected override void DisposeData()
		{
			playingInstances.Clear();
			base.DisposeData();
		}
	}
}