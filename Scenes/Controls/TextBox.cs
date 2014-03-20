using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// A Label into which text can be typed.
	/// </summary>
	public class TextBox : Label, KeyboardControllable
	{
		protected TextBox() {}

		public TextBox(Rectangle drawArea, string text = "")
			: this(new Theme(), drawArea, text) { }

		public TextBox(Theme theme, Rectangle drawArea, string text = "")
			: base(theme, theme.TextBox, drawArea)
		{
			Text = text;
			PreviousText = text;
			State.CanHaveFocus = true;
			Start<Keyboard>();
		}

		public override void Update()
		{
			base.Update();
			if (!IsEnabled)
				SetAppearance(Theme.TextBoxDisabled);
			else if (HasFocus)
				SetAppearance(Theme.TextBoxFocused);
			else
				SetAppearance(Theme.TextBox);
		}

		public bool HasFocus
		{
			get { return State.HasFocus; }
		}

		public void UpdateTextFromKeyboardInput(Func<string, string> handleInput)
		{
			if (IsEnabled && HasFocus)
				Text = handleInput(Text);
		}
	}
}