using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// The base for Slider and Scrollbar controls.
	/// </summary>
	public abstract class BaseSlider : Picture
	{
		protected BaseSlider() {}

		protected BaseSlider(Theme theme, Material material, Rectangle drawArea)
			: base(theme, material, drawArea) {}

		public override void Update()
		{
			base.Update();
			UpdateSliderAppearance();
			UpdatePointerAppearance();
			UpdatePointerState();
			UpdatePointerValue();
			UpdatePointerDrawArea();
			UpdatePointerRotation();
		}

		protected abstract void UpdateSliderAppearance();

		protected abstract void UpdatePointerAppearance();

		private void UpdatePointerState()
		{
			if (!State.IsInside || !State.IsPressed)
				return;
			Pointer.State.IsInside = true;
			Pointer.State.IsPressed = true;
		}

		protected internal Picture Pointer
		{
			get { return Get<Picture>(); }
		}

		public override sealed void Set(object component)
		{
			if (component is Picture)
				ReplaceChild((Picture)component);
			base.Set(component);
		}

		private void ReplaceChild(Picture pointer)
		{
			if (Contains<Picture>())
				RemoveChild(Get<Picture>());
			AddChild(pointer);
		}

		protected abstract void UpdatePointerValue();

		protected abstract void UpdatePointerDrawArea();

		private void UpdatePointerRotation()
		{
			Pointer.Rotation = Rotation;
			Pointer.RotationCenter = RotationCenter;
		}
	}
}