using System.IO;

namespace DeltaEngine.Multimedia.OpenAL.Helpers
{
	internal struct IeeeFloatConverter
	{
		private readonly bool is64BitFloat;

		public IeeeFloatConverter(int bitsPerSample)
		{
			is64BitFloat = bitsPerSample == 64;
		}

		public byte[] ConvertToPcm(byte[] sourceData)
		{
			byte[] result;
			using (MemoryStream resultStream = new MemoryStream())
			{
				BinaryWriter writer = new BinaryWriter(resultStream);
				BinaryReader reader = new BinaryReader(new MemoryStream(sourceData));
				ReadAllSamples(reader, writer, sourceData.Length);
				result = resultStream.ToArray();
			}
			return result;
		}

		private void ReadAllSamples(BinaryReader reader, BinaryWriter writer, int sourceLength)
		{
			int length = sourceLength / (is64BitFloat ? 8 : 4);
			for (int index = 0; index < length; index++)
			{
				double value = is64BitFloat ? reader.ReadDouble() : reader.ReadSingle();
				writer.Write((short)(value * 32767));
			}
		}
	}
}