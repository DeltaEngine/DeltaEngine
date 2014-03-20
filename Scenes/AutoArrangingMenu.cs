using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// A simple menu system where all buttons are the same size and auto-arrange on screen.
	/// When any control is added to the scene it will also autoassign the renderlayer. This applies
	/// to all controls, not just menu buttons.
	/// </summary>
	public class AutoArrangingMenu : Scene
	{
		protected AutoArrangingMenu(string contentName)
			: base(contentName) {}

		public AutoArrangingMenu(Size buttonSize, int renderLayer = 0)
			: base("<GeneratedScene>")
		{
			this.buttonSize = buttonSize;
			center = Vector2D.Half;
			this.renderLayer = renderLayer;
		}

		private Size buttonSize;
		private Vector2D center;
		private int renderLayer;

		public Size ButtonSize
		{
			get { return buttonSize; }
			set
			{
				buttonSize = value;
				ArrangeButtons();
			}
		}

		private void ArrangeButtons()
		{
			var left = Center.X - ButtonSize.Width / 2;
			for (int i = 0; i < buttons.Count; i++)
				buttons[i].DrawArea = new Rectangle(left, GetButtonTop(i), ButtonSize.Width,
					ButtonSize.Height);
		}

		private readonly List<InteractiveButton> buttons = new List<InteractiveButton>();

		internal List<InteractiveButton> Buttons
		{
			get { return buttons; }
		}

		private float GetButtonTop(int button)
		{
			float gapHeight = ButtonSize.Height / 2;
			float totalHeight = buttons.Count * ButtonSize.Height + (buttons.Count - 1) * gapHeight;
			float top = Center.Y - totalHeight / 2;
			return top + button * (ButtonSize.Height + gapHeight);
		}

		public Vector2D Center
		{
			get { return center; }
			set
			{
				center = value;
				ArrangeButtons();
			}
		}

		public override void Clear()
		{
			base.Clear();
			buttons.Clear();
		}

		public void ClearMenuOptions()
		{
			foreach (InteractiveButton button in buttons)
				Remove(button);
			buttons.Clear();
		}

		public void AddMenuOption(Action clicked, string text = "")
		{
			AddMenuOption(new Theme(), clicked, text);
		}

		public void AddMenuOption(Theme theme, Action clicked, string text = "")
		{
			AddButton(theme, clicked, text);
			ArrangeButtons();
		}

		private void AddButton(Theme theme, Action clicked, string text)
		{
			var button = new InteractiveButton(theme, new Rectangle(Vector2D.Zero, ButtonSize), text);
			button.Clicked += clicked;
			buttons.Add(button);
			Add(button);
		}

		public override void Add(Entity2D control)
		{
			base.Add(control);
			control.RenderLayer = renderLayer;
		}

		public int RenderLayer
		{
			get { return renderLayer; }
			set
			{
				renderLayer = value;
				foreach (var entity in Controls)
					entity.RenderLayer = renderLayer;
			}
		}
	}
}