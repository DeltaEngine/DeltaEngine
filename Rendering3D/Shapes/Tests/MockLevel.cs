using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;

namespace DeltaEngine.Rendering3D.Shapes.Tests
{
	public class MockLevel : Level
	{
		//ncrunch: no coverage start
		protected MockLevel(string contentName)
			: base(contentName) { }
		//ncrunch: no coverage end

		public MockLevel(Size size)
			: base(size) {}

		public new void LoadData(Stream fileData)
		{
			base.LoadData(fileData);
		}
	}
}