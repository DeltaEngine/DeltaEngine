namespace DeltaEngine.Multimedia.Mocks
{
	/// <summary>
	/// Mocks a device to speed up unit testing of sound effects and music.
	/// </summary>
	public class MockSoundDevice : SoundDevice
	{
		public bool IsInitialized
		{
			get { return true; }
		}
	}
}