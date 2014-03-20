using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Evaluates command strings and executes methods at run time on demand (triggers, console).
	/// </summary>
	public class ConsoleCommands
	{
		public static ConsoleCommands Current
		{
			get
			{
				if (threadStaticConsoleCommands == null)
					threadStaticConsoleCommands = new ThreadStatic<ConsoleCommands>();
				if (!threadStaticConsoleCommands.HasCurrent)
					threadStaticConsoleCommands.Use(new ConsoleCommands());
				return threadStaticConsoleCommands.Current;
			}	
		}

		internal static ConsoleCommandResolver resolver;
		private static ThreadStatic<ConsoleCommands> threadStaticConsoleCommands;

		private ConsoleCommands()
		{
			RegisterCommandsFromTypes();
		}

		private void RegisterCommandsFromTypes()
		{
			foreach (Type type in new List<Type>(resolver.RegisteredTypes))
			{
				var allMethods = 
					type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				var consoleMethods = allMethods.Where(method => 
					method.GetCustomAttributes(typeof(ConsoleCommandAttribute), true).Any()).ToList();
				RegisterCommandsForType(type, consoleMethods);
			}
		}

		private void RegisterCommandsForType(Type type, List<MethodInfo> methods)
		{
			if (methods.Count == 0)
				return;
			var instance = resolver.Resolve(type);
			foreach (MethodInfo method in methods)
				AddCommand(method, instance);
		}

		private void AddCommand(MethodInfo method, object target)
		{
			string delegateKey = method + " + " + target;
			if (delegatesAlreadyCreated.Contains(delegateKey))
				return;
			delegates.Add(CreateDelegate(method, target));
			delegatesAlreadyCreated.Add(delegateKey);
		}

		private readonly List<string> delegatesAlreadyCreated = new List<string>();
		private readonly List<Delegate> delegates = new List<Delegate>();

		private static string GetMethodName(MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(typeof(ConsoleCommandAttribute), true);
			var attribute = attributes.FirstOrDefault() as ConsoleCommandAttribute;
			return attribute.Name;
		}

		private static Delegate CreateDelegate(MethodInfo method, object target)
		{
			var parameters = method.GetParameters();
			var parameterTypes = parameters.Select(p => p.ParameterType).ToList();
			parameterTypes.Add(method.ReturnType);
			Type delegateType = Expression.GetDelegateType(parameterTypes.ToArray());
			return Delegate.CreateDelegate(delegateType, target, method);
		}

		public string ExecuteCommand(string command)
		{
			var commandAndParameters = new List<string>(command.SplitAndTrim(' '));
			if (commandAndParameters.Count == 0)
				return "";
			var method = delegates.FirstOrDefault(d => GetMethodName(d.Method).Equals(commandAndParameters[0],
				StringComparison.OrdinalIgnoreCase));
			if (method == null)
				return "Error: Unknown console command '" + commandAndParameters[0] + "'";
			commandAndParameters.RemoveAt(0);
			return ExecuteMethod(commandAndParameters, method);
		}

		private static string ExecuteMethod(List<string> commandParameters, Delegate method)
		{
			var methodParameters = method.Method.GetParameters();
			if (methodParameters.Length == commandParameters.Count)
				return methodParameters.Length == 0
					? InvokeDelegate(method, null)
					: ExecuteMethodWithParameters(commandParameters, method, methodParameters);
			string plural = methodParameters.Length == 1 ? "" : "s";
			return "Error: The command has " + methodParameters.Length + " parameter" + plural +
				", but you entered " + commandParameters.Count;
		}

		private static string InvokeDelegate(Delegate method, params object[] parameters)
		{
			try
			{
				return TryInvokeDelegate(method, parameters);
			}
			catch (Exception ex)
			{
				return "Error: Exception while invoking the command: '" + ex.Message + "'";
			}
		}

		private static string TryInvokeDelegate(Delegate method, params object[] parameters)
		{
			var result = Convert.ToString(method.DynamicInvoke(parameters), CultureInfo.InvariantCulture);
			return string.IsNullOrWhiteSpace(result) ? "Command executed" : "Result: '" + result + "'";
		}

		private static string ExecuteMethodWithParameters(List<string> commandParameters,
			Delegate method, ParameterInfo[] parameter)
		{
			var paramList = new List<object>();
			for (int i = 0; i < parameter.Length; i++)
				try
				{
					paramList.Add(TryConvertParameter(commandParameters[i], parameter[i]));
				}
				catch (Exception ex)
				{
					return "Error: Can't process parameter no. " + (i + 1) + ": '" + ex.Message + "'";
				}
			return InvokeDelegate(method, paramList.ToArray());
		}

		private static object TryConvertParameter(string cmdParameter, ParameterInfo parameter)
		{
			return Convert.ChangeType(cmdParameter, parameter.ParameterType, CultureInfo.InvariantCulture);
		}

		public List<string> GetAutoCompletionList(string input)
		{
			var descriptionList = GetMatchingDelegates(input).Select(GetDescription);
			var orderedList = descriptionList.OrderBy(x => x);
			return orderedList.ToList();
		}

		private IEnumerable<MethodInfo> GetMatchingDelegates(string input)
		{
			var matchingDelegates = new List<MethodInfo>();
			foreach (var currentDelegate in delegates)
			{
				MethodInfo method = currentDelegate.Method;
				if (GetMethodName(method).StartsWith(input, true, null))
					matchingDelegates.Add(method);
			}
			return matchingDelegates;
		}

		private static string GetDescription(MethodInfo info)
		{
			string description = GetMethodName(info);
			IEnumerable<string> parameters = info.GetParameters().Select(x => x.ParameterType.Name);
			// ReSharper disable PossibleMultipleEnumeration
			if (parameters.Any())
				description += " " + parameters.Aggregate((x, y) => x + " " + y);
			return description;
		}

		public string AutoCompleteString(string input)
		{
			IEnumerable<MethodInfo> matchingDelegates = GetMatchingDelegates(input);
			IEnumerable<string> names = matchingDelegates.Select(x => GetMethodName(x));
			return !names.Any() ? input : GetShortestString(names);
		}

		private static string GetShortestString(IEnumerable<string> names)
		{
			var nameLengths = names.Select(n => n.Length);
			int minLength = nameLengths.Min();
			string shortestString = names.First(x => x.Length == minLength);
			while (!names.All(s => s.StartsWith(shortestString)))
				shortestString = shortestString.Substring(0, shortestString.Length - 1);
			return shortestString;
		}
	}
}