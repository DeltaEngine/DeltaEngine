using System;
using System.ComponentModel;
using System.Threading;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Tests.Extensions
{
	public class ThreadStaticTests
	{
		[Test]
		public void NewThreadStaticVariableHasNoCurrentValue()
		{
			var threadStatic = new ThreadStatic<string>();
			Assert.IsFalse(threadStatic.HasCurrent);
			Assert.Throws<ThreadStatic<string>.NoValueAvailable>(
				() => Assert.IsNotNull(threadStatic.Current));
		}

		[Test]
		public void NewThreadStaticVariableHasADefaultValue()
		{
			var threadStatic = new ThreadStatic<int>();
			Assert.AreEqual(0, threadStatic.CurrentOrDefault);
		}

		[Test]
		public void CurrentIsAvailableOnlyWithinScopeIfNoFallbackSet()
		{
			var threadStatic = new ThreadStatic<string>();
			using (threadStatic.Use("abc"))
				Assert.AreEqual("abc", threadStatic.Current);
			Assert.IsFalse(threadStatic.HasCurrent);
		}

		[Test]
		public void CurrentIsAvailableOutsideScopeIfFallbackSet()
		{
			var threadStatic = new ThreadStatic<Randomizer>(new PseudoRandom());
			using (threadStatic.Use(new FixedRandom()))
				Assert.IsTrue(threadStatic.Current is FixedRandom);
			Assert.IsTrue(threadStatic.Current is PseudoRandom);
		}

		[Test]
		public void MultipleScopesPushAndPopCorrectly()
		{
			var threadStatic = new ThreadStatic<int>();
			using (threadStatic.Use(1))
			{
				Assert.AreEqual(1, threadStatic.Current);
				TestMidScope(threadStatic);
				Assert.AreEqual(1, threadStatic.Current);
			}
		}

		private static void TestMidScope(ThreadStatic<int> threadStatic)
		{
			using (threadStatic.Use(2))
			{
				Assert.AreEqual(2, threadStatic.Current);
				TestInnerScope(threadStatic);
				Assert.AreEqual(2, threadStatic.Current);
			}
		}

		private static void TestInnerScope(ThreadStatic<int> threadStatic)
		{
			using (threadStatic.Use(3))
				Assert.AreEqual(3, threadStatic.Current);
		}

		[Test]
		public void DisposingMidScopeJumpsToOuterScope()
		{
			var threadStatic = new ThreadStatic<int>();
			threadStatic.Use(1);
			var midScope = threadStatic.Use(2);
			threadStatic.Use(3);
			Assert.AreEqual(3, threadStatic.Current);
			midScope.Dispose();
			Assert.AreEqual(1, threadStatic.Current);
		}

		[Test]
		public void DisposingInnerScopeTwiceHasNoEffect()
		{
			var threadStatic = new ThreadStatic<int>();
			threadStatic.Use(1);
			var innerScope = threadStatic.Use(2);
			Assert.AreEqual(2, threadStatic.Current);
			innerScope.Dispose();
			innerScope.Dispose();
			Assert.AreEqual(1, threadStatic.Current);
		}

		// ncrunch: no coverage start
		[Test, Ignore]
		public void DisposingOnADifferentThreadToCreationThrowsException()
		{
			var worker = new BackgroundWorker();
			worker.DoWork += CreateScope;
			worker.RunWorkerCompleted += DisposeScope;
			worker.RunWorkerAsync();
			Thread.Sleep(1);
		}

		private static void CreateScope(object sender, DoWorkEventArgs e)
		{
			e.Result = new ThreadStatic<int>().Use(1);
		}

		private static void DisposeScope(object sender, RunWorkerCompletedEventArgs e)
		{
			var scope = (IDisposable)e.Result;
			Assert.Throws<ThreadStatic<int>.DisposingOnDifferentThreadToCreation>(scope.Dispose);
		}
	}
}