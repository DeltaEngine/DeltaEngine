using System;
using DeltaEngine.Core;
using DeltaEngine.Entities;

namespace DeltaEngine.Mocks
{
	/// <summary>
	/// Mock Entities Runner for fast resolution of behavior during unit testing.
	/// </summary>
	public class MockEntitiesRunner : EntitiesRunner
	{
		public MockEntitiesRunner(params Type[] typesToResolve)
			: base(new MockBehaviorResolver(typesToResolve), new MockSettings())
		{
			new MockGlobalTime();
		}

		public new void Dispose()
		{
			GlobalTime.Current.Dispose();
			base.Dispose();
		}

		public void RunEntities()
		{
			isPaused = false;
			UpdateGlobalTimeAndRunEntities();
		}

		private bool isPaused;

		private void UpdateGlobalTimeAndRunEntities()
		{
			for (int i = 0; i < (int)(MockGlobalTime.UpdatePerSecond * UpdateTimeStep); i++)
				GlobalTime.Current.Update();
			UpdateAndDrawAllEntities(() => { });
		}

		public void RunEntitiesPaused()
		{
			isPaused = true;
			UpdateGlobalTimeAndRunEntities();
		}

		protected override bool GetWhetherAppIsPaused()
		{
			return isPaused;
		}

		private class MockBehaviorResolver : BehaviorResolver
		{
			public MockBehaviorResolver(Type[] typesToResolve)
			{
				this.typesToResolve = typesToResolve;
			}

			private readonly Type[] typesToResolve;

			public UpdateBehavior ResolveUpdateBehavior(Type behaviorType)
			{
				foreach (var type in typesToResolve)
					if (type == behaviorType)
						return Activator.CreateInstance(type) as UpdateBehavior;
				return null; //ncrunch: no coverage
			}

			public DrawBehavior ResolveDrawBehavior(Type behaviorType)
			{
				foreach (var type in typesToResolve)
					if (type == behaviorType)
						return Activator.CreateInstance(type) as DrawBehavior;
				return null; //ncrunch: no coverage
			}
		}
	}
}