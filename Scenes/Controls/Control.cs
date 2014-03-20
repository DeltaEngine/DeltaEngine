using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// The basis for most UI controls that can respond to mouse or touch input. Although it is a Sprite
	/// it defaults to a transparent material as a Control. Can also be used as a container (e.g. Panel)
	/// </summary>
	public class Control : Sprite, Updateable
	{
		protected Control()
		{
			AnchoringSize = Size.Unused;
			SceneDrawArea = Rectangle.Unused;
		}

		internal Size AnchoringSize { get; set; }
		internal Rectangle SceneDrawArea { get; set; }

		protected Control(Rectangle drawArea)
			: base(CreateEmptyTransparentMaterial(), drawArea)
		{
			Add(new InteractiveState());
			Add(new AnchoringState());
			Start<ControlUpdater>();
			AssignName();
			AnchoringSize = Size.Unused;
			SceneDrawArea = Rectangle.Unused;
		}

		private static Material CreateEmptyTransparentMaterial()
		{
			var ui2DShaderData = new ShaderCreationData(ShaderFlags.Position2DColored);
			return new Material(ContentLoader.Create<Shader>(ui2DShaderData), null)
			{
				DefaultColor = Color.TransparentBlack
			};
		}

		private void AssignName()
		{
			var controls = EntitiesRunner.Current.GetEntitiesOfType<Control>();
			int id = 0;
			string name;
			do
			{
				name = GetTypeName() + ++id;
			} while (controls.Any(control => control != this && control.Name == name));
			Name = name;
		}

		private string GetTypeName()
		{
			return GetType().ToString().Split('.')[GetType().ToString().Split('.').Count() - 1];
		}

		public string Name
		{		
			get { return Contains<string>() ? Get<string>() : "";  }
			set { Set(value); }
		}

		protected void AddChild(Entity2D entity)
		{
			if (children.Any(c => c.Entity2D == entity))
				return;
			children.Add(new Child(entity));
			entity.RenderLayer = RenderLayer + children.Count;
		}

		internal readonly List<Child> children = new List<Child>();

		protected internal class Child
		{
			protected Child() {} //ncrunch: no coverage

			public Child(Entity2D control)
			{
				Entity2D = control;
				Control = control as Control;
				WasEnabled = true;
				WasActive = true;
				WasVisible = true;
			}

			public Entity2D Entity2D { get; private set; }
			public Control Control { get; private set; }
			public bool WasEnabled { get; set; }
			public bool WasActive { get; set; }
			public bool WasVisible { get; set; }
		}

		protected void RemoveChild(Entity2D entity)
		{
			var child = children.FirstOrDefault(c => c.Entity2D == entity);
			if (child != null)
				children.Remove(child);
		}

		public virtual bool IsEnabled
		{
			get { return isEnabled; }
			set
			{
				if (isEnabled == value)
					return;
				isEnabled = value;
				if (isEnabled)
					EnableControl();
				else
					DisableControl();
			}
		}
		private bool isEnabled = true;

		private void EnableControl()
		{
			foreach (Child child in children.Where(c => c.Control != null))
				child.Control.IsEnabled = child.WasEnabled;
		}

		private void DisableControl()
		{
			foreach (Child child in children.Where(c => c.Control != null))
			{
				child.WasEnabled = child.Control.IsEnabled;
				child.Control.IsEnabled = false;
			}
		}

		public override bool IsActive
		{
			get { return base.IsActive; }
			set
			{
				if (base.IsActive == value)
					return;
				base.IsActive = value;
				if (value)
					ActivateChildControls();
				else
					InactivateChildControls();
			}
		}

		private void ActivateChildControls()
		{
			foreach (Child child in children)
				child.Entity2D.IsActive = child.WasActive;
		}

		private void InactivateChildControls()
		{
			foreach (Child child in children)
			{
				child.WasActive = child.Entity2D.IsActive;
				child.Entity2D.IsActive = false;
			}
		}

		public override void ToggleVisibility()
		{
			base.ToggleVisibility();
			if (IsVisible)
				ShowChildControls();
			else
				HideChildControls();
		}

		private void ShowChildControls()
		{
			foreach (Child child in children)
				child.Entity2D.IsVisible = child.WasVisible;
		}

		private void HideChildControls()
		{
			foreach (Child child in children)
			{
				child.WasVisible = child.Entity2D.IsVisible;
				child.Entity2D.IsVisible = false;
			}
		}

		public virtual void Update()
		{
			for (int i = 0; i < children.Count; i++)
				children[i].Entity2D.RenderLayer = RenderLayer + i + 1;
		}

		public InteractiveState State
		{
			get
			{
				if (Contains<InteractiveState>())
					return Get<InteractiveState>();
				var state = new InteractiveState(); //ncrunch: no coverage start
				Add(state);
				return state; //ncrunch: no coverage end
			}
		}

		public virtual void Click()
		{
			if (Clicked != null)
				Clicked();
		}

		public Action Clicked;

		public override bool IsPauseable
		{
			get { return false; }
		}

		public Margin LeftMargin
		{
			get { return AnchoringState.LeftMargin; }
			set { AnchoringState.LeftMargin = value; }
		}

		private AnchoringState AnchoringState
		{
			get { return Get<AnchoringState>(); }
		}

		public Margin RightMargin
		{
			get { return AnchoringState.RightMargin; }
			set { AnchoringState.RightMargin = value; }
		}

		public Margin TopMargin
		{
			get { return AnchoringState.TopMargin; }
			set { AnchoringState.TopMargin = value; }
		}

		public Margin BottomMargin
		{
			get { return AnchoringState.BottomMargin; }
			set { AnchoringState.BottomMargin = value; }
		}

		public int GetLevel()
		{
			var leftLevel = LeftMargin.Other == null ? 0 : LeftMargin.Other.GetLevel() + 1;
			var rightLevel = RightMargin.Other == null ? 0 : RightMargin.Other.GetLevel() + 1;
			var topLevel = TopMargin.Other == null ? 0 : TopMargin.Other.GetLevel() + 1;
			var bottomLevel = BottomMargin.Other == null ? 0 : BottomMargin.Other.GetLevel() + 1;
			return Math.Max(Math.Max(Math.Max(leftLevel, rightLevel), topLevel), bottomLevel);
		}

		internal void RefreshDrawAreaIfAnchored()
		{
			DrawArea = AnchoringState.CalculateDrawArea(this);
		}
	}
}