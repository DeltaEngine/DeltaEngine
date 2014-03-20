using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Logging
{
	/// <summary>
	/// Writes into a log text file and opens it in a editor after execution.
	/// </summary>
	public class TextFileLogger : Logger
	{
		public TextFileLogger()
			: base(!StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
		{
			filePath = Path.Combine(Settings.GetMyDocumentsAppFolder(), LogFilename);
		}

		private readonly string filePath;
		private const string LogFilename = "Log.txt";

		public override void Write(MessageType messageType, string message)
		{
			OpenOrCreate().WriteLine(CreateMessageTypePrefix(messageType) + message);
		}

		private StreamWriter OpenOrCreate()
		{
			if (writer != null)
				return writer;
			var logFile = new FileStream(filePath, FileMode.Create, FileAccess.Write,
				FileShare.ReadWrite);
			writer = new StreamWriter(logFile) { AutoFlush = true };
			writer.WriteLine(AssemblyExtensions.GetTestNameOrProjectName() + " Log " +
				DateTime.Now.GetIsoDateTime());
			return writer;
		}

		private StreamWriter writer;

		public override void Dispose()
		{
			base.Dispose();
			if (writer == null)
				return;
			if (NumberOfRepeatedMessagesIgnored > 0)
				writer.WriteLine("NumberOfRepeatedMessagesIgnored=" + NumberOfRepeatedMessagesIgnored);
			writer.Close();
			writer = null;
			if (ExceptionExtensions.IsDebugMode && !StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
				OpenLogFileInEditor(); //ncrunch: no coverage
		}

		private void OpenLogFileInEditor()
		{
			try
			{
				TryOpenLogFileInEditor();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to open engine log file in text editor: " + ex.Message);
			}
		}

		private void TryOpenLogFileInEditor()
		{
			Process.Start(filePath);
		}
	}
}