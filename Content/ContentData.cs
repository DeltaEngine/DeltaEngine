using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Base class for all content classes. Content is loaded and cached by the ContentLoader.
	/// Content can also be part of an entity as a component. Loading and saving content components
	/// will however only store and retrieve the content name and type, but not any internal data.
	/// </summary>
	public abstract class ContentData : IDisposable
	{
		protected ContentData(string contentName)
		{
			if (string.IsNullOrEmpty(contentName))
				throw new ContentNameMissing();
#if DEBUG
			//ncrunch: no coverage start
			if (!StackTraceExtensions.StartedFromNCrunchOrNunitConsole &&
				!contentName.StartsWith("<Generated"))
			{
				StackFrame[] frames = new StackTrace().GetFrames();
				if (frames != null && frames.All(f => f.GetMethod().DeclaringType != typeof(ContentLoader)))
					throw new MustBeCalledFromContentLoader();
			} //ncrunch: no coverage end
#endif
			Name = contentName;
		}

		public class ContentNameMissing : Exception {}

		public class MustBeCalledFromContentLoader : Exception {}

		public string Name { get; private set; }

		internal bool InternalAllowCreationIfContentNotFound
		{
			get { return AllowCreationIfContentNotFound; }
		}
		protected virtual bool AllowCreationIfContentNotFound
		{
			get { return false; }
		}

		public void Dispose()
		{
			if (!IsDisposed)
				DisposeData();
			IsDisposed = true;
		}

		public bool IsDisposed { get; protected set; }
		protected abstract void DisposeData();

		internal void InternalLoad(Func<ContentData, Stream> getContentDataStream)
		{
#if DEBUG
			if (MetaData.Type != ContentType.JustStore && 
				!GetType().FullName.Contains(MetaData.Type.ToString()))
				throw new DoesNotMatchMetaDataType(this);
#endif
			IsDisposed = false;
			try
			{
				using (var stream = getContentDataStream(this))
					LoadData(stream);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (AllowCreationIfContentNotFound)
					CreateDefault();
				else
					throw;
			}
		}

		public class DoesNotMatchMetaDataType : Exception
		{
			public DoesNotMatchMetaDataType(ContentData contentData)
				: base(contentData + " does not match meta data type: " + contentData.MetaData.Type) {}
		}

		protected abstract void LoadData(Stream fileData);

		public void InternalCreateDefault()
		{
			IsDisposed = false;
			CreateDefault();
		}

		protected virtual void CreateDefault() {}

		internal void FireContentChangedEvent()
		{
			if (ContentChanged != null)
				ContentChanged();
		}

		protected Action ContentChanged;

		public ContentMetaData MetaData
		{
			get { return metaData; }
			internal set { metaData = value; }
		}
		[NonSerialized]
		private ContentMetaData metaData;

		public override string ToString()
		{
			return GetType() + ": " + Name;
		}
	}
}