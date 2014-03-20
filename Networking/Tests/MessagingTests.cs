using System.Collections.Generic;
using System.Threading;
using DeltaEngine.Extensions;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	public class MessagingTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			serverSession = Messaging.StartSession(Port);
			Pause();
			clientSession = Messaging.JoinSession(Address, Port);
			Pause();
		}

		private MessagingSession serverSession;
		private MessagingSession clientSession;
		private const string Address = "127.0.0.1";
		private const int Port = 6578;

		private static void Pause()
		{
			if (!StackTraceExtensions.IsStartedFromNCrunch())
				Thread.Sleep(25); //ncrunch: no coverage
		}

		[TearDown]
		public void DisposeConnectionsSockets()
		{
			clientSession.Dispose();
			serverSession.Dispose();
		}

		[Test, CloseAfterFirstFrame]
		public void UniqueIDsAssigned()
		{
			Assert.AreEqual(0, serverSession.UniqueID);
			Assert.AreEqual(1, clientSession.UniqueID);
		}

		[Test, CloseAfterFirstFrame]
		public void ServerReportsOneParticipant()
		{
			Assert.AreEqual(1, serverSession.NumberOfParticipants);
		}

		[Test, CloseAfterFirstFrame]
		public void ClientReportsParticipantsNotCalculated()
		{
			Assert.AreEqual(ClientMessagingSession.NumberOfParticipantsNotCalculated,
				clientSession.NumberOfParticipants);
		}

		[Test, CloseAfterFirstFrame]
		public void NoMessagesInitially()
		{
			List<MessagingSession.Message> messages = serverSession.GetMessages();
			Assert.AreEqual(0, messages.Count);
			Assert.AreEqual(0, clientSession.GetMessages().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void SendMessagesServerToClient()
		{
			serverSession.SendMessage("hello");
			serverSession.SendMessage(9);
			Pause();
			List<MessagingSession.Message> messages = clientSession.GetMessages();
			Assert.AreEqual(2, messages.Count);
			VerifyMessageContents(messages[0], 0, "hello");
			VerifyMessageContents(messages[1], 0, 9);
		}

		private static void VerifyMessageContents(MessagingSession.Message message, int uniqueID,
			object data)
		{
			Assert.AreEqual(uniqueID, message.SenderUniqueID);
			Assert.AreEqual(data, message.Data);
		}

		[Test, CloseAfterFirstFrame]
		public void MessagesAreOnlyReceivedOnce()
		{
			serverSession.SendMessage("hello");
			clientSession.SendMessage("hello");
			Pause();
			Assert.AreEqual(1, serverSession.GetMessages().Count);
			Assert.AreEqual(0, serverSession.GetMessages().Count);
			Assert.AreEqual(1, clientSession.GetMessages().Count);
			Assert.AreEqual(0, clientSession.GetMessages().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void SendMessagesClientToServer()
		{
			clientSession.SendMessage("hi");
			clientSession.SendMessage(1.2f);
			Pause();
			List<MessagingSession.Message> messages = serverSession.GetMessages();
			Assert.AreEqual(2, messages.Count);
			VerifyMessageContents(messages[0], 1, "hi");
			VerifyMessageContents(messages[1], 1, 1.2f);
		}

		[Test, CloseAfterFirstFrame]
		public void WithTwoClientsWhenOneClientMessagesTheServerItIsEchoedToTheOtherClient()
		{
			if (!StackTraceExtensions.IsStartedFromNCrunch())
				return; //ncrunch: no coverage
			var clientSession2 = Messaging.JoinSession(Address, Port);
			clientSession2.SendMessage("welcome");
			List<MessagingSession.Message> messages = clientSession.GetMessages();
			Assert.AreEqual(1, serverSession.GetMessages().Count);
			Assert.AreEqual(1, messages.Count);
			VerifyMessageContents(messages[0], 2, "welcome");
			Assert.AreEqual(2, serverSession.NumberOfParticipants);
		}

		[Test, CloseAfterFirstFrame]
		public void DisconnectedClientNoLongerReceivesMessagesFromServer()
		{
			clientSession.Disconnect();
			Pause();
			serverSession.SendMessage("hello?");
			Pause();
			Assert.AreEqual(0, clientSession.GetMessages().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void DisconnectedServerNoLongerReceivesMessagesFromClients()
		{
			serverSession.Disconnect();
			Pause();
			clientSession.SendMessage("are you there?");
			Pause();
			Assert.AreEqual(0, serverSession.GetMessages().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void WhenTwoServersExistMessagesAreSentToTheCorrectOne()
		{
			if (!StackTraceExtensions.IsStartedFromNCrunch())
				return; //ncrunch: no coverage
			MessagingSession serverSession2 = Messaging.StartSession(Port + 1);
			MessagingSession clientSession2 = Messaging.JoinSession(Address, Port + 1);
			clientSession.SendMessage("first");
			clientSession2.SendMessage("second");
			Assert.AreEqual("first", serverSession.GetMessages()[0].Data);
			Assert.AreEqual("second", serverSession2.GetMessages()[0].Data);
		}
	}
}