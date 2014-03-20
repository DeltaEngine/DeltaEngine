using System;
using System.Diagnostics;
using System.Reflection;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Categorizes exceptions into fatal and weak ones. Fatal exceptions are always rethrown and weak
	/// ones (most likely simple programming mistakes) can be logged and ignored if no debugger is
	/// attached. See http://vasters.com/clemensv/2012/09/06/Are+You+Catching+Falling+Knives.aspx
	/// </summary>
	public static class ExceptionExtensions
	{
		public static bool IsFatal(this Exception anyException)
		{
			return IsFatalException(anyException) || IsFatalException(anyException.InnerException);
		}

		private static bool IsFatalException(Exception ex)
		{
			return ex is InvalidOperationException || ex is OutOfMemoryException ||
				ex is AccessViolationException || ex is StackOverflowException ||
				ex is TargetInvocationException;
		}

		public static bool IsWeak(this Exception anyException)
		{
			return Debugger.IsAttached == false &&
				(IsWeakException(anyException) || IsWeakException(anyException.InnerException));
		}

		private static bool IsWeakException(Exception ex)
		{
			return ex is ArgumentNullException || ex is NullReferenceException ||
				ex is ArgumentException || ex is IndexOutOfRangeException;
		}

		public static bool IsDebugMode
		{
#if DEBUG
			get { return true; }
#else
			get { return false; }
#endif
		}
	}
}