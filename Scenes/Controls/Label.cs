using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// A background sprite and foreground text.
	/// </summary>
	public class Label : Picture
	{
		internal protected Label() {}

		public Label(Rectangle drawArea, string text = "")
			: this(new Theme(), drawArea, text) {}

		public Label(Theme theme, Rectangle drawArea, string text = "")
			: this(theme, theme.Label, drawArea)
		{
			Text = text;
			PreviousText = Text;
		}

		public string Text
		{
			get { return FontText.Text; }
			set
			{
				if (FontText.Text == value)
					return;
				PreviousText = FontText.Text;
				FontText.Text = value;
			}
		}

		protected FontText FontText
		{
			get
			{
				if (Contains<FontText>())
					return Get<FontText>();
				var fontText = new FontText(Theme.Font, "", GetFontTextDrawArea()); //ncrunch: no coverage start
				Add(fontText);
				return fontText; //ncrunch: no coverage end
			}
		}

		public string PreviousText { get; protected set; }

		internal Label(Theme theme, Material material, Rectangle drawArea)
			: base(theme, material, drawArea)
		{
			var fontText = new FontText(theme.Font, "", GetFontTextDrawArea());
			Add(fontText);
			AddChild(fontText);
		}

		private Rectangle GetFontTextDrawArea()
		{
			return Rectangle.FromCenter(Center, Size * ReductionDueToBorder);
		}

		protected const float ReductionDueToBorder = 0.9f;

		public override void Set(object component)
		{
			if (component == null)
				return;
			if (component is FontText)
				ReplaceChild((FontText)component);
			base.Set(component);
		}

		private void ReplaceChild(FontText text)
		{
			if (Contains<FontText>())
				RemoveChild(Get<FontText>());
			AddChild(text);
		}

		public override void Update()
		{
			base.Update();
			FontText.DrawArea = GetFontTextDrawArea();
		}
	}
}