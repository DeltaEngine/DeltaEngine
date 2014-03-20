using System;
using System.Collections.Generic;
using System.Threading;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Provides an object which can be scoped and is static within a thread (e.g. tests). Based on
	/// http://startbigthinksmall.wordpress.com/2008/04/24/nice-free-and-reusable-net-ambient-context-pattern-implementation/
	/// </summary>
	public class ThreadStatic<T>
	{
		public ThreadStatic() {}

		public ThreadStatic(T fallback)
		{
			this.fallback = fallback;
			isFallbackDefined = true;
		}

		private readonly T fallback;
		private readonly bool isFallbackDefined;

		public T CurrentOrDefault
		{
			get { return HasCurrent ? Current : default(T); }
		}

		public bool HasCurrent
		{
			get { return isFallbackDefined || ThreadStatics.ContainsKey(this); }
		}

		private static Dictionary<ThreadStatic<T>, ThreadStaticValue> ThreadStatics
		{
			get
			{
				return threadStatics ??
					(threadStatics = new Dictionary<ThreadStatic<T>, ThreadStaticValue>(1));
			}
		}

		[ThreadStatic]
		private static Dictionary<ThreadStatic<T>, ThreadStaticValue> threadStatics;

		public T Current
		{
			get
			{
				ThreadStaticValue current;
				ThreadStatics.TryGetValue(this, out current);
				if (current != null)
					return current.value;
				if (isFallbackDefined)
					return fallback;
				throw new NoValueAvailable();
			}
		}

		public class NoValueAvailable : Exception {}

		public IDisposable Use(T value)
		{
			ThreadStaticValue old;
			ThreadStatics.TryGetValue(this, out old);
			return ThreadStatics[this] = new ThreadStaticValue(this, value, old);
		}

		private class ThreadStaticValue : IDisposable
		{
			public ThreadStaticValue(ThreadStatic<T> key, T value, ThreadStaticValue previous)
			{
				threadId = Thread.CurrentThread.ManagedThreadId;
				this.key = key;
				this.value = value;
				this.previous = previous;
			}

			internal readonly int threadId;
			private readonly ThreadStatic<T> key;
			internal readonly T value;
			internal readonly ThreadStaticValue previous;

			internal void MarkAsDisposed()
			{
				isDisposed = true;
				GC.SuppressFinalize(this);
			}

			private bool isDisposed;

			public void Dispose()
			{
				if (isDisposed)
					return;
				key.DisposeScope(this);
			}
		}

		private void DisposeScope(ThreadStaticValue valueToDispose)
		{
			if (Thread.CurrentThread.ManagedThreadId != valueToDispose.threadId)
				throw new DisposingOnDifferentThreadToCreation(); // ncrunch: no coverage
			ThreadStatics.TryGetValue(this, out innerMost);
			while (innerMost != valueToDispose)
				DisposeInnerMostScopes();
			DisposeCurrentScope(valueToDispose);
		}

		public class DisposingOnDifferentThreadToCreation : Exception {}

		private ThreadStaticValue innerMost;

		private void DisposeInnerMostScopes()
		{
			innerMost.MarkAsDisposed();
			innerMost = innerMost.previous;
		}

		private void DisposeCurrentScope(ThreadStaticValue valueToDispose)
		{
			valueToDispose.MarkAsDisposed();
			if (valueToDispose.previous == null)
				ThreadStatics.Remove(this);
			else
				ThreadStatics[this] = valueToDispose.previous;
		}
	}
}