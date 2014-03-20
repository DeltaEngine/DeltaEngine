namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Product information, received by a market product information request.
	/// </summary>
	public class ProductData
	{
		public ProductData(string id, string title, string description, decimal price,
			bool isValid)
		{
			Id = id;
			Title = title;
			Description = description;
			Price = price;
			IsValid = isValid;
		}

		public string Id { get; private set; }
		public string Title { get; private set; }
		public string Description { get; private set; }
		public decimal Price { get; private set; }
		public bool IsValid { get; private set; }
	}
}
