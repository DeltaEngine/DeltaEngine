using System.Collections.Generic;
using System.Reflection;
using DeltaEngine.Editor.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	/// <summary>
	/// Tests for the ViewModel of the SampleBrowser.
	/// </summary>
	[Ignore]
	public class SampleBrowserViewModelTests
	{
		[SetUp]
		public void Init()
		{
			viewModel = new SampleBrowserViewModel(new MockService("UserName", "ProjectName"));
			ChangeComboBoxSelectionTo(0);
		}

		private SampleBrowserViewModel viewModel;

		[Test]
		public void AddSamples()
		{
			Assert.AreEqual(0, viewModel.Samples.Count);
			//viewModel.Samples = GetSamplesMock();
			Assert.AreEqual(2, viewModel.Samples.Count);
		}

		private static List<Sample> GetSamplesMock()
		{
			return new List<Sample>
			{
				new Sample("TestName", SampleCategory.Test, "TestName.sln", "TestName.csproj",
					"TestName.dll") { EntryClass = "ClassName", EntryMethod = "MethodName" },
				new Sample("GameName", SampleCategory.Game, "GameName.sln", "GameName.csproj",
					"GameName.exe")
			};
		}

		[Test]
		public void CheckComboBoxSelections()
		{
			Assert.AreEqual(3, viewModel.AssembliesAvailable.Count);
			Assert.AreEqual("All", viewModel.SelectedAssembly);
		}

		[Test]
		public void ChangeComboBoxSelections()
		{
			viewModel.SetSampleGames(new List<Sample> { GetSamplesMock()[1] });
			viewModel.SetVisualTests(new List<Sample> { GetSamplesMock()[0] });
			viewModel.AddEverythingTogether();
			ChangeComboBoxSelectionTo(0);
			Assert.AreEqual(2, viewModel.GetItemsToDisplay().Count);
			ChangeComboBoxSelectionTo(1);
			Assert.AreEqual(1, viewModel.GetItemsToDisplay().Count);
			Assert.AreEqual("GameName", viewModel.GetItemsToDisplay()[0].Name);
			ChangeComboBoxSelectionTo(2);
			Assert.AreEqual(1, viewModel.GetItemsToDisplay().Count);
			Assert.AreEqual("TestName", viewModel.GetItemsToDisplay()[0].Name);
		}

		private void ChangeComboBoxSelectionTo(int index)
		{
			viewModel.SetSelection(index);
			viewModel.OnAssemblySelectionChanged.Execute(null);
		}

		[Test]
		public void SortSamplesByProjectFilePath()
		{
			viewModel.SetAllSamples(GetSamplesMock());
			viewModel.AddEverythingTogether();
			Assert.AreEqual("GameName", viewModel.Samples[0].Name);
			Assert.AreEqual("TestName", viewModel.Samples[1].Name);
		}

		[Test]
		public void FilterSamplesWithSearchBox()
		{
			viewModel.SetAllSamples(GetSamplesMock());
			viewModel.AddEverythingTogether();
			ChangeSearchText("Game");
			Assert.AreEqual(1, viewModel.GetItemsToDisplay().Count);
			Assert.AreEqual("GameName", viewModel.GetItemsToDisplay()[0].Name);
			ChangeSearchText("Test");
			Assert.AreEqual(1, viewModel.GetItemsToDisplay().Count);
			Assert.AreEqual("TestName", viewModel.GetItemsToDisplay()[0].Name);
			ClearSearchText();
			Assert.AreEqual(2, viewModel.GetItemsToDisplay().Count);
		}

		private void ChangeSearchText(string text)
		{
			viewModel.SetSearchText(text);
			viewModel.OnSearchTextChanged.Execute(null);
		}

		private void ClearSearchText()
		{
			viewModel.OnSearchTextRemoved.Execute(null);
			viewModel.OnSearchTextChanged.Execute(null);
		}

		[Test, Ignore]
		public void ClickOnHelpShouldOpenWebsiteInWebbrowser()
		{
			viewModel.OnHelpClicked.Execute(null);
		}

		[Test]
		public void UninitializedViewClickShouldThrow()
		{
			Assert.Throws<TargetInvocationException>(() => viewModel.OnViewButtonClicked.Execute(null));
		}

		[Test]
		public void UninitializedStartClickShouldThrow()
		{
			Assert.Throws<TargetInvocationException>(() => viewModel.OnStartButtonClicked.Execute(null));
		}
	}
}