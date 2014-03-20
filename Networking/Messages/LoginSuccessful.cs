namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// After sending LoginRequest to the server this is send back if the login was successful. If the
	/// login failed an ServerError message is send back. Only after this the server other messages.
	/// </summary>
	public class LoginSuccessful
	{
		private LoginSuccessful() {}

		public LoginSuccessful(string userName, string imagePath)
		{
			UserName = userName;
			AccountImagePath = imagePath;
		}

		public string UserName { get; private set; }
		public string AccountImagePath { get; private set; }
	}
}