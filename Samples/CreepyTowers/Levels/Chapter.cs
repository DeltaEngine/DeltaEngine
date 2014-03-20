using System;
using CreepyTowers.Content;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D.Shapes;

namespace CreepyTowers.Levels
{
	public abstract class Chapter : IDisposable
	{
		protected Chapter(LevelMaps levelMap)
		{
			GameLevel = ContentLoader.Load<GameLevel>(levelMap.ToString());
			if (GameLevel == null)
				return;
			GameLevel.ChapterFinished += Completed;
			CreateLevel();
		}

		public static Chapter Current;

		protected void CreateLevel()
		{
			UpdateCamera();
			InitializePlayer();
			InitializeLevel();
			RenderLevel();
		}

		protected virtual void UpdateCamera() {}
		protected virtual void InitializePlayer() {}

		protected virtual void InitializeLevel()
		{
			GameLevel.RenderIn3D = true;
		}

		protected virtual void RenderLevel()
		{
			if (GameLevel == null)
				return;
			//levelGrid = new Grid3D(new Vector3D(GameLevel.Size / 2), GameLevel.Size);
			//levelGrid.RenderLayer = -10;
			//levelGrid.IsVisible = false;
			new LevelDebugRenderer(GameLevel);
			GameLevel.RenderLevel();
		}

		//private Grid3D levelGrid;

		protected abstract void Completed();
		protected GameLevel GameLevel { get; private set; }

		public virtual void Restart()
		{
			GameLevel.Restart();
			InitializePlayer();
		}

		public virtual void Dispose()
		{
			//levelGrid.Dispose();
			GameLevel.Dispose();
			Current = null;
		}
	}
}