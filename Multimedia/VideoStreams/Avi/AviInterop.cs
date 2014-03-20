using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Multimedia.VideoStreams.Avi
{
	/// <summary>
	/// Low level system calls for processing Avi files.
	/// </summary>
	public class AviInterop
	{
		public static readonly int StreamtypeVideo = MmioFourcc('v', 'i', 'd', 's');
		public static readonly int StreamtypeAudio = MmioFourcc('a', 'u', 'd', 's');
		public const int AvistreamreadConvenient = -1;
		public const int OfRead = 0;

		public static int MmioFourcc(char ch0, char ch1, char ch2, char ch3)
		{
			return (byte)(ch0) | ((byte)(ch1) << 8) | ((byte)(ch2) << 16) | ((byte)(ch3) << 24);
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Rect
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BitmapInfoHeader
		{
			public int biSize;
			public int biWidth;
			public int biHeight;
			public short biPlanes;
			public short biBitCount;
			public int biCompression;
			public int biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public int biClrUsed;
			public int biClrImportant;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PcmWaveFormat
		{
			public short wFormatTag;
			public short nChannels;
			public int nSamplesPerSec;
			public int nAvgBytesPerSec;
			public short nBlockAlign;
			public short wBitsPerSample;
			public short cbSize;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct StreamInfo
		{
			public int fccType;
			public int fccHandler;
			public int dwFlags;
			public int dwCaps;
			public short wPriority;
			public short wLanguage;
			public int dwScale;
			public int dwRate;
			public int dwStart;
			public int dwLength;
			public int dwInitialFrames;
			public int dwSuggestedBufferSize;
			public int dwQuality;
			public int dwSampleSize;
			public Rect rcFrame;
			public int dwEditCount;
			public int dwFormatChangeCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public short[] szName;
		}

		[DllImport("avifil32.dll")]
		internal static extern void AVIFileInit();

		[DllImport("avifil32.dll", PreserveSig = true)]
		internal static extern uint AVIFileOpen(ref int filePtr, String filepath, int uMode,
			int pclsidHandler);

		[DllImport("avifil32.dll")]
		internal static extern uint AVIFileGetStream(int filePtr, out IntPtr streamPtr, int fccType,
			int lParam);

		[DllImport("avifil32.dll", PreserveSig = true)]
		internal static extern int AVIStreamStart(IntPtr streamPtr);

		[DllImport("avifil32.dll", PreserveSig = true)]
		internal static extern int AVIStreamLength(IntPtr streamPtr);

		[DllImport("avifil32.dll")]
		internal static extern uint AVIStreamInfo(IntPtr streamPtr, ref StreamInfo psi, int lSize);

		[DllImport("avifil32.dll")]
		internal static extern int AVIStreamGetFrameOpen(IntPtr streamPtr, ref BitmapInfoHeader bih);

		[DllImport("avifil32.dll")]
		internal static extern int AVIStreamGetFrame(int pGetFrameObj, int lPos);

		[DllImport("avifil32.dll")]
		internal static extern int AVIStreamReadFormat(IntPtr streamPtr, int lPos,
			ref BitmapInfoHeader lpFormat, ref int cbFormat);

		[DllImport("avifil32.dll")]
		internal static extern int AVIStreamReadFormat(IntPtr streamPtr, int lPos,
			ref PcmWaveFormat lpFormat, ref int cbFormat);

		[DllImport("avifil32.dll")]
		internal static extern int AVIStreamGetFrameClose(int pGetFrameObj);

		[DllImport("avifil32.dll")]
		internal static extern int AVIStreamRelease(IntPtr streamPtr);

		[DllImport("avifil32.dll")]
		internal static extern uint AVIFileRelease(int filePtr);

		[DllImport("avifil32.dll")]
		internal static extern void AVIFileExit();

		[DllImport("avifil32.dll")]
		internal static extern uint AVIStreamRead(IntPtr streamPtr, int lStart, int lSamples,
			IntPtr lpBuffer, int cbBuffer, out int plBytes, out int plSamples);
	}
}