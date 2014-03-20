using System.IO;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.Messages.Tests
{
	public static class MessageTestExtensions
	{
		public static void AssertObjectWhenSavedAndRestoredByToString<T>(T anyObject)
		{
			MemoryStream serializedData = BinaryDataExtensions.SaveToMemoryStream(anyObject);
			var restoredMessage = (T)serializedData.CreateFromMemoryStream();
			Assert.AreEqual(anyObject.ToString(), restoredMessage.ToString());
		}
	}
}