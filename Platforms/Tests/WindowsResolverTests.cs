using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Mocks;
using DeltaEngine.Mocks;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class WindowsResolverTests
	{
		//ncrunch: no coverage start
		[TestFixtureSetUp]
		public void CreateWindowsResolver()
		{
			resolver = new EmptyWindowsResolver();
			Assert.NotNull(resolver);
		}

		private EmptyWindowsResolver resolver;

		[TestFixtureTearDown]
		public void DisposeWindowsResolver()
		{
			resolver.Dispose();
		}

		private class EmptyWindowsResolver : AppRunner
		{
			public void Register(object instance)
			{
				RegisterInstance(instance);
			}

			protected override void RegisterMediaTypes() {}
			protected override void RegisterPhysics() {}
		}

		[Test, Category("Slow")]
		public void RegisterNonRenderableObject()
		{
			var rect = new Rectangle(Vector2D.Half, Size.Half);
			resolver.Register(rect);
		}

		[Test, Category("Slow")]
		public void RegisterRenderableObject()
		{
			if (!StackTraceExtensions.IsStartedFromNCrunch())
				return;
			using (var device = new MockDevice(new MockWindow()))
			{
				device.Clear();
				device.Present();
				resolver.Register(device);
				resolver.RegisterSingleton<Drawing>();
				resolver.Register(new Line2D(Vector2D.One, Vector2D.Zero, Color.Red));
			}
		}
	}
}