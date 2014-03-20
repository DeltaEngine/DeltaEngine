using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// A button which changes size depending on its state 
	/// - eg. grows on mouseover and shrinks on being clicked.
	/// </summary>
	public class InteractiveButton : Button
	{
		protected internal InteractiveButton() { }

		public InteractiveButton(Rectangle drawArea, string text = "")
			: base(new Theme(), drawArea, text) { }

		public InteractiveButton(Theme theme, Rectangle drawArea, string text = "")
			: base(theme, drawArea, text) { }

		public override void Update()
		{
			base.Update();
			if (!IsEnabled)
				Normalize();
			else if (State.IsInside && !State.IsPressed)
				Grow();
			else if (State.IsPressed)
				Shrink();
			else
				Normalize();
		}

		private void Normalize()
		{
			if (AnchoringSize == Size.Unused)
				return;
			DrawArea = Rectangle.FromCenter(DrawArea.Center, AnchoringSize);
			AnchoringSize = Size.Unused;
		}

		private void Grow()
		{
			if (AnchoringSize == Size.Unused)
				AnchoringSize = DrawArea.Size;
			DrawArea = Rectangle.FromCenter(DrawArea.Center, AnchoringSize * Growth);
		}

		private const float Growth = 1.05f;

		private void Shrink()
		{
			if (AnchoringSize == Size.Unused)
				AnchoringSize = DrawArea.Size;
			DrawArea = Rectangle.FromCenter(DrawArea.Center, AnchoringSize / Growth);
		}
	}
}