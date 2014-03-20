using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// Allows one value to be selected from a dropdown list of values
	/// </summary>
	public class DropdownList : Label
	{
		protected DropdownList()
		{
			Clicked += ToggleSelectionBoxVisibility;			
		}

		public DropdownList(Rectangle drawArea, List<object> values)
			: this(new Theme(),  drawArea, values) {}

		public DropdownList(Theme theme, Rectangle drawArea, List<object> values)
			: base(theme, theme.DropdownListBox, drawArea)
		{
			var selectBox = new SelectBox(theme,
				new Rectangle(drawArea.Left, drawArea.Top + drawArea.Height, drawArea.Width,
					drawArea.Height), values) { IsVisible = false };
			selectBox.LineClicked += SelectNewValue;
			Add(selectBox);
			AddChild(selectBox);
			Values = values;
			Clicked += ToggleSelectionBoxVisibility;
			FontText.HorizontalAlignment = HorizontalAlignment.Left;
		}

		internal SelectBox SelectBox
		{
			get { return Get<SelectBox>(); }
		}

		public List<object> Values
		{
			get { return SelectBox.Values; }
			set
			{
				SelectBox.Values = value;
				if (SelectedValue == null || !value.Contains(SelectedValue))
					SelectedValue = value[0];
			}
		}

		public object SelectedValue
		{
			get { return selectedValue; }
			set
			{
				if (!SelectBox.Values.Contains(value))
					throw new SelectedValueMustBeOneOfTheListOfValues();
				selectedValue = value;
				Text = value.ToString();
			}
		}

		private object selectedValue;

		public class SelectedValueMustBeOneOfTheListOfValues : Exception {}

		private void SelectNewValue(int lineNumber)
		{
			if (!SelectBox.IsVisible)
				return;
			SelectedValue = Values[lineNumber];
			SelectBox.IsVisible = false;
		}

		private void ToggleSelectionBoxVisibility()
		{
			SelectBox.IsVisible = !SelectBox.IsVisible;
		}

		public override void Update()
		{
			base.Update();
			SetAppearance(IsEnabled ? Theme.DropdownListBox : Theme.DropdownListBoxDisabled);
			if (!Contains<SelectBox>())
				return;
			SelectBox.DrawArea = new Rectangle(DrawArea.Left, DrawArea.Top + DrawArea.Height,
				DrawArea.Width, DrawArea.Height * SelectBox.DisplayCount);
		}

		public override bool IsEnabled
		{
			get { return base.IsEnabled; }
			set
			{
				base.IsEnabled = value;
				if (!value)
					SelectBox.IsVisible = false;
			}
		}

		public int MaxDisplayCount
		{
			get { return SelectBox.MaxDisplayCount; }
			set { SelectBox.MaxDisplayCount = value; }
		}

		public override void Set(object component)
		{
			if (component is SelectBox)
				ProcessSelectBox((SelectBox)component);
			base.Set(component);
		}

		private void ProcessSelectBox(SelectBox selectBox)
		{
			selectBox.LineClicked += SelectNewValue;
			AddChild(selectBox);
		}
	}
}