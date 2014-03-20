using System;

namespace DeltaEngine.Networking.Tcp
{
	internal class DataCollector
	{
		public void ReadBytes(byte[] data, int startOffset, int numberOfBytesToRead)
		{
			availableData = data;
			offset = startOffset;
			numberOfBytesAvailable = numberOfBytesToRead;
			ReadBytes();
		}

		private byte[] availableData;
		private int offset;
		private int numberOfBytesAvailable;

		private void ReadBytes()
		{
			if (currentContainerToFill == null)
			{
				int messageLength = GetMessageLength();
				if (messageLength == NotEnoughBytesForMessageLength)
					return;
				currentContainerToFill = new MessageData(messageLength);
			}
			if (numberOfBytesAvailable > 0)
				ReadDataToEnd();
		}

		private MessageData currentContainerToFill;

		private int GetMessageLength()
		{
			if (rememberMessageBytesFromLastTime != null)
				return ReadLength();
			if (numberOfBytesAvailable < 1)
				return NotEnoughBytesForMessageLength;
			int messageLength = availableData[offset];
			offset++;
			numberOfBytesAvailable--;
			if (messageLength < 255)
				return messageLength;
			if (numberOfBytesAvailable < NumberOfReservedBytesForMessageLength)
				return RememberIncompleteMessageLengthData();
			return ReadLength();
		}

		private int ReadLength()
		{
			var lengthBuffer = BuildLengthBufferFromPreviousAndCurrentData();
			int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
			if (messageLength > MaxMessageLength)
				throw new MessageLengthIsTooBig(messageLength); //ncrunch: no coverage
			return messageLength;
		}

		private byte[] BuildLengthBufferFromPreviousAndCurrentData()
		{
			var lengthBuffer = new byte[NumberOfReservedBytesForMessageLength];
			int index = 0;
			if (rememberMessageBytesFromLastTime != null)
				for (; index < rememberLengthFromLastTime; index++)
					lengthBuffer[index] = rememberMessageBytesFromLastTime[rememberOffsetFromLastTime + index];
			for (; index < NumberOfReservedBytesForMessageLength; index++)
			{
				lengthBuffer[index] = availableData[offset];
				offset++;
				numberOfBytesAvailable--;
			}
			rememberMessageBytesFromLastTime = null;
			return lengthBuffer;
		}

		private const int NumberOfReservedBytesForMessageLength = sizeof(int);

		/// <summary>
		/// Maximum size for one packet is 128 MB! Usually something is wrong if messages get this big.
		/// </summary>
		private const int MaxMessageLength = 1024 * 1024 * 128;

		//ncrunch: no coverage start
		private class MessageLengthIsTooBig : Exception
		{
			public MessageLengthIsTooBig(int messageLength)
				: base(messageLength + "") {}
		}
		//ncrunch: no coverage end

		private int RememberIncompleteMessageLengthData()
		{
			rememberMessageBytesFromLastTime = availableData;
			rememberOffsetFromLastTime = offset;
			rememberLengthFromLastTime = numberOfBytesAvailable;
			return NotEnoughBytesForMessageLength;
		}

		private byte[] rememberMessageBytesFromLastTime;
		private int rememberOffsetFromLastTime;
		private int rememberLengthFromLastTime;
		private const int NotEnoughBytesForMessageLength = -1;

		private void ReadDataToEnd()
		{
			var bytesRead = currentContainerToFill.ReadData(availableData, offset,
				numberOfBytesAvailable);
			offset += bytesRead;
			numberOfBytesAvailable -= bytesRead;
			if (currentContainerToFill.IsDataComplete)
				TriggerObjectFinishedAndResetCurrentContainer();
			if (numberOfBytesAvailable > 0)
				ReadBytes();
		}

		private void TriggerObjectFinishedAndResetCurrentContainer()
		{
			if (ObjectFinished != null)
				ObjectFinished(currentContainerToFill);
			currentContainerToFill = null;
		}

		public event Action<MessageData> ObjectFinished;
	}
}