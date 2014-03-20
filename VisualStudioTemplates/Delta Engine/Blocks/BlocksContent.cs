using System;
using DeltaEngine.Content;

namespace $safeprojectname$
{
	/// <summary>
	/// Allows a prefix to be applied to the name of each content item prior to loading.
	/// Games can therefore swap mods/skins simply by switching this prefix.
	/// </summary>
	public abstract class BlocksContent
	{
		protected BlocksContent(string prefix = "",
			bool doBricksSplitInHalfWhenRowFull = false)
		{
			Prefix = prefix;
			DoBricksSplitInHalfWhenRowFull = doBricksSplitInHalfWhenRowFull;
			AreFiveBrickBlocksAllowed = true;
			DoBlocksStartInARandomColumn = false;
		}

		public string Prefix { get; set; }
		public bool DoBricksSplitInHalfWhenRowFull { get; set; }
		public bool AreFiveBrickBlocksAllowed { get; set; }
		public bool DoBlocksStartInARandomColumn { get; set; }

		public T Load<T>(string contentName) where T : ContentData
		{
			return ContentLoader.Load<T>(Prefix + contentName);
		}

		public string GetFilenameWithoutPrefix(string filenameWithPrefix)
		{
			if (!filenameWithPrefix.StartsWith(Prefix))
				throw new FilenameWrongPrefixException();
			return filenameWithPrefix.Substring(Prefix.Length);
		}

		public class FilenameWrongPrefixException : Exception {}
	}
}