using System;
using System.Diagnostics;
using System.Globalization;
using DeltaEngine.Platforms;

namespace DeltaEngine.InAppPurchase.PayPal
{
	public class PayPalInAppPurchases : Platforms.InAppPurchase
	{
		public PayPalInAppPurchases(string merchantEmailAddress, string countryCode,
			string currencyCode)
		{
			this.merchantEmailAddress = merchantEmailAddress;
			this.countryCode = countryCode;
			this.currencyCode = currencyCode;
		}

		private readonly string merchantEmailAddress;
		private readonly string countryCode;
		private readonly string currencyCode;

		internal string GeneratePaymentLink(ProductData product)
		{
			return "https://www.paypal.com/cgi-bin/webscr?cmd=_donations" +
				"&business=" + merchantEmailAddress +
				"&lc=" + countryCode +
				"&item_name=" + product.Id +
				"&currency_code=" + currencyCode +
				"&amount=" + product.Price.ToString(CultureInfo.InvariantCulture) +
				"&bn=PP%2dDonationsBF";
		}

		//ncrunch: no coverage start
		public override bool PurchaseProductAsync(ProductData product)
		{
			Process.Start(GeneratePaymentLink(product));
			// Need verification of payment callback, this can only be done server-side and need to
			// pushed/polled back to the client.
			InvokeOnTransactionFinished(product, true);
			return true;
		}

		public override bool RequestProductInformationAsync(int[] productIds)
		{
			throw new NotSupportedException("Need webservice for this");
		}
	}
}