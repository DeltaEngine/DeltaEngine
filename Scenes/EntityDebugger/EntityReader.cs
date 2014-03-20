using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;

namespace DeltaEngine.Scenes.EntityDebugger
{
	/// <summary>
	/// Scenes controls reveal the component values of an Entity, refreshing every frame.
	/// Use EntityWriter if you wish to be able to change these component values
	/// </summary>
	public class EntityReader : EntityEditor
	{
		public EntityReader(Entity2D entity)
			: base(entity) {}

		public override void Update()
		{
			if (HaveComponentsBeenAddedOrRemoved())
				Reset();
			else
				Refresh();
		}

		private bool HaveComponentsBeenAddedOrRemoved()
		{
			latestComponentList = Entity.GetComponentsForEditing();
			if (latestComponentList.Count != componentList.Count)
				return true;
			for (int i = 0; i < componentList.Count; i++)
				if (componentList[i].GetType() != latestComponentList[i].GetType())
					return true; //ncrunch: no coverage
			return false;
		}

		private List<object> latestComponentList;

		private void Refresh()
		{
			foreach (var component in latestComponentList)
				RefreshComponent(component);
		}

		private void RefreshComponent(object component)
		{
			if (component is float)
				RefreshFloatControl((float)component);
			else if (component is Color)
				RefreshColorControls((Color)component);
			else
				RefreshGenericControl(component);
		}

		private void RefreshFloatControl(float component)
		{
			List<Control> controls;
			componentControls.TryGetValue(component.GetType(), out controls);
			((TextBox)controls[1]).Text = component.ToInvariantString();
		}

		private void RefreshColorControls(Color component)
		{
			List<Control> controls;
			componentControls.TryGetValue(component.GetType(), out controls);
			((Slider)controls[1]).Value = component.R;
			((Slider)controls[3]).Value = component.G;
			((Slider)controls[5]).Value = component.B;
			((Slider)controls[7]).Value = component.A;
		}

		private void RefreshGenericControl(object component)
		{
			List<Control> controls;
			if (componentControls.TryGetValue(component.GetType(), out controls))
				((TextBox)controls[1]).Text = component.ToString();
		}
	}
}