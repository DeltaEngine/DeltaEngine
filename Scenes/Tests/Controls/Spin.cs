using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Entities;
using DeltaEngine.Scenes.Controls;

namespace DeltaEngine.Scenes.Tests.Controls
{
	internal class Spin : UpdateBehavior
	{
		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (Control control in entities.OfType<Control>())
				control.Rotation += 20 * Time.Delta;
		}
	}
}