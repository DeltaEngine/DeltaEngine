using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Provides additional check methods on stack traces to find out where we are (e.g. in tests)
	/// </summary>
	public static class StackTraceExtensions
	{
		public static bool StartedFromNCrunchOrNunitConsole
		{
			get
			{
				if (alreadyCheckedIfStartedFromNCrunch)
					return wasStartedFromNCrunchOrNunit;
				alreadyCheckedIfStartedFromNCrunch = true;
				wasStartedFromNCrunchOrNunit = IsStartedFromNCrunch() || IsStartedFromNunitConsole();
				return wasStartedFromNCrunchOrNunit;
			}
		}

		private static bool alreadyCheckedIfStartedFromNCrunch;
		private static bool wasStartedFromNCrunchOrNunit;

		/// <summary>
		/// See http://www.ncrunch.net/documentation/troubleshooting_ncrunch-specific-overrides
		/// </summary>
		public static bool IsStartedFromNCrunch()
		{
			return Environment.GetEnvironmentVariable("NCrunch") == "1";
		}

		public static bool StartedFromNUnitConsoleButNotFromNCrunch
		{
			get { return StartedFromNCrunchOrNunitConsole && !IsStartedFromNCrunch(); }
		}

		/// <summary>
		/// When we don't know the attribute type we cannot use Attribute.IsAttribute. Use this instead.
		/// </summary>
		public static bool HasAttribute(this StackFrame frame, string name)
		{
			object[] attributes = frame.GetMethod().GetCustomAttributes(false);
			return attributes.Any(attribute => attribute.GetType().ToString() == name);
		}

		private const string TestAttribute = "NUnit.Framework.TestAttribute";
		private const string ApproveFirstFrameScreenshotAttribute =
			"DeltaEngine.Platforms.ApproveFirstFrameScreenshotAttribute";
		private const string CloseAfterFirstFrameAttribute =
			"DeltaEngine.Platforms.CloseAfterFirstFrameAttribute";

		public static bool IsUnitTest()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			return frames.Any(frame => IsTestAttribute(frame) || IsInTestSetUp(frame));
		}

		private static bool IsTestAttribute(StackFrame frame)
		{
			return frame.HasAttribute(TestAttribute);
		}

		private static bool IsInTestSetUp(StackFrame frame)
		{
			return frame.HasAttribute(SetUpAttribute) || frame.HasAttribute(FixtureSetUpAttribute);
		}

		private const string SetUpAttribute = "NUnit.Framework.SetUpAttribute";
		private const string FixtureSetUpAttribute = "NUnit.Framework.TestFixtureSetUpAttribute";

		/// <summary>
		/// Get entry name from stack frame, which is either the namespace name where the main method
		/// is located or if we are started from a test, the name of the test method.
		/// </summary>
		public static string GetEntryName()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			var testName = GetTestMethodName(frames);
			if (!string.IsNullOrEmpty(testName))
				return testName;
			foreach (StackFrame frame in frames.Where(frame => frame.GetMethod().Name == "Main"))
				return GetNamespaceName(frame);
			return "Delta Engine"; //ncrunch: no coverage
		}

		public static string GetTestMethodName(this IEnumerable<StackFrame> frames)
		{
			foreach (StackFrame frame in frames)
			{
				if (IsTestAttribute(frame))
					return frame.GetMethod().Name;
				if (!String.IsNullOrEmpty(unitTestMethodName) && IsInTestSetUp(frame))
					return unitTestMethodName;
			}
			return string.Empty;
		}

		public static string GetExecutingAssemblyName()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			if (!String.IsNullOrEmpty(unitTestClassFullName))
				return GetNamespaceNameFromClassName(unitTestClassFullName);
			//ncrunch: no coverage start (these lines can only be reached from production code)
			foreach (StackFrame frame in frames.Where(frame => frame.GetMethod().Name == "Main"))
				return GetNamespaceName(frame);
			foreach (StackFrame frame in frames.Where(IsTestOrTestSetupMethod))
				return frame.GetMethod().DeclaringType.Assembly.GetName().Name;
			if (IsRunningAsWindowsService(frames))
				return GetNamespaceNameForWindowsService(frames.ToList());
			throw new ExecutingAssemblyOrNamespaceNotFound();		
		}

		private static bool IsTestOrTestSetupMethod(StackFrame frame)
		{
			return IsTestAttribute(frame) || IsInTestSetUp(frame);
		} //ncrunch: no coverage end

		private static string GetNamespaceNameFromClassName(string fullClassName)
		{
			var result = Path.GetFileNameWithoutExtension(fullClassName);
			while (result.Contains(".Tests."))
				result = Path.GetFileNameWithoutExtension(result); //ncrunch: no coverage
			return result;
		}

		private class ExecutingAssemblyOrNamespaceNotFound : Exception {}

		private static string GetNamespaceName(StackFrame frame)
		{
			var classType = frame.GetMethod().DeclaringType;
			return classType != null ? classType.Namespace : "";
		}

		public static string GetClassName(this IEnumerable<StackFrame> frames)
		{
			foreach (StackFrame frame in frames.Where(frame => IsTestAttribute(frame)))
				return frame.GetMethod().DeclaringType.Name;
			return string.Empty;
		}

		//ncrunch: no coverage start (these lines can only be reached from production code)
		private static bool IsRunningAsWindowsService(IEnumerable<StackFrame> frames)
		{
			return frames.Any(frame => frame.GetMethod().Name == "ServiceQueuedMainCallback");
		}

		private static string GetNamespaceNameForWindowsService(List<StackFrame> frames)
		{
			var index = Array.FindIndex(frames.ToArray(),
				frame => frame.GetMethod().Name == "ServiceQueuedMainCallback");
			return frames[index - 1].GetMethod().DeclaringType.Namespace;
		}

		public static string GetApprovalTestName()
		{
			var frames = new StackTrace().GetFrames();
			foreach (var frame in frames)
			{
				if (IsTestAttribute(frame) && frame.HasAttribute(ApproveFirstFrameScreenshotAttribute))
					return frames.GetClassName() + "." + frames.GetTestMethodName();
				if (!String.IsNullOrEmpty(unitTestMethodName) && IsInTestSetUp(frame) &&
					HasRunningTestAttribute(ApproveFirstFrameScreenshotAttribute))
					return unitTestClassName + "." + unitTestMethodName;
			}
			return "";
		}

		private static bool HasRunningTestAttribute(string attributeFullName)
		{
			var testClassType = GetClassTypeFromRunningAssemblies(unitTestClassFullName);
			if (testClassType == null)
				return false;
			var method = testClassType.GetMethod(unitTestMethodName);
			if (method == null)
				return false;
			object[] attributes = method.GetCustomAttributes(false);
			return attributes.Any(a => a.GetType().ToString() == attributeFullName);
		}

		private static Type GetClassTypeFromRunningAssemblies(string classFullName)
		{
			var runningAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var assembly in runningAssemblies)
				if (classFullName.StartsWith(assembly.GetName().Name))
					return assembly.GetType(classFullName);
			return null;
		}

		private static bool IsStartedFromNunitConsole()
		{
			string domainName = AppDomain.CurrentDomain.FriendlyName;
			return domainName.StartsWith("test-domain-") || domainName == "NUnit Domain";
		}

		/// <summary>
		/// Set by TeamCity to run Tests for CI builds with Mocks because it's faster
		/// </summary>
		public static bool ForceUseOfMockResolver()
		{
			return IsStartedFromNCrunch() ||
				Environment.GetEnvironmentVariable("RunAllTestsWithMocks") == "1";
		}

		/// <summary>
		/// Since we do not initialize or run the resolver in a test, we need to set the current unit
		/// test name up beforehand so we can find out if the test uses ApproveFirstFrameScreenshot.
		/// </summary>
		public static void SetUnitTestName(string fullName)
		{
			var nameParts = fullName.Split(new[] { '.' });
			unitTestMethodName = nameParts[nameParts.Length - 1];
			unitTestClassName = nameParts[nameParts.Length - 2];
			unitTestClassFullName = nameParts[0];
			for (int num = 1; num < nameParts.Length - 1; num++)
				unitTestClassFullName += "." + nameParts[num];
		}

		public static bool IsCloseAfterFirstFrameAttributeUsed()
		{
			var frames = new StackTrace().GetFrames();
			foreach (var frame in frames)
			{
				if ((IsTestAttribute(frame) || IsInTestSetUp(frame)) &&
					frame.HasAttribute(CloseAfterFirstFrameAttribute))
					return true;
				if (!String.IsNullOrEmpty(unitTestMethodName) && IsInTestSetUp(frame))
					return HasRunningTestAttribute(CloseAfterFirstFrameAttribute);
			}
			return false;
		}

		private static string unitTestClassName;
		private static string unitTestMethodName;
		private static string unitTestClassFullName;

		public static bool StartedFromProgramMain
		{
			get
			{
				if (StartedFromNCrunchOrNunitConsole)
					return false;
				return new StackTrace().GetFrames().Any(IsMethodMainOrStart);
			}
		}

		private static bool IsMethodMainOrStart(StackFrame frame)
		{
			var methodName = frame.GetMethod().Name;
			return methodName == "Main" || methodName == "StartEntryPoint";
		}

		/// <summary>
		/// Shows the callstack as multiline text output to help figure out who called what. Removes the
		/// first callstack line (this method) and all non-helpful System, NUnit and nCrunch lines.
		/// </summary>
		public static string FormatStackTraceIntoClickableMultilineText(int stackFramesToSkip = 0)
		{
			string output = "";
			foreach (var frame in new StackTrace(true).GetFrames().Skip(1 + stackFramesToSkip))
				if (!IsSystemOrTestMethodToExclude(frame.GetMethod()))
					output += "   at " + GetMethodWithParameters(frame.GetMethod()) +
						GetFilenameAndLineInfo(frame) + "\n";
			return output;
		}

		private static string GetFilenameAndLineInfo(StackFrame frame)
		{
			string filename = frame.GetFileName();
			int lineNumber = frame.GetFileLineNumber();
			if (string.IsNullOrEmpty(filename) || lineNumber == 0)
				return "";
			return " in " + filename + ":line " + lineNumber;
		}

		private static string GetMethodWithParameters(MethodBase method)
		{
			return method.DeclaringType + "." + method.Name + "(" + GetParameters(method) + ")";
		}

		private static string GetParameters(MethodBase method)
		{
			string parametersText = "";
			foreach (var parameter in method.GetParameters())
				parametersText += (parametersText.Length > 0 ? ", " : "") + parameter.ParameterType + " " +
					parameter.Name;
			return parametersText;
		}

		private static bool IsSystemOrTestMethodToExclude(MethodBase method)
		{
			return method.DeclaringType.FullName.StartsWith(MethodNamesToExclude) ||
				method.DeclaringType.Assembly.GetName().Name.StartsWith("nCrunch");
		}

		private static readonly string[] MethodNamesToExclude =
		{
			"System.RuntimeMethodHandle", "System.Reflection", "TestDriven", "System.Threading",
			"System.AppDomain", "System.Activator", "System.Runtime", "Delta.Utilities.Testing",
			"Microsoft.VisualStudio.HostingProcess", "System.Windows.", "System.Net.", "MS.Win32.",
			"MS.Internal.", "NUnit.Core.", "xUnit.", "JetBrains.ReSharper.", "nCrunch.", "Autofac."
		};

		public static string FormatExceptionIntoClickableMultilineText(Exception exception)
		{
			string[] messageLines = exception.ToString().SplitAndTrim('\n');
			var output = "";
			foreach (string line in messageLines)
			{
				if (line.Trim().StartsWith("at ") || line.Trim().StartsWith("bei ") ||
					line.Contains(" DeltaEngine."))
				{
					string trimmedLine = line.Trim();
					var removeFirstWord = line.Substring(trimmedLine.IndexOf(' ')).TrimStart();
					bool skipLine = false;
					foreach (var nameToExclude in MethodNamesToExclude)
						if (removeFirstWord.StartsWith(nameToExclude))
						{
							skipLine = true;
							break;
						}
					if (!skipLine)
						output += (output.Length > 0 ? "\n" : "") + "   " + trimmedLine;
				}
				else
					output += (output.Length > 0 ? "\n" : "") + line;
			}
			return output;
		}
	}
}