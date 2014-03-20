using System.Globalization;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;

namespace FindTheWord
{
	public class CharacterButton : GameButton
	{
		public CharacterButton(float xCenter, float yCenter, GameScreen screen)
			: base("CharBackground", GetDrawArea(xCenter, yCenter))
		{
			this.screen = screen;
			letter = NoCharacter;
			var font = ContentLoader.Load<Font>("Tahoma30");
			currentFontText = new FontText(font, "", Rectangle.FromCenter(Vector2D.Half, new Size(0.5f)));
			currentFontText.RenderLayer = 4;
		}

		private readonly GameScreen screen;

		private static Rectangle GetDrawArea(float xCenter, float yCenter)
		{
			return Rectangle.FromCenter(xCenter, yCenter, Dimension, Dimension);
		}

		internal const float Dimension = 65.0f / 1280.0f;
		private char letter;
		internal const char NoCharacter = '\0';
		private readonly FontText currentFontText;

		public char Letter
		{
			get { return letter; }
			set
			{
				letter = value;
				currentFontText.Text = letter.ToString(CultureInfo.InvariantCulture);
				SetNewCenter(DrawArea.Center);
			}
		}

		private void SetNewCenter(Vector2D newCenter)
		{
			Center = newCenter;
			var newLetterArea = DrawArea;
			newLetterArea.Left += Dimension * 0.05f;
			currentFontText.DrawArea = newLetterArea;
		}

		public void RemoveLetter()
		{
			currentFontText.Text = "";
		}

		public void ShowLetter()
		{
			currentFontText.Text = letter.ToString(CultureInfo.InvariantCulture);
		}

		public int Index { get; set; }

		public override string ToString()
		{
			return GetType().Name + "(" + Index + " - " + Letter + ")";
		}

		public bool IsClickable()
		{
			return IsVisible && currentFontText.Text != "";
		}

		public override void Dispose()
		{
			currentFontText.Text = "";
			currentFontText.IsActive = false;
			IsActive = false;
			base.Dispose();
		}

		public void OnDisplayCharButtonClicked()
		{
			if (!IsClickable())
				return;
			screen.OnDisplayCharButtonClicked(this);
		}

		public void OnSolutionCharButtonClicked()
		{
			screen.OnSolutionCharButtonClicked(this);
		}
	}
}