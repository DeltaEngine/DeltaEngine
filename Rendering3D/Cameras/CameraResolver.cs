namespace DeltaEngine.Rendering3D.Cameras
{
	internal interface CameraResolver
	{
		Camera ResolveCamera<T>(object optionalParameter) where T : Camera;
	}
}