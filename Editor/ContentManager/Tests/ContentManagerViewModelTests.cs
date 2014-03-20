using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests
{
	public class ContentManagerViewModelTests : TestWithMocksOrVisually
	{
		[TestFixtureSetUp]
		public void SetExpectedContentNumbers()
		{
			numberOfContentItemsInList = Enum.GetNames(typeof(ContentType)).Length * 2;
			numberOfContentItemsInListWithLetterT = 0;
			foreach (var contentType in Enum.GetNames(typeof(ContentType)))
				if (contentType.ToLower().Contains("t"))
					numberOfContentItemsInListWithLetterT += 2;
		}

		private int numberOfContentItemsInList;
		private int numberOfContentItemsInListWithLetterT;

		[SetUp]
		public void CreateContentViewModel()
		{
			contentManagerViewModel = new ContentManagerViewModel(new MockService("User", "LogoApp"));
			AdvanceTimeAndUpdateEntities();
			contentManagerViewModel.RefreshContentList();
		}

		private ContentManagerViewModel contentManagerViewModel;

		[Test]
		public void DrawNoneExistingBackgrounds()
		{
			contentManagerViewModel.SelectedBackgroundImage = null;
			contentManagerViewModel.SelectedBackgroundImage = "Test1";
			contentManagerViewModel.SelectedBackgroundImage = "TestUser";
		}

		[Test, CloseAfterFirstFrame]
		public void CheckTheTypeOfNoneExistingContent()
		{
			contentManagerViewModel.SelectedContent = null;
			Assert.IsFalse(contentManagerViewModel.IsAnimation);
			contentManagerViewModel.SelectedContent = new ContentIconAndName("Image", "Test1");
			contentManagerViewModel.SelectedContent = new ContentIconAndName("Image", "TestUser");
			Assert.AreEqual("#00FFFFFF",
				((ContentIconAndName)contentManagerViewModel.SelectedContent).Brush.ToString());
			Assert.IsFalse(contentManagerViewModel.IsAnimation);
		}

		[Test, CloseAfterFirstFrame]
		public void DeleteNotExistingContent()
		{
			Assert.AreEqual(numberOfContentItemsInList, contentManagerViewModel.DisplayContentList.Count);
			contentManagerViewModel.AddContentToSelection("NotExisting");
			contentManagerViewModel.DeleteContentFromList("");
			Assert.AreEqual(numberOfContentItemsInList, contentManagerViewModel.DisplayContentList.Count);
			Assert.AreEqual(1, contentManagerViewModel.SelectedContentList.Count);
			contentManagerViewModel.ClearSelectionList("");
			Assert.AreEqual(0, contentManagerViewModel.SelectedContentList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void ControlClickDeselectsAlreadySelectedContent()
		{
			contentManagerViewModel.AddContentToSelection("MyImage1");
			Assert.AreEqual(1, contentManagerViewModel.SelectedContentList.Count);
			contentManagerViewModel.AddContentToSelection("MyImage1");
			Assert.AreEqual(0, contentManagerViewModel.SelectedContentList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void GetOnlyContentWithLetterT()
		{
			Assert.AreEqual(numberOfContentItemsInList, contentManagerViewModel.DisplayContentList.Count);
			contentManagerViewModel.SearchText = ("T");
			Assert.AreEqual(numberOfContentItemsInListWithLetterT,
				contentManagerViewModel.DisplayContentList.Count);
			Assert.AreEqual("#00FFFFFF", contentManagerViewModel.DisplayContentList[0].Brush.ToString());
		}

		[Test, CloseAfterFirstFrame]
		public void GetBackgroundAndSelectedContentShouldBeNullIfNotSet()
		{
			Assert.IsNull(contentManagerViewModel.SelectedContent);
			Assert.IsNull(contentManagerViewModel.SelectedBackgroundImage);
		}

		[Test]
		public void DeleteImageAnimationFromList()
		{
			contentManagerViewModel.DisplayContentList.Add(new ContentIconAndName("ImageAnimation",
				"ImageAnimation"));
			contentManagerViewModel.SelectedContent = new ContentIconAndName("ImageAnimation",
				"ImageAnimation");
			Assert.AreEqual(numberOfContentItemsInList + 1,
				contentManagerViewModel.DisplayContentList.Count);
			contentManagerViewModel.DeleteContentFromList("");
			Assert.AreEqual(numberOfContentItemsInList, contentManagerViewModel.DisplayContentList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingWhenNoStartContentIsAvailableWillDoNothing()
		{
			var numberOfEntities = EntitiesRunner.Current.GetAllEntities().Count;
			contentManagerViewModel.CheckMousePosition(new Vector2D(0.1f, 0.1f));
			Assert.AreEqual(numberOfEntities, EntitiesRunner.Current.GetAllEntities().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void OnlyImagesAndMaterialsAreUsedForStartContent()
		{
			contentManagerViewModel.Activate();
			ClearLists();
			contentManagerViewModel.AddNewContent("Image");
			contentManagerViewModel.AddNewContent("Material");
			contentManagerViewModel.AddNewContent("Font");
			contentManagerViewModel.AddNewContent("Scene");
			contentManagerViewModel.FilterContentList();
			contentManagerViewModel.ShowStartContent();
			Assert.AreEqual(2, contentManagerViewModel.StartPreviewList.Count);
			contentManagerViewModel.CheckMousePosition(new Vector2D(0.11f, 0.26f));
			Assert.AreEqual("Image", ((ContentIconAndName)contentManagerViewModel.SelectedContent).Name);
		}

		private void ClearLists()
		{
			contentManagerViewModel.ContentList.Clear();
			contentManagerViewModel.DisplayContentList.Clear();
			contentManagerViewModel.StartPreviewList.Clear();
		}

		[Test, CloseAfterFirstFrame]
		public void CannotShowPreviewsIfContentManagerIsNotOPen()
		{
			contentManagerViewModel.ShowingContentManager = false;
			contentManagerViewModel.ShowStartContent();
			Assert.AreEqual(0, contentManagerViewModel.StartPreviewList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void SelectImageAnimationFromStartContent()
		{
			contentManagerViewModel.ShowStartContent();
			Assert.AreEqual(12, contentManagerViewModel.StartPreviewList.Count);
			contentManagerViewModel.CheckMousePosition(new Vector2D(0.11f, 0.26f));
			Assert.AreEqual("MyLevel1",
				((ContentIconAndName)contentManagerViewModel.SelectedContent).Name);
		}

		[Test, CloseAfterFirstFrame]
		public void RightClickSelectsUnselectedContent()
		{
			AddBasicContentToList();
			Assert.AreEqual(0, contentManagerViewModel.SelectedContentList.Count);
			contentManagerViewModel.SelectToDelete("Material");
			contentManagerViewModel.SelectToDelete("Material");
			Assert.AreEqual(1, contentManagerViewModel.SelectedContentList.Count);
		}

		private void AddBasicContentToList()
		{
			ClearLists();
			contentManagerViewModel.AddNewContent("ImageAnimation");
			contentManagerViewModel.AddNewContent("Material");
			contentManagerViewModel.FilterContentList();
		}

		[Test, CloseAfterFirstFrame]
		public void ShiftClickAddsMultipleContentToSelectionFromTopToBottom()
		{
			AddBasicContentToList();
			contentManagerViewModel.AddMultipleContentToSelection("Material");
			contentManagerViewModel.AddMultipleContentToSelection("ImageAnimation");
			Assert.AreEqual(3, contentManagerViewModel.SelectedContentList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void ShiftClickAddsMultipleContentToSelectionFromBottomToTop()
		{
			AddBasicContentToList();
			contentManagerViewModel.AddMultipleContentToSelection("ImageAnimation");
			contentManagerViewModel.AddMultipleContentToSelection("Material");
			Assert.AreEqual(3, contentManagerViewModel.SelectedContentList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void GetContentTypeIcons()
		{
			foreach (ContentType contentType in EnumExtensions.GetEnumValues<ContentType>())
			{
				string contentIconFilePath = ContentIconAndName.GetContentTypeIcon(contentType);
				Assert.AreEqual(ContentTypeFolder + contentType + ".png", contentIconFilePath);
			}
		}

		private const string ContentTypeFolder = "../Images/ContentTypes/";

		[Test]
		public void StartContentShowsOnly12Previews()
		{
			contentManagerViewModel.AddNewContent("EarthImages");
			for (int i = 0; i < 13; i++)
				contentManagerViewModel.AddNewContent("Material" + i);
			contentManagerViewModel.ShowStartContent();
			Assert.AreEqual(12, contentManagerViewModel.StartPreviewList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void FilterByTypeOnlyShowsContentOfThisType()
		{
			var numberOfContentInList = contentManagerViewModel.DisplayContentList.Count;
			contentManagerViewModel.AddTypeToFilter("Image");
			Assert.Less(contentManagerViewModel.DisplayContentList.Count, numberOfContentInList);
		}

		[Test, CloseAfterFirstFrame]
		public void AddingAlreadyExistingContentShouldNotDuplicateEntries()
		{
			contentManagerViewModel.AddContentToContentList(ContentType.Image, "DeltaEngineLogo");
			int count = contentManagerViewModel.ContentList.Count;
			contentManagerViewModel.AddContentToContentList(ContentType.Image, "DeltaEngineLogo");
			Assert.AreEqual(count, contentManagerViewModel.ContentList.Count);
		}

		//ncrunch: no coverage start 
		[Test, Ignore]
		public void OpenFileExplorerToAddNewContent()
		{
			AddBasicContentToList();
			contentManagerViewModel.OpenFileExplorerToAddNewContent("");
		}
	}
}