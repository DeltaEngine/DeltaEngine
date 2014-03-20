using System;
using NUnit.Framework;

namespace DeltaEngine.Content.Json.Tests
{
	public class JsonNodeTests
	{
		[Test]
		public void ReadJsonWithChildrenNodes()
		{
			var json = new JsonNode("{ \"Child1\": { \"Number\":1 }, \"Child2\": { \"Number\":2 } }");
			Assert.AreEqual(2, json.NumberOfNodes);
			Assert.AreEqual(1, json["Child1"].Get<int>("Number"));
			Assert.AreEqual(2, json["Child2"].Get<int>("Number"));
		}

		[Test]
		public void ParseBooleansStringsAndNumbers()
		{
			var json = new JsonNode("{ \"Flag\": true, \"SomeNumber\": 1.23, \"Text\": \"blub\" }");
			Assert.AreEqual(3, json.NumberOfNodes);
			Assert.IsTrue(json.Get<bool>("Flag"));
			Assert.AreEqual(1.23f, json.Get<float>("SomeNumber"));
			Assert.AreEqual("blub", json.GetOrDefault("Text", ""));
		}

		[Test]
		public void ParseEmptyJsonTextIsNotAllowed()
		{
			Assert.Throws<JsonNode.NeedValidText>(() => new JsonNode(""));
		}

		[Test]
		public void EmptyJsonShouldHaveZeroNodes()
		{
			var json = new JsonNode("{}");
			Assert.AreEqual(0, json.NumberOfNodes);
		}

		[Test]
		public void ExtractSomeDataValue()
		{
			var json = new JsonNode("{ \"SomeData\":6 }");
			Assert.AreEqual(1, json.NumberOfNodes);
			Assert.AreEqual(6, json.Get<int>("SomeData"));
			Assert.Throws<JsonNode.NodeNotFound>(() => json.Get<int>("blah"));
		}

		[Test]
		public void ReadArrayData()
		{
			var json = new JsonNode("{ \"arrayData\":[1, 2, 3] }");
			Assert.AreEqual(1, json.NumberOfNodes);
			Assert.AreEqual(new[] { 1, 2, 3 }, json["arrayData"].GetIntArray());
		}

		[Test]
		public void ReadJsonArray()
		{
			var json = new JsonNode("{ \"layers\":[ { \"sky\":[1, 1] }, { \"ground\":[0, 0] } ] }");
			Assert.AreEqual(1, json.NumberOfNodes);
			var layers = json["layers"];
			Assert.AreEqual(2, layers.NumberOfNodes);
			Assert.AreEqual(new[] { 1, 1 }, layers[0]["sky"].GetIntArray());
			Assert.AreEqual(new[] { 0, 0 }, layers[1]["ground"].GetIntArray());
		}

		[Test]
		public void UsingNonExistingIndexThrows()
		{
			var json = new JsonNode("{ \"layers\":[ { \"sky\":[1, 1] }, { \"ground\":[0, 0] } ] }");
			Assert.Throws<IndexOutOfRangeException>(() => Assert.NotNull(json["layers"][3]));
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void GetStringOfNode()
		{
			var json = new JsonNode("{ \"layers\":[ { \"sky\":[1, 1] }, { \"ground\":[0, 0] } ] }");
			var nodeString = json.ToString();
			Assert.IsFalse(string.IsNullOrEmpty(nodeString));
		}
	}
}