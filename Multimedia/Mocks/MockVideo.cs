using System.IO;

namespace DeltaEngine.Multimedia.Mocks
{
	/// <summary>
	/// Mocks video playback to speed up unit testing.
	/// </summary>
	public class MockVideo : Video
	{
		public MockVideo(string contentName, SoundDevice soundDevice)
			: base(contentName, soundDevice) {}

		protected override void LoadData(Stream fileData) {}

		protected override void PlayNativeVideo(float volume)
		{
			VideoStopCalled = false;
			surface = new MockVideoSurface();
		}

		public static bool VideoStopCalled { get; private set; }

		private MockVideoSurface surface;

		public override void Update() {}

		protected override void StopNativeVideo()
		{
			VideoStopCalled = true;
			if (surface != null)
				surface.IsActive = false;

			surface = null;
		}

		public override bool IsPlaying()
		{
			return !VideoStopCalled;
		}

		public override float DurationInSeconds
		{
			get { return 3.33333325f; }
		}

		public override float PositionInSeconds
		{
			get { return 1.0f; }
		}
	}
}