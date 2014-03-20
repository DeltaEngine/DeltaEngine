using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Scenes.Controls;

namespace DeltaEngine.Scenes.Tests.Controls
{
	internal class Grow : UpdateBehavior
	{
		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (Control control in entities.OfType<Control>())
			{
				var center = control.DrawArea.Center + new Vector2D(0.01f, 0.01f) * Time.Delta;
				var size = control.DrawArea.Size * (1.0f + Time.Delta / 10);
				control.DrawArea = Rectangle.FromCenter(center, size);
			}
		}
	}
}