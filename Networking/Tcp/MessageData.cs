using System;

namespace DeltaEngine.Networking.Tcp
{
	internal class MessageData
	{
		public MessageData(int dataLength)
		{
			Data = new byte[dataLength];
			readDataLength = -1;
		}

		public byte[] Data { get; private set; }
		private int readDataLength;
		public bool IsDataCompressed { get; private set; }

		public int ReadData(byte[] availableBytes, int offset, int availableBytesCurrentLength)
		{
			int bytesRead = 0;
			if (readDataLength == -1)
			{
				readDataLength = 0;
				IsDataCompressed = availableBytes[offset] != 0;
				offset++;
				availableBytesCurrentLength -= 1;
				bytesRead++;
				if (availableBytesCurrentLength == 0)
					return bytesRead;
			}
			int allowedBytesToRead = Data.Length - readDataLength;
			if (availableBytesCurrentLength < allowedBytesToRead)
				allowedBytesToRead = availableBytesCurrentLength;
			Array.Copy(availableBytes, offset, Data, readDataLength, allowedBytesToRead);
			readDataLength += allowedBytesToRead;
			bytesRead += allowedBytesToRead;
			return bytesRead;
		}

		public bool IsDataComplete
		{
			get { return readDataLength == Data.Length; }
		}
	}
}