using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	class InAppPurchaseTests : TestWithMocksOrVisually
	{
		[Test]
		public void RequestProductInformation()
		{
			var inAppPurchase = GetMockInAppPurchase();
			inAppPurchase.OnReceivedProductInformation += ReceivedProductInformation;
			Assert.IsTrue(inAppPurchase.RequestProductInformationAsync(new[] { 0 }));
		}

		private static MockInAppPurchase GetMockInAppPurchase()
		{
			return new MockInAppPurchase();
		}

		private static void ReceivedProductInformation(ProductData[] products)
		{
			Assert.AreEqual("testId", products[0].Id);
			Assert.AreEqual("testTitle", products[0].Title);
			Assert.AreEqual("testDescription", products[0].Description);
			Assert.AreEqual(1.99m, products[0].Price);
			Assert.IsTrue(products[0].IsValid);
		}

		[Test]
		public void PurchaseProductSuccessfully()
		{
			var inAppPurchase = GetMockInAppPurchase();
			inAppPurchase.OnTransactionFinished += PurchaseProduct;
			Assert.IsTrue(inAppPurchase.PurchaseProductAsync(inAppPurchase.TestProductData));
		}

		private static void PurchaseProduct(ProductData productId, bool wasSuccessful)
		{
			Assert.AreEqual("testId", productId.Id);
			Assert.IsTrue(wasSuccessful);
		}

		[Test]
		public void PurchaseProductCanceled()
		{
			var inAppPurchase = GetMockInAppPurchase();
			inAppPurchase.OnUserCanceled += PurchaseCanceled;
			inAppPurchase.InvokeOnUserCanceled("testId");
		}

		private static void PurchaseCanceled(string productId)
		{
			Assert.AreEqual("testId", productId);
		}
	}
}