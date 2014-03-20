using System;
using System.IO;
using System.Text;
using Autofac.Core;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Mocks;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Platforms.Tests
{
	public class ResolverTests
	{
		[SetUp]
		public void CreateMockResolver()
		{
			resolver = new MockResolver();
		}

		private MockResolver resolver;

		[TearDown]
		public void DisposeMockResolver()
		{
			resolver.Dispose();
		}

		[Test]
		public void RegisterTypeTwice()
		{
			resolver.Register(typeof(A));
			resolver.Register(typeof(A));
		}

		[IgnoreForResolver]
		private class A {}

		[Test]
		public void RegisterSingletonTypeTwice()
		{
			resolver.RegisterSingleton(typeof(A));
			resolver.RegisterSingleton(typeof(A));
		}

		[Test]
		public void RegisterAfterInitializationThrows()
		{
			Assert.IsFalse(resolver.IsInitialized);
			resolver.Register(typeof(A));
			resolver.Resolve<A>();
			Assert.IsTrue(resolver.IsInitialized);
			Assert.Throws<Resolver.UnableToRegisterMoreTypesAppAlreadyStarted>(
				() => resolver.Register(typeof(B)));
			Assert.Throws<Resolver.UnableToRegisterMoreTypesAppAlreadyStarted>(
				() => resolver.RegisterSingleton(typeof(C)));
		}

		[IgnoreForResolver]
		private class B {}
		[IgnoreForResolver]
		private class C {}

		[Test]
		public void TryResolveWithCustomParameter()
		{
			resolver.Register(typeof(DummyClassWithString));
			Assert.DoesNotThrow(() => resolver.Resolve(typeof(DummyClassWithString), "Huhu"));
		}

		private class DummyClassWithString
		{
			// ReSharper disable UnusedParameter.Local
			public DummyClassWithString(string value) {}
			// ReSharper restore UnusedParameter.Local
		}

		[Test]
		public void TryResolveWithInvalidParameter()
		{
			resolver.Register(typeof(DummyClassWithString));
			Assert.Throws<DependencyResolutionException>(
				() => resolver.Resolve(typeof(DummyClassWithString)));
		}

		[Test]
		public void TryResolveWithContentCreationData()
		{
			var creationData = new ImageCreationData(new Size(1));
			Assert.IsNotNull(resolver.Resolve(typeof(MockImage), creationData));
		}

		[Test]
		public void ResolveSpecialTypes()
		{
			Assert.AreEqual(EntitiesRunner.Current, resolver.Resolve<EntitiesRunner>());
			Assert.AreEqual(GlobalTime.Current, resolver.Resolve<GlobalTime>());
			Assert.AreEqual(ScreenSpace.Current, resolver.Resolve<ScreenSpace>());
			Assert.AreEqual(Randomizer.Current, resolver.Resolve<Randomizer>());
		}

		[Test]
		public void RegisterInstanceMultipleTimeLogsToConsole()
		{
			var previousOut = Console.Out;
			var text = new StringBuilder();
			using (StringWriter writer = new StringWriter(text))
			{
				Console.SetOut(writer);
				resolver.RegisterInstance(new A());
				resolver.RegisterInstance(new A());
#if DEBUG
				Assert.AreEqual(
					"Warning: Type DeltaEngine.Platforms.Tests.ResolverTests+A " +
						"already exists in alreadyRegisteredTypes" + writer.NewLine, text.ToString());
#else
				Assert.AreEqual(string.Empty, text.ToString()); 
#endif
			}
			Console.SetOut(previousOut);
		}
	}
}