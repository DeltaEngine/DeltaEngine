using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Entities
{
	/// <summary>
	/// Here we experiment around with the Update and Draw threads proposed by case 4269 to be
	/// implemented into the EntitiesRunner and the AutofacStarter to be used by all apps.
	/// See discussion in forum.deltaengine.net, this class was not updated since.
	/// </summary>
	public class UpdateAndDrawThreadsTests
	{
		//ncrunch: no coverage start
		[Test, Ignore]
		public void UpdateAndDrawInParallel()
		{
			const float UpdateTimeStep = 0.2f;
			const float RenderTimeStep = 0.016f;
			var entities = new MockEntitiesRunner(typeof(MockUpdateBehavior));
			var state = new PositionEntity(Vector2D.Zero);
			float drawTime = 0.0f;
			float updateTime = 0.0f;
			float accumulator = 0.0f;
			while (drawTime < 10.0f)
			{
				drawTime += RenderTimeStep;
				accumulator += RenderTimeStep;
				while (accumulator >= UpdateTimeStep)
				{
					state.InvokeNextUpdateStarted();
					state.Update();
					updateTime += UpdateTimeStep;
					accumulator -= UpdateTimeStep;
				}
				float interpolation = accumulator / UpdateTimeStep;
				var interpolatedPosition = state.Position;
				Console.WriteLine("interpolatedPosition=" + interpolatedPosition +
					", drawTime=" + drawTime +
					", updateTime=" + updateTime +
					", interpolation=" + interpolation);
			}
			entities.Dispose();
		}

		public sealed class PositionEntity : DrawableEntity, Updateable
		{
			public PositionEntity(Vector2D pos)
			{
				Add(pos);
			}

			public Vector2D Position
			{
				get { return Get<Vector2D>(); }
				set { Set(value); }
			}

			public void Update()
			{
				Position += Vector2D.One;
			}
		}
	}
}