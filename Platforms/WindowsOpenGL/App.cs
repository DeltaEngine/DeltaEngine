using DeltaEngine.Core;

namespace DeltaEngine.Platforms
{
	public abstract class App
	{
		private readonly OpenGLResolver resolver = new OpenGLResolver();

		protected App() {}

		protected App(Window windowToRegister)
		{
			resolver.RegisterInstance(windowToRegister);
		}

		protected void Run()
		{
			resolver.Run();
			resolver.Dispose();
		}

		internal protected T Resolve<T>()
			where T : class
		{
			return resolver.Resolve<T>();
		}
	}
}