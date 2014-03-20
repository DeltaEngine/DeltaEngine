using System.IO;

namespace DeltaEngine.Multimedia.Mocks
{
	/// <summary>
	/// Mocks music to speed up unit testing of multimedia.
	/// </summary>
	public class MockMusic : Music
	{
		public MockMusic(string contentName, SoundDevice device)
			: base(contentName, device) {}

		protected override void LoadData(Stream fileData) {}

		protected override void PlayNativeMusic()
		{
			MusicStopCalled = false;
		}

		public static bool MusicStopCalled { get; private set; }

		public override void Run()
		{
			if (!IsPlaying())
				HandleStreamFinished();
		}

		protected override void StopNativeMusic()
		{
			MusicStopCalled = true;
		}

		protected override void SetPlayingVolume(float value) {}

		public override bool IsPlaying()
		{
			return !MusicStopCalled;
		}

		public override float DurationInSeconds
		{
			get { return 4.13f; }
		}

		public override float PositionInSeconds
		{
			get { return 1.0f; }
		}
	}
}