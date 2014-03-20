using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public abstract class ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			try
			{
				TryPreviewContent(contentName);
			}
			catch (ContentLoader.ContentNotFound ex)
			{
				LogAndDisplayMessage("Content " + ex.Message + " in " + contentName + " not found");
			}
			catch (ContentLoader.ContentNameShouldNotHaveExtension)
			{
				LogAndDisplayMessage("Content " + contentName +
					" has a '.' in the ContentName. This is not allowed for it will be viewed as an extension");
			}
			catch (Exception)
			{
				LogUnableToLoadContentMessage(contentName);
			}
		}

		private void TryPreviewContent(string contentName)
		{
			if (!isInitialized)
				LazyInitialize();
			Preview(contentName);
		}

		private bool isInitialized;

		private void LazyInitialize()
		{
			isInitialized = true;
			Init();
		}

		protected abstract void Init();

		protected abstract void Preview(string contentName);

		private static void LogAndDisplayMessage(string message)
		{
			Logger.Warning(message);
			new FontText(ContentLoader.Load<Font>("Verdana12"), message, Rectangle.One);
		}

		protected static void LogUnableToLoadContentMessage(string contentName)
		{
			Logger.Warning("Could not load " + contentName + ". Check if all content is available");
		}
	}
}