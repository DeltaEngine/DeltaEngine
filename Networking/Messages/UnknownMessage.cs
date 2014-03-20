namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Send back whenever someone does not understand a message and it is not handled in other ways
	/// </summary>
	public sealed class UnknownMessage : TextMessage
	{
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		private UnknownMessage() {}

		public UnknownMessage(string error)
			: base(error) {}
	}
}