using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DeltaEngine.Core;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Used to start VisualTests from assemblies in an extra AppDomain for the SampleBrowser. Also
	/// used in the ContinuousUpdater to get all tests and start them safely in the Editor.
	/// </summary>
	public class AssemblyStarter : IDisposable
	{
		//ncrunch: no coverage start
		public AssemblyStarter(string assemblyFilePath, bool copyToLocalFolderForExecution)
		{
			if (!File.Exists(assemblyFilePath))
				throw new FileNotFoundException("Assembly not found, unable to start it", assemblyFilePath);
			rememberedDirectory = Directory.GetCurrentDirectory();
			var assemblyFullPath = GetFullPathFromRelativeOrAbsoluteFilePath(assemblyFilePath);
			if (copyToLocalFolderForExecution)
				CopyAssemblyFileAndAllDependencies(assemblyFullPath);
			else if (!string.IsNullOrEmpty(assemblyFullPath))
				Directory.SetCurrentDirectory(assemblyFullPath);
			domain = AppDomain.CreateDomain(DomainName, null, CreateDomainSetup(assemblyFullPath));
			domain.SetData("EntryAssembly", Path.GetFullPath(assemblyFilePath));
		}

		private string GetFullPathFromRelativeOrAbsoluteFilePath(string assemblyFilePath)
		{
			return Path.GetDirectoryName(Path.IsPathRooted(assemblyFilePath)
				? assemblyFilePath : Path.Combine(rememberedDirectory, assemblyFilePath));
		}

		private void CopyAssemblyFileAndAllDependencies(string assemblyDirectory)
		{
			if (rememberedDirectory == assemblyDirectory)
				return;
			var files = Directory.GetFiles(assemblyDirectory);
			foreach (var file in files)
			{
				var filename = Path.GetFileName(file);
				var targetFilePath = Path.Combine(rememberedDirectory, filename);
				if (!File.Exists(targetFilePath) ||
					File.GetLastWriteTime(file) > File.GetLastWriteTime(targetFilePath))
					CopyDependencyFile(file, targetFilePath);
			}
		}

		private static void CopyDependencyFile(string file, string targetFilePath)
		{
			try
			{
				TryCopyDependencyFile(file, targetFilePath, true);
			}
			catch (Exception ex)
			{
				Logger.Warning("Unable to copy newer dependency file (" + file + "), " +
					"file seems to be locked: " + ex);
			}
		}

		private static void TryCopyDependencyFile(string file, string targetFilePath, bool overwrite)
		{
			File.Copy(file, targetFilePath, overwrite);
		}

		[NonSerialized]
		private readonly string rememberedDirectory;
		[NonSerialized]
		private readonly AppDomain domain;
		private const string DomainName = "Delta Engine Assembly Starter";

		private static AppDomainSetup CreateDomainSetup(string assemblyFilePath)
		{
			return new AppDomainSetup
			{
				ApplicationBase = Path.GetFullPath(assemblyFilePath),
				ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
			};
		}

		public void Start(string className, string methodName, object[] parameters = null)
		{
			domain.SetData("EntryClass", className);
			domain.SetData("EntryMethod", methodName);
			domain.SetData("Parameters", parameters);
			try
			{
				TryDoCallBack();
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private void TryDoCallBack()
		{
			domain.DoCallBack(StartEntryPoint);
		}

		private static void StartEntryPoint()
		{
			var assembly = LoadAssembly();
			var className = (string)AppDomain.CurrentDomain.GetData("EntryClass");
			var methodName = (string)AppDomain.CurrentDomain.GetData("EntryMethod");
			var parameters = (object[])AppDomain.CurrentDomain.GetData("Parameters");
			foreach (var type in assembly.GetTypes())
				if (type.Name == className)
					StartMethod(type, methodName, parameters);
		}

		private static Assembly LoadAssembly()
		{
			var assemblyFilePath = (string)AppDomain.CurrentDomain.GetData("EntryAssembly");
			return Assembly.LoadFile(assemblyFilePath);
		}

		private static void StartMethod(Type type, string methodName, object[] parameters)
		{
			var methods = type.GetMethods();
			var instance = Activator.CreateInstance(type);
			StackTraceExtensions.SetUnitTestName(type.FullName + "." + methodName);
			RunMethodWithAttribute(instance, methods, SetUpAttribute);
			methods.FirstOrDefault(method => method.Name == methodName).Invoke(instance, parameters);
			RunMethodWithAttribute(instance, methods, TearDownAttribute);
		}

		private static void RunMethodWithAttribute(object classInstance, MethodInfo[] methods,
			string attributeName)
		{
			RunBaseMethodWithAttribute(classInstance, methods, attributeName);
			RunThisClassMethodWithAttribute(classInstance, methods, attributeName);
		}

		private static void RunBaseMethodWithAttribute(object classInstance, MethodInfo[] methods,
			string attributeName)
		{
			foreach (var method in methods)
				if (method.DeclaringType != classInstance.GetType())
					foreach (var attribute in method.GetCustomAttributes(true))
						if (attribute.GetType().ToString() == attributeName)
							method.Invoke(classInstance, null);
		}

		private static void RunThisClassMethodWithAttribute(object classInstance, MethodInfo[] methods,
			string attributeName)
		{
			foreach (var method in methods)
				if (method.DeclaringType == classInstance.GetType())
					foreach (var attribute in method.GetCustomAttributes(true))
						if (attribute.GetType().ToString() == attributeName)
							method.Invoke(classInstance, null);
		}

		private const string SetUpAttribute = "NUnit.Framework.SetUpAttribute";
		private const string TearDownAttribute = "NUnit.Framework.TearDownAttribute";

		public string[] GetTestNames()
		{
			domain.DoCallBack(FindAllTestNames);
			return (string[])domain.GetData("TestClassAndMethodNames");
		}

		private static void FindAllTestNames()
		{
			var assembly = LoadAssembly();
			var tests = new List<string>();
			foreach (var type in assembly.GetTypes())
				if (type.Name == "Program" || type.Name.EndsWith("Tests"))
					foreach (var method in type.GetMethods())
						AddTestType(tests, method, type);
			AppDomain.CurrentDomain.SetData("TestClassAndMethodNames", tests.ToArray());
		}

		private static void AddTestType(List<string> tests, MethodInfo method, Type type)
		{
			if (method.Name == "Main")
			{
				tests.Insert(0, type.Name + "." + method.Name);
				return;
			}
			if (method.Name == "ToString" || method.Name == "Equals" || method.Name == "GetHashCode" ||
				method.Name == "GetType")
				return;
			foreach (var attribute in method.GetCustomAttributes(true))
				if (attribute.GetType().ToString() == TestAttribute)
				{
					tests.Add(type.Name + "." + method.Name);
					return;
				}
		}

		private const string TestAttribute = "NUnit.Framework.TestAttribute";

		public void Dispose()
		{
			Directory.SetCurrentDirectory(rememberedDirectory);
			AppDomain.Unload(domain);
		}
	}
}