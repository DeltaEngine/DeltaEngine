using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// A set of Radio Buttons form a Radio Dialog.
	/// </summary>
	public class RadioButton : Label
	{
		protected RadioButton() {}

		public RadioButton(Rectangle drawArea, string text = "")
			: this(new Theme(), drawArea, text) { }

		public RadioButton(Theme theme, Rectangle drawArea, string text = "")
			: base(theme, theme.RadioButtonBackground, drawArea)
		{
			Text = text;
			var selector = new Picture(theme, theme.RadioButtonNotSelected, GetSelectorDrawArea());
			Add(selector);
			AddChild(selector);
		}

		private Rectangle GetSelectorDrawArea()
		{
			float aspectRatio = Theme.RadioButtonNotSelected.DiffuseMap != null
				? Theme.RadioButtonNotSelected.DiffuseMap.PixelSize.AspectRatio
				: DefaultRadioButtonAspectRatio;
			var size = new Size(aspectRatio * DrawArea.Height, DrawArea.Height);
			return new Rectangle(DrawArea.TopLeft, size);
		}

		private const float DefaultRadioButtonAspectRatio = 1.0f;

		public override void Update()
		{
			base.Update();
			SetAppearance(IsEnabled ? Theme.RadioButtonBackground : Theme.RadioButtonBackgroundDisabled);
			UpdateSelectorAppearance();
			Selector.DrawArea = GetSelectorDrawArea();
		}

		private Picture Selector
		{
			get { return Get<Picture>(); }
		}

		private void UpdateSelectorAppearance()
		{
			if (!IsEnabled)
				Selector.SetAppearance(Theme.RadioButtonDisabled);
			if (State.IsInside && State.IsSelected)
				Selector.SetAppearance(Theme.RadioButtonSelectedMouseover);
			else if (State.IsInside)
				Selector.SetAppearance(Theme.RadioButtonNotSelectedMouseover);
			else if (State.IsSelected)
				Selector.SetAppearance(Theme.RadioButtonSelected);
			else
				Selector.SetAppearance(Theme.RadioButtonNotSelected);
		}

		public override void Click()
		{
			State.IsSelected = true;
			base.Click();
		}

		public override sealed void Set(object component)
		{
			if (component is Picture)
				ReplaceChild((Picture)component);
			base.Set(component);
		}

		private void ReplaceChild(Picture selector)
		{
			if (Contains<Picture>())
				RemoveChild(Get<Picture>()); //ncrunch: no coverage
			AddChild(selector);
		}
	}
}