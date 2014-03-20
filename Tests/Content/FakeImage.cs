using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Tests.Content
{
	public class FakeImage : Image
	{
		//ncrunch: no coverage start
		public FakeImage(string contentName)
			: base(contentName) {}

		public FakeImage(ImageCreationData data)
			: base(data) {}

		protected override void DisposeData() {}
		protected override void SetSamplerStateAndTryToLoadImage(Stream fileData) {}
		protected override void TryLoadImage(Stream fileData) {}
		public override void FillRgbaData(byte[] rgbaColors) {}
		protected override void SetSamplerState() {}
	}
}