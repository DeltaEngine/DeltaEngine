using System;
using DeltaEngine.Editor.ContentManager.Previewers;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class MockContentViewer : ContentViewer
	{
		protected override void ShowNoPreviewText(string contentName)
		{
			throw new PreviewerNotAvailable(contentName);
		}

		public class PreviewerNotAvailable : Exception
		{
			public PreviewerNotAvailable(string contentName)
				: base(contentName) {}
		}
	}
}