using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Scenes.EntityDebugger
{
	/// <summary>
	/// Right-clicking an Entity selects it for either observing or editing
	/// </summary>
	public class EntitySelector
	{
		public EntitySelector()
		{
			new Command(Clicked).Add(new MouseButtonTrigger(MouseButton.Right));
		}

		private void Clicked(Vector2D position)
		{
			Time.IsPaused = false;
			DisposeOldEditor();
			StartNewEditorIfEntityFound(position);
			PauseAppIfEditorIsWriter();
		}

		private void DisposeOldEditor()
		{
			if (ActiveEditor != null)
				ActiveEditor.Dispose();
			ActiveEditor = null;
		}

		private void StartNewEditorIfEntityFound(Vector2D position)
		{
			foreach (
				Entity2D entity in entities.Where(entity => entity.RotatedDrawAreaContains(position)))
				if (EditorMode == EditorMode.Read)
					ActiveEditor = new EntityReader(entity);
				else
					// Can't test this with ncrunch as pausing time can mess up other tests at random
					ActiveEditor = new EntityWriter(entity); // ncrunch: no coverage
		}

		internal EntityEditor ActiveEditor { get; private set; }

		private void PauseAppIfEditorIsWriter()
		{
			if (ActiveEditor is EntityWriter)
				// Can't test this with ncrunch as pausing time can mess up other tests at random
				Time.IsPaused = true; // ncrunch: no coverage
		}

		public EditorMode EditorMode { get; set; }

		internal readonly List<Entity2D> entities = new List<Entity2D>();

		public void Add(Entity2D entity)
		{
			entities.Add(entity);
		}
	}
}