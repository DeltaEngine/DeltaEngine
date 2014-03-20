namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Tries to connect to the Delta Engine online service for logging, content and building. Always
	/// needs a valid ApiKey, which should be setup via the Editor. The initial project name is used
	/// for logging and content requesting, but can be changed via ChangeProject in the Editor.
	/// </summary>
	public sealed class LoginRequest
	{
		private LoginRequest() {}

		public LoginRequest(string apiKey, string initialProjectName)
		{
			ApiKey = apiKey;
			InitialProjectName = initialProjectName;
		}

		public string ApiKey { get; private set; }
		public string InitialProjectName { get; private set; }
	}
}