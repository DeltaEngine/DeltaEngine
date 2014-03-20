using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Content.Json.Tests
{
	[Category("Slow")]
	public class JsonFileTests
	{
		//ncrunch: no coverage start
		private const string TestFilename = "Test.json";

		[SetUp]
		public void CreateFileToLoad()
		{
			if (File.Exists(TestFilename))
				return;

			var newFile = File.CreateText(TestFilename);
			newFile.WriteLine(@"{
	""SomeData"":6
}");
			newFile.Close();
		}

		[Test]
		public void LoadNodeFromTestJson()
		{
			var file = new JsonFile(TestFilename);
			Assert.AreEqual(6, file.Root.Get<int>("SomeData"));
		}

		[Test]
		public void TestJsonHasOneNode()
		{
			var file = new JsonFile(TestFilename);
			Assert.AreEqual(1, file.Root.NumberOfNodes);
		}

		[Test]
		public void LoadingJsonWithoutFileIsNotAllowed()
		{
			Assert.Throws<JsonFile.FileNotFound>(() => new JsonFile(null));
			Assert.Throws<JsonFile.FileNotFound>(() => new JsonFile(""));
		}

		[Test]
		public void LoadingNonexistingFileIsNotAllowed()
		{
			Assert.Throws<JsonFile.FileNotFound>(() => new JsonFile("NonExistingFile.json"));
		}
	}
}