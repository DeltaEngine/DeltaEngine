using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// Simple UI button which changes appearance based on mouse/touch interaction.
	/// </summary>
	[SaveSafely]
	public class Button : Label
	{
		internal protected Button() {} 

		public Button(Rectangle drawArea, string text = "")
			: this(new Theme(), drawArea, text) { }

		public Button(Theme theme, Rectangle drawArea, string text = "")
			: base(theme, theme.Button, drawArea)
		{
			Text = text;
		}

		public override void Update()
		{
			if (!IsEnabled)
				SetAppearance(Theme.ButtonDisabled);
			else if (State.IsInside && State.IsPressed)
				SetAppearance(Theme.ButtonPressed);
			else if (State.IsInside)
				SetAppearance(Theme.ButtonMouseover);
			else
				SetAppearance(Theme.Button);
			base.Update();
		}
	}
}