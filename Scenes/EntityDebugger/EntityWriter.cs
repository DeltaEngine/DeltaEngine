using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;

namespace DeltaEngine.Scenes.EntityDebugger
{
	/// <summary>
	/// The app does not need to be paused to use EntityWriter but it likely makes sense to do so: 
	/// Every frame the contents of the controls are written back to the Entity. If invoked through 
	/// EntitySelector the app will be paused. Components can be edited but not added or removed.
	/// </summary>
	public class EntityWriter : EntityEditor
	{
		public EntityWriter(Entity2D entity)
			: base(entity) {}

		public override void Update()
		{
			if (Entity != null && Entity.RotationCenter.IsNearlyEqual(Entity.DrawArea.Center))
				KeepRotationCenterInLineWithDrawAreaCenter();
			foreach (var pair in componentControls)
				UpdateComponentFromControls(pair.Key, pair.Value);
		}

		private void KeepRotationCenterInLineWithDrawAreaCenter()
		{
			List<Control> drawAreaControls;
			List<Control> rotationCenterControls;
			if (componentControls.TryGetValue(typeof(Rectangle), out drawAreaControls))
				if (componentControls.TryGetValue(typeof(Vector2D), out rotationCenterControls))
					UpdateRotationCenterIfDrawAreaHasChanged(((TextBox)drawAreaControls[1]).Text,
						(TextBox)rotationCenterControls[1]);
		}

		private void UpdateRotationCenterIfDrawAreaHasChanged(string drawAreaText, 
			TextBox rotationCenterControl)
		{
			var drawArea = (Rectangle)GetComponentFromString(typeof(Rectangle), drawAreaText);
			if (Entity.DrawArea != drawArea)
				rotationCenterControl.Text = drawArea.Center.ToString();
		}

		private static object GetComponentFromString(Type componentType, string text)
		{
			try
			{
				return TryGetComponentFromString(componentType, text);
			}
			catch (Exception)
			{
				return Activator.CreateInstance(componentType);
			}
		}

		private static object TryGetComponentFromString(Type componentType, string text)
		{
			return Activator.CreateInstance(componentType, new object[] { text });
		}

		private void UpdateComponentFromControls(Type componentType, List<Control> controls)
		{
			if (componentType == typeof(Color))
				UpdateColorFromSliders(controls);
			else
				UpdateComponentFromTextBox(componentType, (TextBox)controls[1]);
		}

		private void UpdateColorFromSliders(List<Control> controls)
		{
			var r = (byte)((Slider)controls[1]).Value;
			var g = (byte)((Slider)controls[3]).Value;
			var b = (byte)((Slider)controls[5]).Value;
			var a = (byte)((Slider)controls[7]).Value;
			var color = new Color(r, g, b, a);
			if (Entity.Get<Color>() != color)
				Entity.Set(color);
		}

		private void UpdateComponentFromTextBox(Type componentType, TextBox textbox)
		{
			if (textbox.Text != textbox.PreviousText)
				if (componentType == typeof(float))
					UpdateFloatIfValueHasChanged(textbox.Text);
				else
					UpdateComponentIfValueHasChanged(componentType, textbox);
		}

		private void UpdateFloatIfValueHasChanged(string text)
		{
			try
			{
				TryUpdateFloatIfValueHasChanged(text);
			}
			catch (Exception)
			{
				SetFloatToZero();
			}
		}

		private void TryUpdateFloatIfValueHasChanged(string text)
		{
			var value = text.Convert<float>();
			if (Entity.Get<float>() != value)
				Entity.Set(value);
		}

		private void SetFloatToZero()
		{
			if (Entity.Get<float>() != 0.0f)
				Entity.Set(0.0f);
		}

		private void UpdateComponentIfValueHasChanged(Type componentType, TextBox textbox)
		{
			object newComponent = GetComponentFromString(componentType, textbox.Text);
			List<object> oldComponents = Entity.GetComponentsForEditing();
			foreach (var oldComponent in oldComponents)
				if (oldComponent.GetType() == componentType &&
					oldComponent.ToString() != newComponent.ToString())
					Entity.Set(newComponent);
		}
	}
}