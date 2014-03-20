using DeltaEngine.Core;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering3D.Cameras;

namespace DeltaEngine.Platforms
{
	internal class AutofacCameraResolver : CameraResolver
	{
		public AutofacCameraResolver(Resolver resolver)
		{
			this.resolver = resolver;
		}

		private readonly Resolver resolver;

		public Camera ResolveCamera<T>(object optionalParameter) where T : Camera
		{
			Camera camera;
			if (optionalParameter == null)
				camera =
					(T)
						resolver.Resolve(typeof(T),
							new object[] { resolver.Resolve<Device>(), resolver.Resolve<Window>() });
			else
				camera =
					(T)
						resolver.Resolve(typeof(T),
							new [] { resolver.Resolve<Device>(), resolver.Resolve<Window>() , optionalParameter});
			return camera;
		}
	}
}