using System;
using System.IO;
using DeltaEngine.Content.Mocks;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Content.Tests
{
	public class MockImageTests
	{
		[SetUp]
		public void CreateContentLoaderInstance()
		{
			ContentLoader.Use<MockContentLoader>();
		}

		[TearDown]
		public void DisposeContentLoader()
		{
			ContentLoader.DisposeIfInitialized();
		}

		[Test]
		public void FillImageWithColor()
		{
			var imageCreationData = new ImageCreationData(new Size(12, 12));
			var image = ContentLoader.Create<MockImage>(imageCreationData);
			image.Fill(Color.Blue);
			image.CallCompareActualSizeMetadataSizeMethod(new Size(12, 12));
			// For some reason LogWarningIfTheActualSizeIsDifferentFromTheMetadataPixelSize does not cover
			image.CallCompareActualSizeMetadataSizeMethod(new Size(16, 16));
		}


		[Test]
		public void LoadContentViaCreationData()
		{
			var imageCreationData = new ImageCreationData(new Size(12, 12));
			var image = ContentLoader.Create<MockImage>(imageCreationData);
			Assert.AreEqual(imageCreationData.PixelSize, image.PixelSize);
			Assert.AreEqual(imageCreationData.BlendMode, image.BlendMode);
			Assert.AreEqual(imageCreationData.UseMipmaps, image.UseMipmaps);
			Assert.AreEqual(imageCreationData.AllowTiling, image.AllowTiling);
			Assert.AreEqual(imageCreationData.DisableLinearFiltering, image.DisableLinearFiltering);
		}

		[Test]
		public void CheckWarningForAlpha()
		{
			var imageCreationData = new ImageCreationData(new Size(12, 12));
			var image = ContentLoader.Create<MockImage>(imageCreationData);
			image.BlendMode = BlendMode.Normal;
			var mockLogger = new MockLogger();
			image.CheckAlphaIsCorrect(false);
			Assert.IsTrue(
				mockLogger.LastMessage.Contains(
					"is supposed to have alpha pixels, but the image pixel format is not using alpha."));
			image.BlendMode = BlendMode.Opaque;
			image.CheckAlphaIsCorrect(true);
			Assert.IsTrue(
				mockLogger.LastMessage.Contains(
					"is supposed to have no alpha pixels, but the image pixel format is using alpha."));
		}

		[Test]
		public void LogWarningIfTheActualSizeIsDifferentFromTheMetadataPixelSize()
		{
			var logger = new MockLogger();
			var imageCreationData = new ImageCreationData(new Size(12, 12));
			var image = ContentLoader.Create<MockImage>(imageCreationData);
			image.CallCompareActualSizeMetadataSizeMethod(new Size(16, 16));
			Assert.IsTrue(logger.LastMessage.Contains("different from the MetaData PixelSize"));
		}

		[Test]
		public void LogErrorIfLoadDataHasFailed()
		{
			var logger = new MockLogger();
			ContentLoader.Load<ImageWithFailingLoadData>("DefaultImage");
			Assert.IsTrue(logger.LastMessage.Contains("TestError"));
		}

		private class ImageWithFailingLoadData : Image
		{
			public ImageWithFailingLoadData(string contentName)
				: base(contentName) {}

			protected override void DisposeData() {} // ncrunch: no coverage
			protected override void SetSamplerStateAndTryToLoadImage(Stream fileData)
			{
				LoadImage(fileData);
			}

			protected override void TryLoadImage(Stream fileData)
			{
				throw new Exception("TestError");
			}

			public override void FillRgbaData(byte[] rgbaColors) {}	// ncrunch: no coverage
			protected override void SetSamplerState() {}
		}
	}
}