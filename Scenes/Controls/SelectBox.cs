using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// Allows one value to be selected from a dropdown list of values
	/// </summary>
	public class SelectBox : Picture
	{
		protected SelectBox()
		{
			Clicked += ClickLine;
		}

		public SelectBox(Rectangle firstLineDrawArea, List<object> values)
			: this(new Theme(), firstLineDrawArea, values) { }

		public SelectBox(Theme theme, Rectangle firstLineDrawArea, List<object> values)
			: base(theme, theme.SelectBox, firstLineDrawArea)
		{
			var scrollbar = new Scrollbar(theme, Rectangle.Unused) { Rotation = 90 };
			Add(scrollbar);
			AddChild(scrollbar);
			Clicked += ClickLine;
			if (values == null || values.Count == 0)
				throw new MustBeAtLeastOneValue();
			Add(new Data(values, firstLineDrawArea, values.Count));
			DrawArea = new Rectangle(firstLineDrawArea.Left, firstLineDrawArea.Top,
				firstLineDrawArea.Width, firstLineDrawArea.Height * values.Count);
			Values = values;
		}

		protected Scrollbar Scrollbar
		{
			get { return Get<Scrollbar>(); }
		}

		private class Data
		{
			protected Data() {} //ncrunch: no coverage

			public Data(List<object> values, Rectangle firstLineDrawArea, int maxDisplayCount)
			{
				Values = values;
				FirstLineDrawArea = firstLineDrawArea;
				MaxDisplayCount = maxDisplayCount;
			}

			public List<object> Values { get; set; }
			public Rectangle FirstLineDrawArea { get; set; }
			public int MaxDisplayCount { get; set; }
		}

		public class MustBeAtLeastOneValue : Exception {}

		public List<object> Values
		{
			get { return Get<Data>().Values; }
			set
			{
				if (value == null || value.Count == 0)
					throw new MustBeAtLeastOneValue();
				Get<Data>().Values = value;
				UpdateGraphics();
			}
		}

		private void UpdateGraphics()
		{
			var firstLineDrawArea = Get<Data>().FirstLineDrawArea;
			DrawArea = new Rectangle(firstLineDrawArea.Left, firstLineDrawArea.Top,
				firstLineDrawArea.Width, firstLineDrawArea.Height * DisplayCount);
			UpdateScrollbar();
			ClearOldSelectionBoxTexts();
			CreateNewSelectionBoxTexts();
		}

		public int DisplayCount
		{
			get { return Values == null ? 0 : (int)MathExtensions.Min(MaxDisplayCount, Values.Count); }
		}

		public int MaxDisplayCount
		{
			get { return Get<Data>().MaxDisplayCount; }
			set
			{
				Get<Data>().MaxDisplayCount = value;
				UpdateGraphics();
			}
		}

		private void UpdateScrollbar()
		{
			var values = Values;
			if (values == null)
				return; //ncrunch: no coverage
			float width = DrawArea.Width * ScrollbarPercentageWidth;
			Scrollbar.DrawArea = new Rectangle(DrawArea.Right - width, DrawArea.Top, DrawArea.Height,
				width);
			Scrollbar.RotationCenter = new Vector2D(Scrollbar.DrawArea.Left + width / 2,
				Scrollbar.DrawArea.Top + width / 2);
			Scrollbar.RenderLayer = RenderLayer + 1;
			Scrollbar.MaxValue = values.Count - 1;
			Scrollbar.ValueWidth = DisplayCount;
			Scrollbar.IsVisible = IsVisible && values.Count > MaxDisplayCount;
		}

		private const float ScrollbarPercentageWidth = 0.1f;

		private void ClearOldSelectionBoxTexts()
		{
			foreach (FontText text in texts)
			{
				RemoveChild(text);
				text.IsActive = false;
			}
			texts.Clear();
		}

		internal readonly List<FontText> texts = new List<FontText>();

		private void CreateNewSelectionBoxTexts()
		{
			int count = DisplayCount;
			for (int i = 0; i < count; i++)
			{
				var font = new FontText(Theme.Font, "", Rectangle.Unused)
				{
					HorizontalAlignment = HorizontalAlignment.Left,
					IsVisible = IsVisible
				};
				AddChild(font);
				texts.Add(font);
			}
		}

		private void ClickLine()
		{
			int lineNumber = GetMouseoverLineNumber();
			if (lineNumber >= 0 && LineClicked != null)
				LineClicked(Scrollbar.LeftValue + lineNumber);
		}

		public Action<int> LineClicked;

		private int GetMouseoverLineNumber()
		{
			float x = Get<InteractiveState>().RelativePointerPosition.X;
			if (Scrollbar.IsVisible && x >= 1.0f - ScrollbarPercentageWidth)
				return -1; //ncrunch: no coverage
			float y = Get<InteractiveState>().RelativePointerPosition.Y;
			return y < 0.0f || y > 1.0f ? -1 : (int)(y * DisplayCount);
		}

		//ncrunch: no coverage start
		public override void Update()
		{
			base.Update();
			if (!Contains<Data>())
				return;
			Get<Data>().FirstLineDrawArea = new Rectangle(DrawArea.Left, DrawArea.Top, DrawArea.Width,
				DrawArea.Height / DisplayCount);
			SetAppearance(IsEnabled ? Theme.SelectBox : Theme.SelectBoxDisabled);
			UpdateTexts();
			UpdateScrollbar();
		} //ncrunch: no coverage end

		private void UpdateTexts()
		{
			var data = Get<Data>();
			int count = DisplayCount;
			int mouseoverValue = GetMouseoverLineNumber();
			for (int i = 0; i < count; i++)
			{
				texts[i].DrawArea = Rectangle.FromCenter(DrawArea.Center.X,
					DrawArea.Top + (i + 0.5f) * data.FirstLineDrawArea.Height,
					DrawArea.Width * ReductionDueToBorder, data.FirstLineDrawArea.Height);
				texts[i].Color = i == mouseoverValue ? Color.White : Color.VeryLightGray;
				texts[i].Text = data.Values[i + Scrollbar.LeftValue].ToString();
			}
		}

		private const float ReductionDueToBorder = 0.9f;

		public override void Set(object component)
		{
			if (component is Scrollbar)
				AddChild((Scrollbar)component);
			base.Set(component);
			if (component is Data)
				ProcessData((Data)component);
		}

		private void ProcessData(Data data)
		{
			DrawArea = new Rectangle(data.FirstLineDrawArea.Left, data.FirstLineDrawArea.Top,
				data.FirstLineDrawArea.Width, data.FirstLineDrawArea.Height * data.MaxDisplayCount);
			Values = data.Values;
		}
	}
}