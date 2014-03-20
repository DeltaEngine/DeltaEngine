using System.IO;

namespace DeltaEngine.Content.Json
{
	/// <summary>
	/// Content data for Newtonsoft Json.
	/// </summary>
	public class JsonContent : ContentData
	{
		//ncrunch: no coverage start
		protected JsonContent(string contentName)
			: base(contentName) {}

		protected override void LoadData(Stream fileData)
		{
			using (var stream = new StreamReader(fileData))
			{
				var text = stream.ReadToEnd();
				Data = new JsonNode(text);
			}
		}

		public JsonNode Data { get; private set; }

		protected override void DisposeData() {}
	}
}