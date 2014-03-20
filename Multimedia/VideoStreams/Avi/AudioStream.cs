using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Multimedia.VideoStreams.Avi
{
	/// <summary>
	/// The audio portion of an Avi stream.
	/// </summary>
	public class AudioStream : AviStream
	{
		public AudioStream(int filePtr, IntPtr streamPtr)
			: base(filePtr, streamPtr)
		{
			int size = Marshal.SizeOf(waveFormat);
			AviInterop.AVIStreamReadFormat(StreamPointer, 0, ref waveFormat, ref size);
		}

		private readonly AviInterop.PcmWaveFormat waveFormat;

		public byte[] GetStreamData()
		{
			int streamLength;
			int samples;
			uint result = AviInterop.AVIStreamRead(StreamPointer, sampleOffset,
				AviInterop.AvistreamreadConvenient, IntPtr.Zero, 0, out streamLength, out samples);
			if (result != 0)
				throw new Exception("Exception in AVIStreamRead: " + AviErrors.GetError(result));

			IntPtr waveData = Marshal.AllocHGlobal(streamLength);
			result = AviInterop.AVIStreamRead(StreamPointer, sampleOffset, samples,
				waveData, streamLength, out streamLength, out samples);
			if (result != 0)
				throw new Exception("Exception in AVIStreamRead: " + AviErrors.GetError(result));

			sampleOffset += samples;
			var resultData = new byte[streamLength];
			Marshal.Copy(waveData, resultData, 0, streamLength);
			Marshal.FreeHGlobal(waveData);

			return resultData;
		}

		private int sampleOffset;

		public int BitsPerSample
		{
			get { return waveFormat.wBitsPerSample; }
		}

		public int SamplesPerSecond
		{
			get { return waveFormat.nSamplesPerSec; }
		}

		public int Channels
		{
			get { return waveFormat.nChannels; }
		}
	}
}