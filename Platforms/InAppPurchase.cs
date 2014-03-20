
namespace DeltaEngine.Platforms
{
	/// <summary>
	/// This class provides abstract access to the native IAP (In App Purchases).
	/// </summary>
	public abstract class InAppPurchase
	{
		public void InvokeOnReceivedProductInformation(ProductData[] products)
		{
			if (OnReceivedProductInformation != null)
				OnReceivedProductInformation(products);
		}

		public event ProductInformationDelegate OnReceivedProductInformation;
		public delegate void ProductInformationDelegate(ProductData[] products);

		public void InvokeOnTransactionFinished(ProductData productId, bool wasSuccessfull)
		{
			if (OnTransactionFinished != null)
				OnTransactionFinished(productId, wasSuccessfull);
		}

		public event TransactionDelegate OnTransactionFinished;
		public delegate void TransactionDelegate(ProductData productId, bool wasSuccessful);

		public void InvokeOnUserCanceled(string productId)
		{
			if (OnUserCanceled != null)
				OnUserCanceled(productId);
		}

		public event UserCanceledDelegate OnUserCanceled;
		public delegate void UserCanceledDelegate(string productId);

		public abstract bool RequestProductInformationAsync(int[] productIds);
		public abstract bool PurchaseProductAsync(ProductData product);
	}
}
