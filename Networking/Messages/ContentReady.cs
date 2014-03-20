namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Server sends this to the Client once all important Content has been sent and the app can start
	/// </summary>
	public class ContentReady : ContentMessage {}
}