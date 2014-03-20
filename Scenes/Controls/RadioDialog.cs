using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// A simple UI control which creates and groups Radio Buttons.
	/// </summary>
	public class RadioDialog : Control
	{
		protected RadioDialog() {}

		public RadioDialog(Rectangle drawArea)
			: this(new Theme(), drawArea) { }

		public RadioDialog(Theme theme, Rectangle drawArea)
			: base(drawArea)
		{
			Add(theme);
			Add(new List<RadioButton>());
		}

		private Theme Theme
		{
			get { return Get<Theme>(); }
		}

		internal List<RadioButton> Buttons
		{
			get { return Get<List<RadioButton>>(); }
		}

		public void AddButton(string text)
		{
			var button = new RadioButton(Theme, Rectangle.Unused, text);
			button.Clicked += () => ButtonClicked(button);
			Buttons.Add(button);
			AddChild(button);
			if (Buttons.Count == 1)
				button.State.IsSelected = true;
			for (int i = 0; i < Buttons.Count; i++)
				Buttons[i].DrawArea = GetButtonDrawArea(i);
		}

		private Rectangle GetButtonDrawArea(int position)
		{
			float height = DrawArea.Height / (Buttons.Count);
			float aspectRatio = Theme.RadioButtonBackground.DiffuseMap != null
				? Theme.RadioButtonBackground.DiffuseMap.PixelSize.AspectRatio
				: DefaultRadioButtonAspectRatio;
			float width = height * aspectRatio;
			var rectangle = new Rectangle(DrawArea.Left, DrawArea.Top + position * height, width, height);
			return rectangle;
		}

		private const float DefaultRadioButtonAspectRatio = 4.0f;

		private void ButtonClicked(RadioButton clicked)
		{
			foreach (RadioButton button in Buttons)
				button.State.IsSelected = (button == clicked);
		}

		public override void Update()
		{
			base.Update();
			for (int i = 0; i < Buttons.Count; i++)
				Buttons[i].DrawArea = GetButtonDrawArea(i);
		}

		public RadioButton SelectedButton
		{
			get { return Get<List<RadioButton>>().FirstOrDefault(button => button.State.IsSelected); }
		}

		public override void Set(object component)
		{
			var buttons = component as List<RadioButton>;
			if (buttons != null)
				foreach (var button in buttons)
					ProcessButton(button);
			base.Set(component);
		}

		private void ProcessButton(RadioButton button)
		{
			button.Clicked += () => ButtonClicked(button);
			AddChild(button);
		}
	}
}