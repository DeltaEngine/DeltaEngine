using DeltaEngine.Content;
using DeltaEngine.Content.Xml;

namespace CreepyTowers.Stats
{
	/// <summary>
	/// This holds which stat is to be adjusted, how much to adjust by, and which stat is used to
	/// resist the adjustment. 
	/// </summary>
	public struct StatAdjustment
	{
		public StatAdjustment(string attribute, string resist, float adjustment)
			: this()
		{
			Attribute = attribute;
			Resist = resist;
			Adjustment = adjustment;
		}

		public string Attribute { get; private set; }
		public string Resist { get; private set; }
		public float Adjustment { get; private set; }

		public StatAdjustment(string name)
			: this()
		{
			var adjusts = ContentLoader.Load<XmlContent>("AdjustmentProperties");
			var adjust = adjusts.Data.GetChild(name);
			if (adjust == null)
				return;
			Attribute = adjust.GetAttributeValue("Attribute");
			Resist = adjust.GetAttributeValue("Resist");
			Adjustment = adjust.GetAttributeValue("Adjustment", 0.0f);
		}
	}
}