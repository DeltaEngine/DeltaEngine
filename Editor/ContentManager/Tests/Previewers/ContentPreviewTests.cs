using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class ContentPreviewTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Init()
		{
			contentPreview = new ConcreteContentPreview();
		}

		private ConcreteContentPreview contentPreview;

		private class ConcreteContentPreview : ContentPreview
		{
			protected override void Init()
			{
				NumberOfInitializations++;
			}

			public int NumberOfInitializations { get; private set; }

			protected override void Preview(string contentName)
			{
				if (contentName.Contains("Not"))
					throw new ContentLoader.ContentNotFound("Unavailable");
				if (Path.HasExtension(contentName))
					throw new ContentLoader.ContentNameShouldNotHaveExtension();
				if (contentName.Contains("Invalid"))
					throw new Exception();
				HasBeenPreviewed = true;
			}

			public bool HasBeenPreviewed { get; private set; }
		}

		[Test, CloseAfterFirstFrame]
		public void LazyInitializeContentPreviewersToAvoidLoadingRequiredContentTooEarly()
		{
			Assert.AreEqual(0, contentPreview.NumberOfInitializations);
			Assert.IsFalse(contentPreview.HasBeenPreviewed);
			contentPreview.PreviewContent("ValidContent");
			Assert.AreEqual(1, contentPreview.NumberOfInitializations);
			Assert.IsTrue(contentPreview.HasBeenPreviewed);
			contentPreview.PreviewContent("ValidContent");
			Assert.AreEqual(1, contentPreview.NumberOfInitializations);
		}

		[Test]
		public void TryToPreviewNotAvailableContentShowsErrorText()
		{
			contentPreview.PreviewContent("NotAvailableContent");
			Assert.IsFalse(contentPreview.HasBeenPreviewed);
		}

		[Test]
		public void TryToPreviewContentContainingAnExtensionShowsErrorText()
		{
			contentPreview.PreviewContent("NameWith.Extension");
			Assert.IsFalse(contentPreview.HasBeenPreviewed);
		}

		[Test, CloseAfterFirstFrame]
		public void TryToPreviewInvalidContentLogsWarning()
		{
			contentPreview.PreviewContent("InvalidContent");
			Assert.IsFalse(contentPreview.HasBeenPreviewed);
		}
	}
}