using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	public class SampleTests
	{
		[Test]
		public void CreateGameSample()
		{
			var sample = new Sample("GameSample", SampleCategory.Game, "Game.sln", "Game.csproj",
				"Game.exe");
			Assert.AreEqual("Sample Game", sample.Description);
			Assert.AreEqual(SampleCategory.Game, sample.Category);
			Assert.AreEqual("http://DeltaEngine.net/Editor/Icons/GameSample.png", sample.ImageUrl);
		}

		[Test]
		public void CreateTestSample()
		{
			var sample = new Sample("TestSample", SampleCategory.Test, "Test.sln", "Tests.csproj",
				"Tests.dll") { EntryClass = "ClassName", EntryMethod = "MethodName" };
			Assert.AreEqual("Visual Test", sample.Description);
			Assert.AreEqual(SampleCategory.Test, sample.Category);
			Assert.AreEqual("http://DeltaEngine.net/Editor/Icons/VisualTest.png", sample.ImageUrl);
		}

		[Test]
		public void CreateDeltaEngineVisualTestSample()
		{
			var sample = new Sample("DeltaEngine.Platforms.Tests: HasAvailableRam", SampleCategory.Test,
				"Test.sln", "Tests.csproj", "Tests.dll")
			{
				EntryClass = "ClassName",
				EntryMethod = "MethodName"
			};
			Assert.AreEqual("Visual Test", sample.Description);
			Assert.AreEqual(SampleCategory.Test, sample.Category);
			Assert.AreEqual("http://DeltaEngine.net/Editor/Icons/Platforms.png", sample.ImageUrl);
		}

		[Test]
		public void TutorialTitlesAreSplitAndAllOtherCategoriesAreNotSplit()
		{
			var game = new Sample("DoNotSplit", SampleCategory.Game, "A.sln", "B.csproj", "C.exe");
			var tutorial = new Sample("SplitThis", SampleCategory.Tutorial, "A.sln", "B.csproj", "C.exe");
			Assert.AreEqual("DoNotSplit", game.Title);
			Assert.AreEqual("Split This", tutorial.Title);
		}

		[Test]
		public void ContainsFilterText()
		{
			var gameSample = new Sample("GameSample", SampleCategory.Game, "Game.sln", "Game.csproj",
				"Game.exe");
			Assert.True(gameSample.ContainsFilterText("Game"));
			Assert.False(gameSample.ContainsFilterText("Test"));
			var testSample = new Sample("TestSample", SampleCategory.Test, "Test.sln", "Tests.csproj",
				"Tests.dll") { EntryClass = "ClassName", EntryMethod = "MethodName" };
			Assert.True(testSample.ContainsFilterText("Test"));
			Assert.False(testSample.ContainsFilterText("Game"));
		}

		[Test]
		public void ContainsTutorialFilterText()
		{
			var gameSample = new Sample("TutorialSample", SampleCategory.Tutorial, "Tutorial.sln",
				"Tutorial.csproj", "Tutorial.exe");
			Assert.True(gameSample.ContainsFilterText("Tutorial"));
			Assert.True(gameSample.ContainsFilterText("TutorialSample"));
			Assert.True(gameSample.ContainsFilterText("Tutorial Sample"));
		}

		[Test]
		public void ToStringTest()
		{
			const string Title = "Sample's Name";
			const SampleCategory Category = SampleCategory.Game;
			const string Description = "Sample Game";
			var sample = new Sample(Title, Category, "Game.sln", "Game.csproj", "Game.exe");
			Assert.AreEqual(
				"Sample: Title=" + Title + ", Category=" + Category + ", Description=" + Description,
				sample.ToString());
		}

		[Test]
		public void ToStringTestTutorial()
		{
			const SampleCategory Category = SampleCategory.Tutorial;
			var sample = new Sample("TutorialName", Category, "Game.sln", "Game.csproj", "Game.exe");
			Assert.AreEqual(
				"Sample: Title=Tutorial Name, Category=" + Category + ", Description=Tutorial",
				sample.ToString());
		}

		[Test]
		public void ChangeDescription()
		{
			var sample = new Sample("Game", SampleCategory.Game, "Game.sln", "Game.csproj", "Game.exe");
			const string NewDescription = "Updated Description Text";
			Assert.AreNotEqual(NewDescription, sample.Description);
			sample.Description = NewDescription;
			Assert.AreEqual(NewDescription, sample.Description);
		}

		[Test]
		public void ChangeFeaturedState()
		{
			var sample = new Sample("Game", SampleCategory.Game, "Game.sln", "Game.csproj", "Game.exe");
			Assert.IsFalse(sample.IsFeatured);
			sample.IsFeatured = true;
			Assert.IsTrue(sample.IsFeatured);
		}
	}
}