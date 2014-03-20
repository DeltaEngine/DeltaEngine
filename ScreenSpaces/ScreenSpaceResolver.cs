namespace DeltaEngine.ScreenSpaces
{
	internal interface ScreenSpaceResolver
	{
		ScreenSpace ResolveScreenSpace<T>() where T : ScreenSpace;
	}
}