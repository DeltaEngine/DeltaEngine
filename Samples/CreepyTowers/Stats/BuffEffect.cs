using DeltaEngine.Content;
using DeltaEngine.Content.Xml;

namespace CreepyTowers.Stats
{
	/// <summary>
	/// Holds which attribute this buff affects, how much it affects it, and how long the buff lasts 
	/// </summary>
	public struct BuffEffect
	{
		public BuffEffect(string name)
			: this()
		{
			var buffEffects = ContentLoader.Load<XmlContent>("BuffProperties");
			var buffEffect = buffEffects.Data.GetChild(name);
			if (buffEffect == null)
				return;
			Attribute = buffEffect.GetAttributeValue("Attribute");
			Multiplier = buffEffect.GetAttributeValue("Multiplier", 1.0f);
			Addition = buffEffect.GetAttributeValue("Addition", 0.0f);
			Duration = buffEffect.GetAttributeValue("Duration", 0.0f);
		}

		public string Attribute { get; private set; }
		public float Multiplier { get; private set; }
		public float Addition { get; private set; }
		public float Duration { get; private set; }
	}
}