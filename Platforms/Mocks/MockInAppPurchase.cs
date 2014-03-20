namespace DeltaEngine.Platforms.Mocks
{
	/// <summary>
	/// Mock in-app purchase used for unit testing.
	/// </summary>
	public class MockInAppPurchase : InAppPurchase
	{
		public override bool RequestProductInformationAsync(int[] productIds)
		{
			InvokeOnReceivedProductInformation(new[] { TestProductData });
			return true;
		}

		public readonly ProductData TestProductData = new ProductData("testId", "testTitle",
			"testDescription", 1.99m, true);

		public override bool PurchaseProductAsync(ProductData product)
		{
			InvokeOnTransactionFinished(product, true);
			return true;
		}
	}
}