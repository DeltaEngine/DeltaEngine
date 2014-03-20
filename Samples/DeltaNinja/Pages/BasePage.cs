using System;
using System.Collections.Generic;
using System.Diagnostics;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.ScreenSpaces;
using DeltaNinja.UI;

namespace DeltaNinja.Pages
{
	public abstract class BasePage : Scene
	{
		internal const float TitleTopMargin = 0.02f;
		//internal const float LINKS_BottomMargin = 0.03f;

		protected BasePage(ScreenSpace screen)
		{
			this.screen = screen;
			links = new List<LogoLink>();
			new Command(OnMouseMovement).Add(new MouseMovementTrigger());
			new Command(OnMouseClick).Add(new MouseButtonTrigger());
		}

		public event Action<MenuButton> ButtonClicked;

		private readonly ScreenSpace screen;
		private readonly List<LogoLink> links;

		public void SetTitle(string imageName, float width, float ratio, float topOffset)
		{
			var view = screen.Viewport;
			float center = view.Width / 2f;
			float offset = view.Top + topOffset;
			var title = new Sprite(imageName, new Rectangle(0, 0, width, width / ratio));
			title.Center = new Vector2D(center, offset + TitleTopMargin + title.DrawArea.Height / 2f);
			title.RenderLayer = (int)GameRenderLayer.Menu;
			Add(title);
		}

		public void AddLogoLink(string imageName, string link, float size, int offset)
		{
			var view = screen.Viewport;
			float center = (view.Width / 2f) + (offset * size);
			var logo = new LogoLink(imageName, link, size);
			logo.Center = new Vector2D(center, view.Bottom - size);
			logo.RenderLayer = (int)GameRenderLayer.Menu;
			links.Add(logo);
			Add(logo);
		}

		public void AddButton(MenuButton code, float width, float ratio)
		{
			var material = new Material(ShaderFlags.Position2DTextured, code + "Button");
			var theme = new Theme
			{
				Button = material,
				ButtonMouseover =
					new Material(ShaderFlags.Position2DTextured, code + "Button")
					{
						DefaultColor = DefaultColors.HoverButton
					},
				ButtonPressed = material,
				Font = ContentLoader.Load<Font>("Verdana12"),
			};
			buttonTop += 0.1f;
			var button = new Button(theme, new Rectangle(0.4f, buttonTop, width, width / ratio));
			button.RenderLayer = 9000;
			button.IsVisible = false;
			button.Clicked += () => OnButtonClicked(code);
			Add(button);
		}

		private float buttonTop = 0.25f;

		protected virtual void OnButtonClicked(MenuButton code)
		{
			var handler = ButtonClicked;
			if (handler != null)
				handler(code);
		}

		private void OnMouseClick(Vector2D position)
		{
			foreach (var link in links)
				if (link.IsHover(position))
					Process.Start(link.Url);
		}

		private void OnMouseMovement(Vector2D position)
		{
			foreach (var link in links)
				link.Color = link.IsHover(position) ? DefaultColors.Yellow : Color.White;
		}
	}
}