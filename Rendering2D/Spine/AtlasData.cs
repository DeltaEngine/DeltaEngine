using System.IO;
using DeltaEngine.Content;

namespace DeltaEngine.Rendering2D.Spine
{
	internal class AtlasData : ContentData
	{
		public AtlasData(string contentName)
			: base(contentName) {}

		protected override void LoadData(Stream fileData)
		{
			text = new StreamReader(fileData).ReadToEnd();
		}

		private string text;

		public TextReader TextReader
		{
			get { return new StringReader(text); }
		}

		protected override void DisposeData()
		{
			TextReader.Dispose();
		}
	}
}