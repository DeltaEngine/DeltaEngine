using System;
using System.IO;

namespace DeltaEngine.Multimedia.OpenAL.Helpers
{
	internal class MsAdpcmConverter
	{
		private readonly int numberOfSamples;
		private readonly int channels;
		private readonly short blockAlign;
		private StateObject[] states;
		private static readonly int[] AdaptationTable = { 230, 230, 230, 230, 307, 409, 512, 614, 768, 614, 512, 409, 307, 230, 230, 230 };
		private static readonly int[] AdaptCoeff1 = { 256, 512, 0, 192, 240, 460, 392 };
		private static readonly int[] AdaptCoeff2 = { 0, -256, 0, 64, 0, -208, -232 };

		public MsAdpcmConverter(int setChannels, short samplesPerBlock, short setBlockAlign)
		{
			channels = setChannels;
			blockAlign = setBlockAlign;
			numberOfSamples = (samplesPerBlock - 2) * channels;
			InitializeStates();
		}

		private void InitializeStates()
		{
			StateObject firstState = new StateObject();
			StateObject secondState = channels > 1 ? new StateObject() : firstState;
			states = new StateObject[] { firstState, secondState };
		}

		public byte[] ConvertToPcm(byte[] data)
		{
			byte[] resultBytes;
			using (MemoryStream result = new MemoryStream())
			{
				BinaryReader reader = new BinaryReader(new MemoryStream(data));
				BinaryWriter writer = new BinaryWriter(result);
				DecodeAllBlocks(data.Length, reader, writer);
				resultBytes = result.ToArray();
			}
			return resultBytes;
		}

		private void DecodeAllBlocks(int length, BinaryReader reader, BinaryWriter writer)
		{
			int numberOfBlocks = length / blockAlign;
			for (int blockIndex = 0; blockIndex < numberOfBlocks; blockIndex++)
				DecodeBlock(reader, writer);
		}

		private void DecodeBlock(BinaryReader reader, BinaryWriter writer)
		{
			ReadPredicatesAndDeltas(reader);
			ReadFirstSamplesAndWriteThemToResult(reader, writer);
			DecodeAllSamples(reader, writer);
		}

		private void ReadPredicatesAndDeltas(BinaryReader reader)
		{
			for (int index = 0; index < channels; index++)
				states[index].predicator = reader.ReadByte();
			for (int index = 0; index < channels; index++)
				states[index].delta = reader.ReadInt16();
		}

		private void ReadFirstSamplesAndWriteThemToResult(BinaryReader reader, BinaryWriter writer)
		{
			for (int index = 0; index < channels; index++)
				states[index].sample1 = reader.ReadInt16();
			for (int index = 0; index < channels; index++)
			{
				states[index].sample2 = reader.ReadInt16();
				writer.Write(states[index].sample2);
			}
			for (int index = 0; index < channels; index++)
				writer.Write(states[index].sample1);
		}

		private void DecodeAllSamples(BinaryReader reader, BinaryWriter writer)
		{
			for (int index = 0; index < numberOfSamples; index += 2)
			{
				byte code = reader.ReadByte();
				DecodeSample(states[0], code >> 4, writer);
				DecodeSample(states[1], code & 0x0f, writer);
			}
		}

		private static void DecodeSample(StateObject state, int code, BinaryWriter writer)
		{
			int linearSample = CalculateSampleOverflowSafe(state, code);
			MoveToNextSampleClampedTo16Bit(state, linearSample);
			state.delta = (short)Math.Max((state.delta * AdaptationTable[code]) / 256, 16);
			writer.Write(state.sample1);
		}

		private static int CalculateSampleOverflowSafe(StateObject state, int code)
		{
			int adaptedSample1 = state.sample1 * AdaptCoeff1[state.predicator];
			int adaptedSample2 = state.sample2 * AdaptCoeff2[state.predicator];
			int linearSample = (adaptedSample1 + adaptedSample2) / 256;
			code = (code & 0x08) != 0 ? (code - 0x10) : code;
			return linearSample + (state.delta * code);
		}

		private static void MoveToNextSampleClampedTo16Bit(StateObject state, int linearSample)
		{
			state.sample2 = state.sample1;
			state.sample1 = (short)Math.Min(short.MaxValue, Math.Max(short.MinValue, linearSample));
		}

		private class StateObject
		{
			public byte predicator;
			public short delta;
			public short sample1;
			public short sample2;
		}
	}
}