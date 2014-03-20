using System;
using System.Threading;
using DeltaEngine.Core;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Core
{
	public class GlobalTimeTests
	{
		[SetUp]
		public void InitializeMockTime()
		{
			time = new MockGlobalTime();
		}

		private GlobalTime time;

		[Test]
		public void RunTime()
		{
			time.Update();
			Assert.LessOrEqual(time.Milliseconds, 50);
		}

		[Test]
		public void RunTimeWithStopwatch()
		{
			time = new StopwatchTime();
			time.Update();
			Assert.IsTrue(time.Milliseconds < 2, "Milliseconds=" + time.Milliseconds);
		}
		
		[Test]
		public void CalculateFps()
		{
			do
				time.Update();
			while (time.Milliseconds <= 1000);
			Assert.IsTrue(Math.Abs(time.Fps - 20) <= 1, "Fps=" + time.Fps);
		}

		[Test]
		public void GetSecondsSinceStartToday()
		{
			Assert.LessOrEqual(time.GetSecondsSinceStartToday(), 1.0f / 20.0f);
			time.Update();
			Assert.AreNotEqual(0.0f, time.GetSecondsSinceStartToday());
		}
		
		[Test]
		public void FpsShouldBeSixty()
		{
			Assert.AreEqual(60, GlobalTime.Current.Fps);
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void CalculateFpsWithStopwatch()
		{
			time = new StopwatchTime();
			const int TargetFps = 30;
			do
			{
				Thread.Sleep(1000 / TargetFps);
				time.Update();
			} while (time.Milliseconds < 1000);
			Assert.IsTrue(Math.Abs(time.Fps - TargetFps) <= 5, "Fps=" + time.Fps);
		}
	}
}