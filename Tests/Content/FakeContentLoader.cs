using System;
using DeltaEngine.Content;

namespace DeltaEngine.Tests.Content
{
	internal class FakeContentLoader : ContentLoader
	{
		private FakeContentLoader()
		{
			ContentProjectPath = "NoPath";
		}

		public override ContentMetaData GetMetaData(string contentName, Type contentClassType = null)
		{
			var metaData = new ContentMetaData { Type = ContentType.Xml };
			if (contentName.Contains("WrongPath"))
				metaData.LocalFilePath = "No.xml";
			return metaData;
		}

		// ncrunch: no coverage start
		protected override bool HasValidContentAndMakeSureItIsLoaded()
		{
			return true;
		}

		public override DateTime LastTimeUpdated
		{
			get { return DateTime.Now; }
		}
	}
}