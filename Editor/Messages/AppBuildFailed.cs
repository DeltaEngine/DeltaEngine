namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// A message which is sent from the BuildService in the case that the build of a request app has
	/// failed.
	/// </summary>
	public sealed class AppBuildFailed
	{
		// ReSharper disable UnusedMember.Local
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		private AppBuildFailed() {}
		// ReSharper restore UnusedMember.Local

		public AppBuildFailed(string reason)
		{
			Reason = reason;
		}

		public string Reason { get; private set; }

		public override string ToString()
		{
			return GetType().Name + ": " + Reason;
		}
	}
}