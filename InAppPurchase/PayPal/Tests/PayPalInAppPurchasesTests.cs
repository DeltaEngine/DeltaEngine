using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.InAppPurchase.PayPal.Tests
{
	public class PayPalInAppPurchasesTests
	{
		[Test]
		public void CreateInstance()
		{
			Assert.DoesNotThrow(() => GetPayPalInAppPurchases());
		}

		private static PayPalInAppPurchases GetPayPalInAppPurchases()
		{
			return new PayPalInAppPurchases("payments@example.com", "DE", "EUR");
		}

		[Test]
		public void GeneratePaymentLink()
		{
			const string ExpectedLink = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=" +
				"payments@example.com&lc=DE&item_name=testId&currency_code=EUR&amount=1.99&bn=PP%2dDonationsBF";
			var inAppPurchase = GetPayPalInAppPurchases();
			Assert.AreEqual(inAppPurchase.GeneratePaymentLink(GetTestProductData()), ExpectedLink);
		}

		private static ProductData GetTestProductData()
		{
			return new ProductData("testId", "testTitle", "testDescription", 1.99m, true);
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void PurchaseProductOpenWebBrowser()
		{
			var inAppPurchase = GetPayPalInAppPurchases();
			inAppPurchase.PurchaseProductAsync(GetTestProductData());
		}
	}
}