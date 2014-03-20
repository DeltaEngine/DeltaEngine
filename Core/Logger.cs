using System;
using System.Collections.Generic;
using DeltaEngine.Extensions;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Allows any problem or issue to be logged via messages or exceptions, usually used by the
	/// MockLogger for tests and ConsoleLogger, TextFileLogger and NetworkLogger in apps.
	/// </summary>
	public abstract class Logger : IDisposable
	{
		private static readonly ThreadStatic<List<Logger>> RegisteredLoggers =
			new ThreadStatic<List<Logger>>();

		protected Logger(bool registerToAllThreads = false)
		{
			LastMessage = "";
			if (registerToAllThreads)
				RegisterToAllThreads();
			else
				RegisterToCurrentThread();
		}

		private void RegisterToAllThreads()
		{
			var thisType = GetType();
			foreach (Logger logger in CurrentLoggersInAllThreads)
				if (logger.GetType() == thisType)
				{
					if (thisType.Name.StartsWith("Console"))
						RemoveConsoleLoggerFromPreviousFailingTest();
					else
						throw new LoggerWasAlreadyAttached();
					break;
				}
			CurrentLoggersInAllThreads.Add(this);
		}

		private void RemoveConsoleLoggerFromPreviousFailingTest()
		{
			foreach (Logger logger in CurrentLoggersInAllThreads)
				if (logger.GetType() == GetType())
				{
					CurrentLoggersInAllThreads.Remove(logger);
					break;
				}
		}

		private void RegisterToCurrentThread()
		{
			if (!RegisteredLoggers.HasCurrent)
				RegisteredLoggers.Use(new List<Logger>());
			var thisType = GetType();
			foreach (Logger logger in RegisteredLoggers.Current)
				if (logger.GetType() == thisType)
				{
					if (thisType.Name.StartsWith("Mock") || thisType.Name.StartsWith("TextFile"))
						RemoveExistingLoggerFromPreviousFailingTest();
					else
						throw new LoggerWasAlreadyAttached();
					break;
				}
			RegisteredLoggers.Current.Add(this);
		}

		private void RemoveExistingLoggerFromPreviousFailingTest()
		{
			foreach (Logger logger in RegisteredLoggers.Current)
				if (logger.GetType() == GetType())
				{
					RegisteredLoggers.Current.Remove(logger);
					break;
				}
		}

		private static readonly List<Logger> CurrentLoggersInAllThreads = new List<Logger>();

		internal static int TotalNumberOfAttachedLoggers
		{
			get
			{
				return CurrentLoggersInAllThreads.Count +
					(RegisteredLoggers.HasCurrent ? RegisteredLoggers.Current.Count : 0);
			}
		}

		public class LoggerWasAlreadyAttached : Exception {}

		/// <summary>
		/// Lowest available log level for notifications like a successful operation or debug output.
		/// </summary>
		public static void Info(string message)
		{
			WarnIfNoLoggersAreAttached(message);
			foreach (var logger in CurrentLoggers)
				logger.WriteMessage(MessageType.Info, message);
		}

		private static List<Logger> CurrentLoggers
		{
			get
			{
				var total = new List<Logger>(CurrentLoggersInAllThreads);
				if (RegisteredLoggers.HasCurrent)
					total.AddRange(RegisteredLoggers.Current);
				return total;
			}
		}

		private void WriteMessage(MessageType type, string message)
		{
			if (message != LastMessage)
				Write(type, message);
			else
				NumberOfRepeatedMessagesIgnored++;
			LastMessage = message;
		}

		public string LastMessage { get; protected set; }
		public int NumberOfRepeatedMessagesIgnored { get; private set; }

		private static void WarnIfNoLoggersAreAttached(string message)
		{
			if (CurrentLoggers.Count == 0)
				Console.WriteLine("No loggers have been created for this message: " + message);
		}

		public abstract void Write(MessageType messageType, string message);

		public enum MessageType
		{
			Info,
			Warning,
			Error
		}

		/// <summary>
		/// If something bad happened but we can continue this type of message is logged.
		/// </summary>
		public static void Warning(string message)
		{
			WarnIfNoLoggersAreAttached(message);
			foreach (var logger in CurrentLoggers)
				logger.WriteMessage(MessageType.Warning, message);
		}

		/// <summary>
		/// If something bad happened and we caught a non fatal exception this message is logged.
		/// </summary>
		public static void Warning(Exception exception)
		{
			WarnIfNoLoggersAreAttached(exception.ToString());
			foreach (var logger in CurrentLoggers)
				logger.WriteMessage(MessageType.Warning, exception.ToString());
		}

		/// <summary>
		/// If a fatal exception happened this message is logged. Often the App has to quit afterward.
		/// </summary>
		public static void Error(Exception exception)
		{
			var exceptionText = StackTraceExtensions.FormatExceptionIntoClickableMultilineText(exception);
			WarnIfNoLoggersAreAttached(exceptionText);
			foreach (var logger in CurrentLoggers)
				logger.WriteMessage(MessageType.Error, exceptionText);
		}

		protected string CreateMessageTypePrefix(MessageType messageType)
		{
			return DateTime.Now.ToString("T") + " " +
				(messageType == MessageType.Info ? "" : messageType + ": ");
		}

		public virtual void Dispose()
		{
			if (RegisteredLoggers.HasCurrent)
				RegisteredLoggers.Current.Remove(this);
			CurrentLoggersInAllThreads.Remove(this);
		}
	}
}
