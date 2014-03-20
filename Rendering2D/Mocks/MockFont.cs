using System.IO;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Rendering2D.Mocks
{
	/// <summary>
	/// Mock to quickly fake loading fonts
	/// </summary>
	public class MockFont : Font
	{
		public MockFont(string contentName)
			: base("<GeneratedMockFont:" + contentName + ">") { }

		protected override void LoadData(Stream fileData)
		{
			base.LoadData(fileData);
			InitializeDescriptionAndMaterial();
		}
	}
}