using System;
using System.Collections.Generic;
using System.Globalization;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.ScreenSpaces;
using GameOfDeath.Items;

namespace $safeprojectname$
{
	public class UserInterface : Scene
	{
		public UserInterface()
		{
			viewport = ScreenSpace.Current.Viewport;
			SetViewportBackground("GrassBackground");
			AddUITop();
			AddUIBottom();
			AddUILeft();
			AddUIRight();
			AddScoreBoard();
			AddItems();
			RecalculateOnSizeChange();
			ScreenSpace.Current.ViewportSizeChanged += RecalculateOnSizeChange;
		}

		public static readonly Size QuadraticFullscreenSize = new Size(1920, 1920);

		private void AddUITop()
		{
			var topMaterial = ContentLoader.Load<Material>("MaterialUITop");
			topHeight = topMaterial.DiffuseMap.PixelSize.Height / QuadraticFullscreenSize.Height;
			topBorder = new Sprite(topMaterial,
				new Rectangle(viewport.Left, viewport.Top, viewport.Width, topHeight))
			{
				RenderLayer = (int)RenderLayers.Gui
			};
			Add(topBorder);
		}

		private float topHeight;
		private Sprite topBorder;
		private Rectangle viewport;

		private void AddUIBottom()
		{
			var bottomMaterial = ContentLoader.Load<Material>("MaterialUIBottom");
			bottomHeight = bottomMaterial.DiffuseMap.PixelSize.Height / QuadraticFullscreenSize.Height;
			bottomBorder = new Sprite(bottomMaterial,
				new Rectangle(viewport.Left, viewport.Bottom - bottomHeight, viewport.Width, bottomHeight))
			{
				RenderLayer = (int)RenderLayers.Gui
			};
			Add(bottomBorder);
		}

		private float bottomHeight;
		private Sprite bottomBorder;

		private void AddUILeft()
		{
			var leftMaterial = ContentLoader.Load<Material>("MaterialUILeft");
			leftWidth = leftMaterial.DiffuseMap.PixelSize.Width / QuadraticFullscreenSize.Width;
			leftBorder = new Sprite(leftMaterial,
				new Rectangle(viewport.Left, viewport.Top + topHeight, leftWidth,
					viewport.Height - (bottomHeight + topHeight))) { RenderLayer = (int)RenderLayers.Gui };
			Add(leftBorder);
		}

		private float leftWidth;
		private Sprite leftBorder;

		private void AddUIRight()
		{
			var rightMaterial = ContentLoader.Load<Material>("MaterialUIRight");
			rightWidth = rightMaterial.DiffuseMap.PixelSize.Width / QuadraticFullscreenSize.Width;
			rightBorder = new Sprite(rightMaterial,
				new Rectangle(viewport.Right - rightWidth, viewport.Top + topHeight, rightWidth,
					viewport.Height - (bottomHeight + topHeight))) { RenderLayer = (int)RenderLayers.Gui };
			Add(rightBorder);
		}

		private float rightWidth;
		private Sprite rightBorder;

		private void AddScoreBoard()
		{
			bodyCountRabbit = new Sprite(ContentLoader.Load<Material>("MaterialDeadRabbit"),
				new Rectangle(viewport.TopRight + DeadRabbitOffset, DeadRabbitSize))
			{
				RenderLayer = (int)RenderLayers.Gui + 2
			};
			Add(bodyCountRabbit);

			MoneyText = new FontText(Font.Default, "$0",
				new Rectangle(viewport.TopLeft + MoneyOffset, new Size(0.2f, 0.03f)))
			{
				RenderLayer = (int)RenderLayers.Gui + 2,
				Color = Color.Black
			};
			Add(MoneyText);

			KillText = new FontText(Font.Default, "0",
				new Rectangle(viewport.TopRight + KillOffset, new Size(0.1f, 0.03f)))
			{
				RenderLayer = (int)RenderLayers.Gui + 2,
				Color = Color.Black
			};
			Add(KillText);
		}

		private Sprite bodyCountRabbit;
		private static readonly Vector2D DeadRabbitOffset = new Vector2D(-0.175f, 0.002f);
		private static readonly Size DeadRabbitSize = new Size(0.05f, 0.0667f);
		private static readonly Vector2D MoneyOffset = new Vector2D(0.02f, 0.02f);
		private static readonly Vector2D KillOffset = new Vector2D(-0.12f, 0.02f);

		public FontText MoneyText { get; private set; }
		public FontText KillText { get; private set; }

		private static Theme GetIconTheme(ItemType type)
		{
			switch (type)
			{
			case ItemType.Mallet:
				return new Theme
				{
					Button = ContentLoader.Load<Material>("MaterialIconMallet"),
					ButtonMouseover = ContentLoader.Load<Material>("MaterialIconMallet"),
					ButtonPressed = ContentLoader.Load<Material>("MaterialIconMallet")
				};
			case ItemType.Fire:
				return new Theme
				{
					Button = ContentLoader.Load<Material>("MaterialIconFire"),
					ButtonMouseover = ContentLoader.Load<Material>("MaterialIconFire"),
					ButtonPressed = ContentLoader.Load<Material>("MaterialIconFire")
				};
			case ItemType.Toxic:
				return new Theme
				{
					Button = ContentLoader.Load<Material>("MaterialIconToxic"),
					ButtonMouseover = ContentLoader.Load<Material>("MaterialIconToxic"),
					ButtonPressed = ContentLoader.Load<Material>("MaterialIconToxic")
				};
			case ItemType.Atomic:
				return new Theme
				{
					Button = ContentLoader.Load<Material>("MaterialIconAtomic"),
					ButtonMouseover = ContentLoader.Load<Material>("MaterialIconAtomic"),
					ButtonPressed = ContentLoader.Load<Material>("MaterialIconAtomic")
				};
			}
			return new Theme();
		}

		private void AddItems()
		{
			CreateItems();
			CreateIcons();
			SelectItem(0);
		}

		private void CreateItems()
		{
			CreateMallet();
			CreateFire();
			CreateToxic();
			CreateAtomic();
		}

		private void CreateMallet()
		{
			var mallet = new Mallet();
			cachedItems.Add(mallet);
		}

		private void CreateFire()
		{
			var fire = new Fire();
			cachedItems.Add(fire);
		}

		private void CreateToxic()
		{
			var toxic = new Toxic();
			cachedItems.Add(toxic);
		}

		private void CreateAtomic()
		{
			var atomic = new Atomic();
			cachedItems.Add(atomic);
		}

		private readonly List<Item> cachedItems = new List<Item>();

		private void CreateIcons()
		{
			for (int index = 0; index < NumberOfItems; index++)
			{
				icons[index] = new Button(GetIconTheme((ItemType)index),
					Rectangle.FromCenter(0.39f + index * 0.08f, ScreenSpace.Current.Top + 0.04f, 0.07f, 0.08f));
				int itemIndex = index;
				icons[index].Clicked += () => SelectItemIfSufficientFunds(itemIndex);
				icons[index].RenderLayer = (int)RenderLayers.Icons;
				Add(icons[index]);
			}
		}

		private const int NumberOfItems = 4;
		private readonly Button[] icons = new Button[NumberOfItems];

		private void SelectItem(int itemIndex)
		{
			CurrentItem.IsVisible = false;
			currentItemIndex = itemIndex;
			CurrentItem.IsVisible = true;
		}

		private int currentItemIndex;

		public Item CurrentItem
		{
			get { return cachedItems[currentItemIndex]; }
		}

		public void HandleItemInGame(Vector2D position)
		{
			if (position.X < ScreenSpace.Current.Viewport.Left + leftWidth ||
				position.X > ScreenSpace.Current.Viewport.Right - rightWidth ||
				position.Y < ScreenSpace.Current.Viewport.Top + topHeight ||
				position.Y > ScreenSpace.Current.Viewport.Bottom - bottomHeight)
				return;
			if (money >= CurrentItem.Cost)
				CreateEffect(position);
		}

		internal void SelectItemIfSufficientFunds(int itemIndex)
		{
			if (money >= cachedItems[itemIndex].Cost)
				SelectItem(itemIndex);
		}

		private void CreateEffect(Vector2D position)
		{
			if (DidDamage != null)
				CurrentItem.DoDamage = DidDamage;
			var effect = CurrentItem.CreateEffect(position);
			if (effect == null)
				return;
			Money -= CurrentItem.Cost;
		}

		public event Action<Vector2D, float, float> DidDamage;

		private void RecalculateOnSizeChange()
		{
			viewport = ScreenSpace.Current.Viewport;
			background.DrawArea = viewport;
			topBorder.DrawArea = new Rectangle(viewport.Left, viewport.Top, viewport.Width, topHeight);
			bottomBorder.DrawArea = new Rectangle(viewport.Left, viewport.Bottom - bottomHeight,
				viewport.Width, bottomHeight);
			leftBorder.DrawArea = new Rectangle(viewport.Left, viewport.Top + topHeight, leftWidth,
				viewport.Height - (bottomHeight + topHeight));
			rightBorder.DrawArea = new Rectangle(viewport.Right - rightWidth, viewport.Top + topHeight,
				rightWidth, viewport.Height - (bottomHeight + topHeight));
			bodyCountRabbit.DrawArea = new Rectangle(viewport.TopRight + DeadRabbitOffset,
				DeadRabbitSize);
			KillText.DrawArea = new Rectangle(viewport.TopRight + KillOffset, new Size(0.1f, 0.03f));
			MoneyText.DrawArea =
				new Rectangle(new Vector2D(viewport.Left + MoneyOffset.X, viewport.Top + MoneyOffset.Y),
					new Size(0.2f, 0.03f));
			for (int index = 0; index < icons.Length; index++)
				icons[index].DrawArea = Rectangle.FromCenter(0.39f + index * 0.08f,
					ScreenSpace.Current.Top + 0.04f, 0.07f, 0.08f);
		}

		public int Money
		{
			get { return money; }
			set
			{
				money = value;
				MoneyText.Text = "$" + money.ToString(CultureInfo.InvariantCulture);
			}
		}

		private int money;

		public int Kills
		{
			get { return kills; }
			set
			{
				kills = value;
				KillText.Text = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		private int kills;

		public void UpdateDisplayedItemPosition(Vector2D position)
		{
			if (position.X < ScreenSpace.Current.Viewport.Left + leftWidth ||
				position.X > ScreenSpace.Current.Viewport.Right - rightWidth ||
				position.Y < ScreenSpace.Current.Viewport.Top + topHeight ||
				position.Y > ScreenSpace.Current.Viewport.Bottom - bottomHeight)
				return;
			CurrentItem.UpdatePosition(position);
		}
	}
}