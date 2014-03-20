using System;
using System.Linq;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace $safeprojectname$.GUI
{
	public abstract class Menu : IDisposable
	{
		protected abstract void CreateScene();
		public Scene Scene { get; set; }

		public virtual void Show()
		{
			if (Scene == null || Scene.Controls.Count == 0)
				return;
			Scene.Show();
			IsShown = true;
		}

		public bool IsShown { get; private set; }

		public virtual void Hide()
		{
			if (Scene == null || Scene.Controls.Count == 0)
				return;
			Scene.Hide();
			IsShown = false;
		}

		public void Enable()
		{
			foreach (Control control in Scene.Controls)
				control.IsEnabled = true;
		}

		public void Disable()
		{
			foreach (Control control in Scene.Controls)
				control.IsEnabled = false;
		}

		public Entity2D GetSceneControl(string controlName)
		{
			if (Scene.Controls.Count == 0 || Scene.Controls == null)
				return null;

			return Scene.Controls.FirstOrDefault(control => ((Control)control).Name == controlName);
		}

		public virtual void Reset() {}

		public void Dispose()
		{
			Scene.Dispose();
			IsShown = false;
		}
	}
}