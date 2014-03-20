using System.IO;
using DeltaEngine.Content.Xml;

namespace DeltaEngine.Content.Mocks
{
	/// <summary>
	/// Mocks xml content in unit tests.
	/// </summary>
	public class MockXmlContent : XmlContent
	{
		public MockXmlContent(string contentName)
			: base(contentName)
		{
			ContentChanged += () => changeCounter++;
		}

		public int changeCounter;

		protected override void LoadData(Stream fileData)
		{
			LoadCounter++;
			Data = new XmlData("Root");
			Data.AddChild(new XmlData("Hi"));
		}

		public int LoadCounter { get; private set; }

		protected override void CreateDefault()
		{
			base.CreateDefault();
			Data = new XmlData("Default");
		}
	}
}