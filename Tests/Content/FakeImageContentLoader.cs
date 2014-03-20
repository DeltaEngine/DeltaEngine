using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Tests.Content
{
	internal class FakeImageContentLoader : ContentLoader
	{
		private FakeImageContentLoader()
		{
			ContentProjectPath = "NoPath";
		}

		public override ContentMetaData GetMetaData(string contentName, Type contentClassType = null)
		{
			if (metaData != null)
				return metaData;
			metaData = new ContentMetaData { Type = ContentType.Image };
			metaData.Values.Add("ImageName", "DeltaEngineLogo");
			metaData.Values.Add("UV", Rectangle.One.ToString());
			metaData.Values.Add("PadLeft", "0.5");
			metaData.Values.Add("PadRight", "0.5");
			metaData.Values.Add("PadTop", "0.5");
			metaData.Values.Add("PadBottom", "0.5");
			metaData.Values.Add("Rotated", "false");
			return metaData;
		}

		private ContentMetaData metaData;

		//ncrunch: no coverage start
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