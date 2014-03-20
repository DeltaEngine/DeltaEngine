namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Thrown by the server when an internal error happens (DB connection lost, wrong data received)
	/// </summary>
	public class ServerError
	{
		protected ServerError() {}

		public ServerError(string error)
		{
			Error = error;
		}

		public string Error { get; private set; }

		public override string ToString()
		{
			return "Server Error: " + Error;
		}
	}
}