using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Content
{
	public class FakeImageContentLoaderTests
	{
		[SetUp]
		public void CreateContentLoader() {}

		[TearDown]
		public void DisposeContentLoader()
		{
			ContentLoader.DisposeIfInitialized();
		}

		[Test]
		public void LoadDefaultImage()
		{
			ContentLoader.resolver = new FakeImageResolver();
			ContentLoader.Use<FakeImageContentLoader>();
			ContentLoader.Load<MockFakeImage>("Verdana12Font");
		}

		public class FakeImageResolver : ContentLoaderResolver
		{
			public override ContentData Resolve(Type contentType, string contentName)
			{
				return new MockFakeImage(contentName);
			}
		}

		public sealed class MockFakeImage : Image
		{
			public MockFakeImage(string contentName)
				: base(contentName) {}

			protected override void SetSamplerStateAndTryToLoadImage(Stream fileData) {}

			//ncrunch: no coverage start
			protected override void TryLoadImage(Stream fileData) {}
			public override void FillRgbaData(byte[] rgbaColors) {}
			protected override void SetSamplerState() {}
			//ncrunch: no coverage end

			protected override void DisposeData()
			{
				ContentLoader.current.GetMetaData(Name, GetType()).Values.Remove("ImageName");
			}
		}
	}
}