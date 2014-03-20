using NUnit.Framework;

namespace DeltaEngine.Editor.Mocks.Tests
{
	public class MockServiceTests
	{
		[SetUp]
		public void Init()
		{
			mockService = new MockService("TestUser", "TestProject");
		}

		private MockService mockService;

		[Test]
		public void AvailableProjectsShouldBeZeroOnInitialization()
		{
			Assert.AreEqual(0, mockService.AvailableProjects.Length);
		}

		[Test]
		public void AfterChangingProjectsAvailableProjectsPropertyShouldIncrease()
		{
			mockService.ChangeProject("AnotherTestProject");
			Assert.AreEqual(1, mockService.AvailableProjects.Length);
		}

		[Test]
		public void SetAvailableProjectsShouldBeTwo()
		{
			mockService.SetAvailableProjects(new[] { "AnotherProject", "YetAnotherTestProject" });
			Assert.AreEqual(2, mockService.AvailableProjects.Length);
		}

		[Test]
		public void GetAndSetCurrentContentProjectSolutionFilePath()
		{
			const string CustomCodeSolutionPath = "TestProject.sln";
			mockService.CurrentContentProjectSolutionFilePath = CustomCodeSolutionPath;
			Assert.AreEqual(CustomCodeSolutionPath, mockService.CurrentContentProjectSolutionFilePath);
			mockService.SetContentProjectSolutionFilePath("TestProject", "");
			Assert.IsTrue(mockService.CurrentContentProjectSolutionFilePath.Contains("Samples"));
			Assert.AreNotEqual(CustomCodeSolutionPath, mockService.CurrentContentProjectSolutionFilePath);
			mockService.ChangeProject("TestTutorials");
			Assert.IsTrue(mockService.CurrentContentProjectSolutionFilePath.Contains("Tutorials"));
		}

		[Test]
		public void SetActions()
		{
			bool isDataReceived = false;
			bool isContentUpdated = false;
			bool isContentDeleted = false;
			bool isContentReady = false;
			bool isProjectChanged = false;
			mockService.DataReceived += delegate { isDataReceived = true; };
			mockService.ContentUpdated += delegate { isContentUpdated = true; };
			mockService.ContentDeleted += delegate { isContentDeleted = true; };
			mockService.ProjectChanged += delegate { isProjectChanged = true; };
			mockService.ContentReady += delegate { isContentReady = true; };
			mockService.ReceiveData(new object());
			mockService.ChangeProject("TestProject");
			mockService.SetContentReady();
			Assert.IsTrue(isDataReceived);
			Assert.IsTrue(isContentUpdated);
			Assert.IsTrue(isContentDeleted);
			Assert.IsTrue(isProjectChanged);
			Assert.IsTrue(isContentReady);
		}
	}
}