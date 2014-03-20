using System;
using System.Diagnostics;
using System.Threading;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Starts a command line process with given argument and optional timeout. Supports events or
	/// can be used synchronously with checking Error or Output afterwards. Exceptions are thrown
	/// when things go bad (ExitCode not 0 or process times out).
	/// </summary>
	public class ProcessRunner
	{
		//ncrunch: no coverage start
		public ProcessRunner(string filePath, string argumentsLine = "", int timeoutInMs = NoTimeout)
		{
			FilePath = filePath;
			ArgumentsLine = argumentsLine;
			this.timeoutInMs = timeoutInMs;
			WorkingDirectory = Environment.CurrentDirectory;
			IsWaitingForExit = true;
			IsExitCodeRelevant = true;
			Output = "";
			Errors = "";
		}

		private const int NoTimeout = -1;

		public string FilePath { get; protected set; }
		public string ArgumentsLine { get; protected set; }
		protected readonly int timeoutInMs;
		public string WorkingDirectory { get; set; }
		public bool IsWaitingForExit { get; set; }
		public bool IsExitCodeRelevant { get; set; }
		public string Output { get; private set; }
		public string Errors { get; private set; }

		public void Start()
		{
			nativeProcess = new Process();
			SetupStartInfo();
			InitializeProcessOutputStreams();
			if (IsWaitingForExit)
				StartNativeProcessAndWaitForExit();
			else
				StartNativeProcess();
		}

		private Process nativeProcess;

		protected void SetupStartInfo()
		{
			nativeProcess.StartInfo.FileName = FilePath;
			nativeProcess.StartInfo.Arguments = ArgumentsLine;
			nativeProcess.StartInfo.WorkingDirectory = WorkingDirectory;
			nativeProcess.StartInfo.CreateNoWindow = true;
			nativeProcess.StartInfo.UseShellExecute = false;
			nativeProcess.StartInfo.RedirectStandardOutput = true;
			nativeProcess.StartInfo.RedirectStandardError = true;
		}

		/// <summary>
		/// Helpful post how to avoid the possible deadlock of a process
		/// http://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why
		/// </summary>
		private void InitializeProcessOutputStreams()
		{
			outputWaitHandle = new AutoResetEvent(false);
			errorWaitHandle = new AutoResetEvent(false);
			nativeProcess.OutputDataReceived += OnStandardOutputDataReceived;
			nativeProcess.ErrorDataReceived += OnErrorOutputDataReceived;
		}

		private AutoResetEvent outputWaitHandle;
		private AutoResetEvent errorWaitHandle;

		private void OnStandardOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (String.IsNullOrEmpty(e.Data))
			{
				outputWaitHandle.Set();
				return;
			}
			if (StandardOutputEvent != null)
				StandardOutputEvent(e.Data);
			Output += e.Data + Environment.NewLine;
		}

		public event Action<string> StandardOutputEvent;

		private void OnErrorOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (String.IsNullOrEmpty(e.Data))
			{
				errorWaitHandle.Set();
				return;
			}
			if (ErrorOutputEvent != null)
				ErrorOutputEvent(e.Data);
			Errors += e.Data + Environment.NewLine;
		}

		public event Action<string> ErrorOutputEvent;

		private void StartNativeProcessAndWaitForExit()
		{
			StartNativeProcess();
			WaitForExit();
			if (IsExitCodeRelevant)
				CheckExitCode();
		}

		private void StartNativeProcess()
		{
			nativeProcess.Start();
			nativeProcess.BeginOutputReadLine();
			nativeProcess.BeginErrorReadLine();
		}

		private void WaitForExit()
		{
			if (!outputWaitHandle.WaitOne(timeoutInMs))
				throw new StandardOutputHasTimedOutException();
			if (!errorWaitHandle.WaitOne(timeoutInMs))
				throw new ErrorOutputHasTimedOutException(); //ncrunch: no coverage
			if (!nativeProcess.WaitForExit(timeoutInMs))
				throw new ProcessHasTimedOutException(); //ncrunch: no coverage
		}

		public class StandardOutputHasTimedOutException : Exception {}
		public class ErrorOutputHasTimedOutException : Exception {}
		public class ProcessHasTimedOutException : Exception {}

		private void CheckExitCode()
		{
			if (nativeProcess.ExitCode != 0)
				throw new ProcessTerminatedWithError(Errors + "\n" + Output); //ncrunch: no coverage
		}

		public class ProcessTerminatedWithError : Exception
		{
			public ProcessTerminatedWithError(string errors)
				: base(errors) {}
		}

		public void Close()
		{
			nativeProcess.Kill();
		}
	}
}