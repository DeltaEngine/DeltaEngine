using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeltaEngine.Content.Json
{
	/// <summary>
	/// Provides json text parsing support without having to include Newtonsoft Json yourself and
	/// making it work on different platforms easily replacing functionality (like Content.Xml).
	/// </summary>
	public class JsonNode
	{
		public JsonNode(string text)
		{
			if (String.IsNullOrEmpty(text))
				throw new NeedValidText();
			data = JObject.Parse(text);
		}

		private JsonNode(JToken parsedJsonData)
		{
			data = parsedJsonData;
		}

		public class NeedValidText : Exception {}

		private readonly JToken data;

		public int NumberOfNodes
		{
			get { return ((JContainer)data).Count; }
		}

		public T Get<T>(string nodeName)
		{
			var node = data[nodeName];
			if (node == null)
				throw new NodeNotFound(nodeName);
			return node.Value<T>();
		}

		public T GetOrDefault<T>(string nodeName, T defaultValue)
		{
			var node = data[nodeName];
			return node != null ? node.Value<T>() : defaultValue;
		}

		public class NodeNotFound : Exception
		{
			public NodeNotFound(string nodeName)
				: base("Node not found: " + nodeName) {}
		}

		public JsonNode this[string childName]
		{
			get { return new JsonNode(data[childName]); }
		}

		public JsonNode this[int arrayIndex]
		{
			get
			{
				if (data is JArray)
					return FindJsonArrayElement(arrayIndex);
				return new JsonNode(data[arrayIndex]); //ncrunch: no coverage
			}
		}

		private JsonNode FindJsonArrayElement(int arrayIndex)
		{
			int counter = 0;
			foreach (var element in data)
				if (counter++ == arrayIndex)
					return new JsonNode(element);
			throw new IndexOutOfRangeException();
		}

		public int[] GetIntArray()
		{
			var array = data as JArray;
			return array.Values<int>().ToArray();
		}

		//ncrunch: no coverage start
		public override string ToString()
		{
			return JsonConvert.SerializeObject(data);
		}
	}
}