using System;
using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Content.Xml.Tests
{
	public class InputCommandsTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void TestInputCommands()
		{
			var inputCommands = ContentLoader.Load<InputCommands>("DefaultCommands");
			Assert.AreEqual("DefaultCommands", inputCommands.Name);
			Assert.IsTrue(inputCommands.InternalAllowCreationIfContentNotFound);
		}

		[Test, CloseAfterFirstFrame]
		public void LogErrorIfTriggerTypeDoesNotExist()
		{
			var logger = new MockLogger();
			ContentLoader.Load<NoDataInputCommands>("NoDataInputCommands").InternalCreateDefault();
			Assert.IsTrue(logger.LastMessage.Contains(NonTriggerTypeName), logger.LastMessage);
			Assert.IsTrue(logger.LastMessage.Contains("MissingMethodException"), logger.LastMessage);
			logger.Dispose();
		}

		private const string NonTriggerTypeName = "Int32";

		private class NoDataInputCommands : InputCommands
		{
			protected NoDataInputCommands(string contentName)
				: base(contentName) {}

			protected override bool AllowCreationIfContentNotFound
			{
				get { return true; }
			}

			protected override void CreateDefault()
			{
				var click = new XmlData("Command");
				click.AddChild(NonTriggerTypeName, "");
				Command.Register(Command.Click, ParseTriggers(click));
			}
		}

		[Test, CloseAfterFirstFrame, Timeout(5000)]
		public void CreateDefaultInputCommandsIfContentNotFound()
		{
			var inputCommands = ContentLoader.Load<NotExistingInputCommands>("NotExistingInputCommands");
			inputCommands.InternalCreateDefault();
			var exitCommand = new Command(Command.Exit, (Action)null);
			List<Trigger> triggers = exitCommand.GetTriggers();
			Assert.AreEqual(1, triggers.Count);
			Assert.AreEqual("KeyTrigger", triggers[0].GetShortNameOrFullNameIfNotFound());
		}

		private class NotExistingInputCommands : InputCommands
		{
			protected NotExistingInputCommands(string contentName)
				: base(contentName) {}

			//ncrunch: no coverage start
			protected override bool AllowCreationIfContentNotFound
			{
				get { return Name == "NotExistingInputCommands"; }
			}
		}
	}
}