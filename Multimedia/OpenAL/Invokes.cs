using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Multimedia.OpenAL
{
	internal static class Invokes
	{
		private const string OpenALLib = "openal32.dll";

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr alcOpenDevice(string devicename);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr alcCaptureOpenDevice(string deviceName, uint frequency, ALFormat format, int buffersize);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alcCaptureSamples(IntPtr device, IntPtr buffer, int sample);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alcCaptureStart(IntPtr device);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alcCaptureStop(IntPtr device);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alcCaptureCloseDevice(IntPtr device);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alcCloseDevice(IntPtr device);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe IntPtr alcCreateContext(IntPtr device, int* attrlist);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool alcMakeContextCurrent(IntPtr context);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alcProcessContext(IntPtr context);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alcSuspendContext(IntPtr context);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alcDestroyContext(IntPtr context);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr alcGetContextsDevice(IntPtr context);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr alcGetCurrentContext();

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern AlcError alcGetError(IntPtr device);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr alcGetString(IntPtr device, AlcGetString param);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alcGetIntegerv(IntPtr device, AlcGetInteger param, int size, int* data);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGenBuffers(int n, [Out] int* buffers);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alDeleteBuffers(int n, int* buffers);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool alIsBuffer(int buffer);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alGetBufferi(int buffer, All param, [Out] out int value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alBufferData(int buffer, ALFormat format, IntPtr bufferData, int size, int freq);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alBufferf(uint buffer, All key, float value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alBufferi(uint buffer, All key, int value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alBuffer3f(uint buffer, All key, float value1, float value2, float value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alBuffer3i(uint buffer, All key, int value1, int value2, int value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alGetBufferf(uint buffer, All param, [Out] out float value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alGetBuffer3f(uint buffer, All param, [Out] out float value1, [Out] out float value2, [Out] out float value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alGetBuffer3i(uint buffer, All param, [Out] out int value1, [Out] out int value2, [Out] out int value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alBufferfv(uint buffer, All param, float* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alBufferiv(uint buffer, All param, int* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGenSources(int n, [Out] int* sources);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alDeleteSources(int n, int* sources);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool alIsSource(int source);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alSourcePlay(int source);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alSourcePlayv(int count, int* sources);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alSourceRewind(int source);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alSourceRewindv(int count, int* sources);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alSourcePause(int source);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alSourcePausev(int count, int* sources);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alSourceStop(int source);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alSourceStopv(int count, int* sources);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alSourcei(int source, All key, int value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alSourceiv(int source, All key, int* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alGetSourcei(int source, All param, [Out] out int value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alGetSource3i(int source, All param, [Out] out int value1, [Out] out int value2, [Out] out int value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alSourcef(int source, All key, float value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alSourcefv(int source, All key, float* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alSource3f(int source, All key, float value1, float value2, float value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alSource3i(int source, All key, int value1, int value2, int value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alGetSourcef(int source, All param, [Out] out float value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alGetSource3f(int source, All param, [Out] out float value1, [Out] out float value2, [Out] out float value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetSourcefv(int source, All param, [Out] out float* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetSourceiv(int source, All param, [Out] out int* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alSourceQueueBuffers(int source, int size, int* buffers);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alSourceUnqueueBuffers(int source, int size, int* buffers);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alListenerf(All key, float value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alListener3f(All key, float value1, float value2, float value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alListenerfv(All key, float* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alListener3i(All key, int value1, int value2, int value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alListeneriv(All key, int* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetListenerf(All key, [Out] out float* value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetListener3f(All key, [Out] out float* value1, [Out] out float* value2, [Out] out float* value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetListenerfv(All key, [Out] out float* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetListeneri(All key, [Out] out int* value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetListener3i(All key, [Out] out int* value1, [Out] out int* value2, [Out] out int* value3);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetListeneriv(All key, [Out] out int* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alDopplerFactor(float value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alSpeedOfSound(float value);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool alIsEnabled(All capability);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alEnable(All capability);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alDisable(All capability);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern void alDistanceModel(All distanceModel);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern bool alGetBoolean(All capability);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern double alGetDouble(All capability);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern float alGetFloat(All capability);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern int alGetInteger(All capability);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetBooleanv(All capability, [Out] out bool* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetDoublev(All capability, [Out] out double* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetFloatv(All capability, [Out] out float* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe void alGetIntegerv(All capability, [Out] out int* values);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern unsafe char* alGetString(All parameter);

		[DllImport(OpenALLib, CallingConvention = CallingConvention.Cdecl)]
		public static extern ALError alGetError();
	}
}