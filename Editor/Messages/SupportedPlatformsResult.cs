namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// A message which is sent to the BuildService to know which Platforms are supported.
	/// </summary>
	public sealed class SupportedPlatformsResult
	{
		// ReSharper disable UnusedMember.Local
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		private SupportedPlatformsResult() { }
		// ReSharper restore UnusedMember.Local

		public SupportedPlatformsResult(PlatformName[] platforms)
		{
			Platforms = platforms;
		}

		public PlatformName[] Platforms { get; private set; }
	}
}