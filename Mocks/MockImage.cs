using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Mocks
{
	public sealed class MockImage : Image
	{
		public MockImage(string contentName)
			: base(contentName) {}

		public MockImage(ImageCreationData creationData)
			: base(creationData) {}

		protected override void SetSamplerStateAndTryToLoadImage(Stream fileData) {}
		protected override void TryLoadImage(Stream fileData) {} //ncrunch: no coverage

		public override void FillRgbaData(byte[] rgbaColors)
		{
			if (PixelSize.Width * PixelSize.Height * 4 != rgbaColors.Length)
				throw new InvalidNumberOfBytes(PixelSize);
		}

		public void CallCompareActualSizeMetadataSizeMethod(Size actualSize)
		{
			CompareActualSizeMetadataSize(actualSize);
		}

		protected override void SetSamplerState() {}

		protected override void DisposeData() {}

		public void CheckAlphaIsCorrect(bool hasAlpha)
		{
			WarnAboutWrongAlphaFormat(hasAlpha);
		}
	}
}