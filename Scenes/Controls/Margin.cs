using System;
using DeltaEngine.Entities;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// Specifies the fixed distance one control should remain from another (or from the screen edge)
	/// -1 means this edge is not anchored
	/// </summary>
	public struct Margin
	{
		public Margin(Edge othersEdge)
			: this("", othersEdge, -1) {}

		public Margin(Edge othersEdge, float distance)
			: this("", othersEdge, distance) {}

		public Margin(Control other, Edge othersEdge, float distance)
			: this()
		{
			if (other != null)
				OtherControlName = other.Name;
			OthersEdge = othersEdge;
			Distance = distance;
		}

		private string OtherControlName { get; set; }
		public Edge OthersEdge { get; private set; }
		public float Distance { get; private set; }

		internal Margin(string otherControlName, Edge othersEdge, float distance)
			: this()
		{
			OtherControlName = otherControlName;
			OthersEdge = othersEdge;
			Distance = distance;
		}

		public Control Other
		{
			get
			{
				if (other != null || OtherControlName == "")
					return other;
				var controls = EntitiesRunner.Current.GetEntitiesOfType<Control>();
				foreach (var control in controls)
					if (control.Name == OtherControlName)
						return other = control;
				return null; //ncrunch: no coverage
			}
		}

		[NonSerialized]
		private Control other;
	}
}