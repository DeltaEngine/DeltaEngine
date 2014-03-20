using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Scenes.EntityDebugger
{
	/// <summary>
	/// Base for EntityReader which shows the real-time values for Entity components, and EntityWriter
	/// which allows those values to be changed.
	/// </summary>
	public abstract class EntityEditor : Entity, Updateable
	{
		protected EntityEditor(Entity2D entity)
		{
			Entity = entity;
			ScreenSpace.Current.ViewportSizeChanged += RepositionControls;
			UpdatePriority = Priority.Last;
		}

		public Entity2D Entity
		{
			get { return entity; }
			set
			{
				entity = value;
				Reset();
			}
		}

		private Entity2D entity;

		protected void Reset()
		{
			row = 0;
			scene.Clear();
			componentControls.Clear();
			componentList = entity.GetComponentsForEditing();
			foreach (object component in componentList)
				AddComponentToScene(component);
		}

		private int row;
		internal readonly Scene scene = new Scene();
		protected readonly Dictionary<Type, List<Control>> componentControls =
			new Dictionary<Type, List<Control>>();
		protected List<object> componentList;

		private void AddComponentToScene(object component)
		{
			if (component is float)
				AddFloatTextBoxToScene((float)component);
			else if (component is Color)
				AddColorSlidersToScene((Color)component);
			else
				AddGenericComponentToSceneIfItCanBeCreatedFromString(component);
		}

		private void AddFloatTextBoxToScene(float value)
		{
			var label = new Label(GetNextLabelDrawArea(), "Rotation");
			var textbox = new TextBox(GetNextControlDrawArea(), value.ToInvariantString());
			componentControls.Add(typeof(float), new List<Control> { label, textbox });
			scene.Add(label);
			scene.Add(textbox);
		}

		private Rectangle GetNextLabelDrawArea()
		{
			Rectangle viewport = ScreenSpace.Current.Viewport;
			return new Rectangle(viewport.Left + SceneLeftEdge,
				viewport.Top + SceneTopEdge + row * ControlHeightPlusGap, LabelWidth, ControlHeight);
		}

		internal const float SceneLeftEdge = 0.025f;
		internal const float SceneTopEdge = 0.025f;
		internal const float LabelWidth = 0.11f;
		internal const float ControlHeight = 0.033f;
		private const float ControlHeightPlusGap = 0.05f;

		private Rectangle GetNextControlDrawArea()
		{
			Rectangle viewport = ScreenSpace.Current.Viewport;
			return new Rectangle(viewport.Left + SceneLeftEdge + LabelWidth + LabelToControlGap,
				viewport.Top + SceneTopEdge + (row++) * ControlHeightPlusGap, ControlWidth, ControlHeight);
		}

		internal const float LabelToControlGap = 0.025f;
		internal const float ControlWidth = 0.2f;

		private void AddColorSlidersToScene(Color color)
		{
			var labelR = CreateColorLabel("Red");
			var r = CreateColorSlider(color.R);
			var labelG = CreateColorLabel("Green");
			var g = CreateColorSlider(color.G);
			var labelB = CreateColorLabel("Blue");
			var b = CreateColorSlider(color.B);
			var labelA = CreateColorLabel("Alpha");
			var a = CreateColorSlider(color.A);
			var controls = new List<Control> { labelR, r, labelG, g, labelB, b, labelA, a };
			componentControls.Add(typeof(Color), controls);
			scene.Add(controls);
		}

		private Label CreateColorLabel(string color)
		{
			return new Label(GetNextLabelDrawArea(), color);
		}

		private Slider CreateColorSlider(byte value)
		{
			return new Slider(GetNextControlDrawArea()) { Value = value, MaxValue = 255 };
		}

		private void AddGenericComponentToSceneIfItCanBeCreatedFromString(object component)
		{
			if (CanComponentBeCreatedFromString(component.GetType()))
				AddGenericComponentToScene(component);
		}

		private static bool CanComponentBeCreatedFromString(Type type)
		{
			return type.GetConstructor(new[] { typeof(string) }) != null;
		}

		private void AddGenericComponentToScene(object component)
		{
			var label = new Label(GetNextLabelDrawArea(), GetName(component));
			var textbox = new TextBox(GetNextControlDrawArea(), component.ToString());
			var controls = new List<Control> { label, textbox };
			componentControls.Add(component.GetType(), controls);
			scene.Add(controls);
		}

		private static string GetName(object component)
		{
			if (component is Rectangle)
				return "Draw Area";
			if (component is Vector2D)
				return "Rot Center";
			return component.GetShortNameOrFullNameIfNotFound();
		}

		private void RepositionControls()
		{
			row = 0;
			foreach (var pair in componentControls)
				foreach (Control control in pair.Value)
					if (control is TextBox || !(control is Label))
						control.DrawArea = GetNextControlDrawArea();
					else
						control.DrawArea = GetNextLabelDrawArea();
		}

		public abstract void Update();

		public override bool IsPauseable { get { return false; } }

		public override void Dispose()
		{
			base.Dispose();
			scene.Clear();
			base.Dispose();
		}
	}
}