using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class ConsoleCommandManagerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			Resolve<TestCommand>();
			consoleCommands = ConsoleCommands.Current;
		}

		private ConsoleCommands consoleCommands;

		[Test]
		public void ExecuteUnknownCommand()
		{
			string result = consoleCommands.ExecuteCommand("NotRegistered");
			Assert.AreEqual("Error: Unknown console command 'NotRegistered'", result);
		}

		[Test]
		public void ExecuteCommand()
		{
			Assert.AreEqual("Result: '3'", consoleCommands.ExecuteCommand("AddInts 1 2"));
			Assert.AreEqual("Result: '3.5'", consoleCommands.ExecuteCommand("AddFloats 1.2 2.3"));
		}

		[Test]
		public void ExecuteEmptyCommand()
		{
			string result = consoleCommands.ExecuteCommand("");
			Assert.AreEqual("", result);
		}

		[Test]
		public void ExecuteCommandWithWrongNumberOfParameters()
		{
			string result = consoleCommands.ExecuteCommand("AddFloats 1");
			Assert.AreEqual("Error: The command has 2 parameters, but you entered 1", result);
		}

		[Test]
		public void ExecuteCommandWithAnInvalidParameter()
		{
			string result = consoleCommands.ExecuteCommand("AddFloats 1 a");
			const string ExpectedEnglish =
				"Error: Can't process parameter no. 2: 'Input string was not in a correct format.'";
			const string ExpectedGerman =
				"Error: Can't process parameter no. 2: 'Die Eingabezeichenfolge hat das falsche Format.'";
			Assert.IsTrue(ExpectedEnglish.Equals(result) || ExpectedGerman.Equals(result));
		}

		[Test]
		public void ExecuteCommandThatThrowsException()
		{
			string result = consoleCommands.ExecuteCommand("ThrowsException");
			Assert.AreEqual(
				"Error: Exception while invoking the command: " +
					"'" + new TargetInvocationException(null).Message + "'", result);
		}

		[Test]
		public void GetAutoCompletionListFromTestCommandMethods()
		{
			List<string> autoCompletions = consoleCommands.GetAutoCompletionList("add");
			Assert.AreEqual(2, autoCompletions.Count);
			Assert.AreEqual("AddFloats Single Single", autoCompletions[0]);
			Assert.AreEqual("AddInts Int32 Int32", autoCompletions[1]);
		}

		[Test]
		public void GetAutoCompletionListFromGlobalTimeMethod()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			List<string> autoCompletions =
				consoleCommands.GetAutoCompletionList("GetSecondsSinceStart");
			Assert.AreEqual(1, autoCompletions.Count);
			Assert.AreEqual("GetSecondsSinceStartToday", autoCompletions[0]);
		}

		[Test]
		public void AutoCompletingNonMatchingStringReturnsInput()
		{
			Assert.AreEqual("z", consoleCommands.AutoCompleteString("z"));
		}

		[Test]
		public void AutoCompletingAmbiguousStringReturnsMatchingPartOfInput()
		{
			Assert.AreEqual("Add", consoleCommands.AutoCompleteString("add"));
		}

		[Test]
		public void AutoCompletingUnambiguousStringReturnsMethod()
		{
			Assert.AreEqual("AddFloats", consoleCommands.AutoCompleteString("addf"));
		}
	}
}