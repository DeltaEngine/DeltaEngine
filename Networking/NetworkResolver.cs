namespace DeltaEngine.Networking
{
	internal interface NetworkResolver
	{
		Server ResolveServer();
		Client ResolveClient();
	}
}