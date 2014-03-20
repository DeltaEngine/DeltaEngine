using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Mocks;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Tests.Content
{
	public class MaterialTests
	{
		[SetUp]
		public void CreateContentLoader()
		{
			ContentLoader.Use<FakeContentLoader>();
			new MockSettings();
			ScreenSpace.Current = new QuadraticScreenSpace(new MockWindow());
			var shader =
				ContentLoader.Create<FakeShader>(new ShaderCreationData(ShaderFlags.Position2DColored));
			var image = ContentLoader.Create<FakeImage>(new ImageCreationData(new Size(100)));
			material = new Material(shader, image);
		}

		private Material material;

		[TearDown]
		public void DisposeContentLoader()
		{
			ContentLoader.DisposeIfInitialized();
		}

		[Test]
		public void TestPixelBasedRenderSize()
		{
			material.RenderSizeMode = RenderSizeMode.PixelBased;
			Assert.AreEqual(0.3125f, material.MaterialRenderSize.Width);
			Assert.AreEqual(0.3125f, material.MaterialRenderSize.Height);
		}

		[Test]
		public void Test800X480RenderSize()
		{
			material.RenderSizeMode = RenderSizeMode.SizeFor800X480;
			Assert.AreEqual(0.125f, material.MaterialRenderSize.Width);
			Assert.AreEqual(0.125f, material.MaterialRenderSize.Height);
		}

		[Test]
		public void Test1024X720RenderSize()
		{
			material.RenderSizeMode = RenderSizeMode.SizeFor1024X768;
			Assert.AreEqual(0.09765625f, material.MaterialRenderSize.Width);
			Assert.AreEqual(0.09765625f, material.MaterialRenderSize.Height);
		}

		[Test]
		public void Test1280X720RenderSize()
		{
			material.RenderSizeMode = RenderSizeMode.SizeFor1280X720;
			Assert.AreEqual(0.078125f, material.MaterialRenderSize.Width);
			Assert.AreEqual(0.078125f, material.MaterialRenderSize.Height);
		}

		[Test]
		public void Test1920X1080RenderSize()
		{
			material.RenderSizeMode = RenderSizeMode.SizeFor1920X1080;
			Assert.AreEqual(0.0520833321f, material.MaterialRenderSize.Width);
			Assert.AreEqual(0.0520833321f, material.MaterialRenderSize.Height);
		}

		[Test]
		public void TestSettingsBasedRenderSize()
		{
			material.RenderSizeMode = RenderSizeMode.SizeForSettingsResolution;
			AssertMaterialRenderSize(material,
				StackTraceExtensions.StartedFromNCrunchOrNunitConsole
					? 0.3125f : ExceptionExtensions.IsDebugMode ? 0.15625f : 0.078125f);
		}

		private static void AssertMaterialRenderSize(Material material, float dimension)
		{
			Assert.AreEqual(dimension, material.MaterialRenderSize.Width);
			Assert.AreEqual(dimension, material.MaterialRenderSize.Height);
		}
	}
}