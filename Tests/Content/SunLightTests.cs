using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Content
{
	public class SunLightTests
	{
		[Test]
		public void CreateSunLight()
		{
			SunLight.Current = new SunLight(Vector3D.UnitZ, Color.White);
			Assert.AreEqual(Vector3D.UnitZ, SunLight.Current.Direction);
		}
	}
}