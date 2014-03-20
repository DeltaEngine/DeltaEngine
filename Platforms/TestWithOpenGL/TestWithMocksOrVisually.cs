using System;
using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D;
using NUnit.Framework;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Automatically tests with MockResolver when NCrunch is used, otherwise OpenGLResolver is used
	/// </summary>
	[TestFixture]
	public class TestWithMocksOrVisually
	{
		[SetUp]
		public void InitializeResolver()
		{
			if (StackTraceExtensions.ForceUseOfMockResolver())
			{
				resolver = new MockResolver();
				return;
			}
			//ncrunch: no coverage start
			if (!StackTraceExtensions.StartedFromProgramMain)
				StackTraceExtensions.SetUnitTestName(TestContext.CurrentContext.Test.FullName);
			resolver = new OpenGLResolver();
			if (StackTraceExtensions.IsCloseAfterFirstFrameAttributeUsed() ||
				StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
				Resolve<Window>().CloseAfterFrame();
			//ncrunch: no coverage end
		}

		private AppRunner resolver;

		protected bool IsMockResolver
		{
			get { return resolver is MockResolver; }
		}

		protected void RegisterMock<T>(T instance) where T : class
		{
			if (IsMockResolver)
				(resolver as MockResolver).RegisterMock(instance);
		}

		[TearDown]
		public void RunTestAndDisposeResolverWhenDone()
		{
			try
			{
				if (StackTraceExtensions.StartedFromProgramMain ||
					TestContext.CurrentContext.Result.Status == TestStatus.Passed)
					resolver.Run();
			}
			finally
			{
				resolver.Dispose();
			}
		}

		protected T Resolve<T>() where T : class
		{
			return resolver.Resolve<T>();
		}

		protected void RunAfterFirstFrame(Action executeOnce)
		{
			resolver.CodeAfterFirstFrame = executeOnce;
		}

		protected void AdvanceTimeAndUpdateEntities(
			float timeToAddInSeconds = 1.0f / Settings.DefaultUpdatesPerSecond)
		{
			var renderer2D = resolver.Resolve<BatchRenderer2D>();
			var renderer3D = resolver.Resolve<BatchRenderer3D>();
			var drawing = resolver.Resolve<Drawing>();
			if (CheckIfWeNeedToRunTickToAvoidInitializationDelay())
				RunTickOnce(renderer2D, renderer3D, drawing);
			var startTimeMs = GlobalTime.Current.Milliseconds;
			do
				RunTickOnce(renderer2D, renderer3D, drawing);
			while (GlobalTime.Current.Milliseconds - startTimeMs +
				MathExtensions.Epsilon < timeToAddInSeconds * 1000);
		}

		private bool CheckIfWeNeedToRunTickToAvoidInitializationDelay()
		{
			return !(resolver is MockResolver) && GlobalTime.Current.Milliseconds == 0;
		}

		private static void RunTickOnce(BatchRenderer2D batchRenderer2D, BatchRenderer3D batchRenderer3D,
			Drawing drawing)
		{
			GlobalTime.Current.Update();
			EntitiesRunner.Current.UpdateAndDrawAllEntities(() =>
			{
				batchRenderer2D.DrawAndResetBatches();
				batchRenderer3D.DrawAndResetBatches();
				drawing.DrawEverythingInCurrentLayer();
			});
		}
	}
}