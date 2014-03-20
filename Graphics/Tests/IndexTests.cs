using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class IndexTests
	{
		[Test]
		public void ConvertingShortResultsInSameValueAsUShort()
		{
			var indices = new List<int> { 4593, 41593, 3954593 };
			var shorts = indices.Select(index => (short)(index+1)).ToList();
			for (int num = 0; num < indices.Count; num++)
				Assert.AreEqual((ushort)indices[num]+1, (ushort)shorts[num]);
		}
	}
}