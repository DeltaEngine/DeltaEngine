using System;
using System.Reflection;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	public class ExceptionExtensionsTests
	{
		[Test]
		public void CheckForFatalException()
		{
			Assert.IsFalse(new Exception().IsFatal());
			Assert.IsTrue(new Exception("", new InvalidOperationException()).IsFatal());
			Assert.IsTrue(new InvalidOperationException().IsFatal());
			Assert.IsTrue(new OutOfMemoryException().IsFatal());
			Assert.IsTrue(new AccessViolationException().IsFatal());
			Assert.IsTrue(new StackOverflowException().IsFatal());
			Assert.IsTrue(new TargetInvocationException(null).IsFatal());
			Assert.IsFalse(new NullReferenceException(null).IsFatal());
		}

		[Test]
		public void CheckForWeakException()
		{
			Assert.IsFalse(new Exception().IsWeak());
			Assert.IsTrue(new ArgumentNullException().IsWeak());
		}

		[Test]
		public void MakeSureFatalExceptionsAreRethrown()
		{
			try
			{
				TryMakeSureFatalExceptionsAreRethrown();
			} // ncrunch: no coverage
			catch (Exception ex)
			{
				Assert.IsTrue(ex.IsFatal());
			}
		}

		private static void TryMakeSureFatalExceptionsAreRethrown()
		{
			throw new InvalidOperationException();
		}

		[Test]
		public void RethrowWeakExceptionIfNoDebuggerIsAttached()
		{
			try
			{
				TryRethrowWeakExceptionIfNoDebuggerIsAttached();
			} // ncrunch: no coverage
			catch (Exception ex)
			{
				// This will fail if debugger is attached, but that is expected
				Assert.IsTrue(ex.IsWeak());
			}
		}

		private static void TryRethrowWeakExceptionIfNoDebuggerIsAttached()
		{
			throw new ArgumentNullException();
		}

		[Test]
		public void IsDebugMode()
		{
#if DEBUG
			Assert.IsTrue(ExceptionExtensions.IsDebugMode);
#else
			Assert.IsFalse(ExceptionExtensions.IsDebugMode);
#endif
		}
	}
}