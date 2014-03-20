using System.Threading;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Editor.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	public class PopupMessageViewModelTests
	{
		[SetUp]
		public void InitializeMessageViewModelAndService()
		{
			service = new MockService("TestUser", "TestProject");
			messageViewModel = new PopupMessageViewModel(service, MessageDisplayTime);
		}

		private MockService service;
		private const int MessageDisplayTime = 1000;
		private PopupMessageViewModel messageViewModel;

		[Test]
		public void InitialPopupMessageShouldBeInvisibleAndHasNoText()
		{
			Assert.IsEmpty(messageViewModel.Text);
			Assert.AreEqual(Visibility.Hidden, messageViewModel.Visiblity);
		}

		[Test]
		public void RaiseUpdateContentEventShouldShowUpdateText()
		{
			service.RecieveData(ContentType.Image);
			AssertPopUpText(ContentType.Image);
		}

		private void AssertPopUpText(ContentType contentType)
		{
			Assert.AreEqual("MockContent " + contentType + " Updated!", messageViewModel.Text);
			Assert.AreEqual(Visibility.Visible, messageViewModel.Visiblity);
		}

		[Test]
		public void MultipleUpdateContentMessagesAreShownImmediately()
		{
			service.RecieveData(ContentType.Material);
			AssertPopUpText(ContentType.Material);
			service.RecieveData(ContentType.Json);
			AssertPopUpText(ContentType.Json);
			service.RecieveData(ContentType.Xml);
			AssertPopUpText(ContentType.Xml);
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void PopupMessageShouldBeHiddenAfterThreeSeconds()
		{
			service.RecieveData(ContentType.Image);
			Thread.Sleep(MessageDisplayTime + 100);
			Assert.AreEqual(Visibility.Hidden, messageViewModel.Visiblity);
		}
	}
}