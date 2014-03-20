using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Multimedia.OpenAL.Helpers;

namespace DeltaEngine.Multimedia.OpenAL
{
	public sealed class ALSoundDevice : SoundDevice
	{
		private IntPtr audioDevice;
		private readonly IntPtr audioContext;

		public ALSoundDevice()
		{
			IntPtr currentContext = Invokes.alcGetCurrentContext();
			if (currentContext != IntPtr.Zero)
			{
				audioContext = currentContext;
				audioDevice = Invokes.alcGetContextsDevice(audioContext);
			}
			else
			{
				audioDevice = Invokes.alcOpenDevice("");
				if (audioDevice != IntPtr.Zero)
				{
					audioContext = CreateContext(audioDevice, new int[0]);
					Invokes.alcMakeContextCurrent(audioContext);
				}
			}
		}

		private static unsafe IntPtr CreateContext(IntPtr device, int[] attributes)
		{
			fixed (int* ptr = attributes)
				return Invokes.alcCreateContext(device, ptr);
		}

		public override void Dispose()
		{
			base.Dispose();
			if (audioDevice == IntPtr.Zero)
				return;
			Invokes.alcDestroyContext(audioContext);
			Invokes.alcCloseDevice(audioDevice);
			audioDevice = IntPtr.Zero;
		}

		public unsafe int CreateBuffer()
		{
			int buffer = 0;
			Invokes.alGenBuffers(1, &buffer);
			return buffer;
		}

		public unsafe int[] CreateBuffers(int numberOfBuffers)
		{
			int[] buffer = new int[numberOfBuffers];
			fixed (int* ptr = buffer)
				Invokes.alGenBuffers(numberOfBuffers, ptr);
			return buffer;
		}

		public unsafe void DeleteBuffer(int bufferHandle)
		{
			Invokes.alDeleteBuffers(1, &bufferHandle);
		}

		public unsafe void DeleteBuffers(int[] bufferHandles)
		{
			fixed (int* ptr = bufferHandles)
				Invokes.alDeleteBuffers(bufferHandles.Length, ptr);
		}

		public unsafe void BufferData(int bufferHandle, AudioFormat format, byte[] data, int length, int sampleRate)
		{
			fixed (byte* ptr = data)
				Invokes.alBufferData(bufferHandle, AudioFormatToALFormat(format), (IntPtr)ptr, length, sampleRate);
		}

		public unsafe int CreateChannel()
		{
			int source = 0;
			Invokes.alGenSources(1, &source);
			return source;
		}

		public unsafe void DeleteChannel(int channelHandle)
		{
			Invokes.alDeleteSources(1, &channelHandle);
		}

		public void AttachBufferToChannel(int bufferHandle, int channelHandle)
		{
			Invokes.alSourcei(channelHandle, All.Buffer, bufferHandle);
		}

		public unsafe void QueueBufferInChannel(int bufferHandle, int channelHandle)
		{
			Invokes.alSourceQueueBuffers(channelHandle, 1, &bufferHandle);
		}

		public unsafe int UnqueueBufferFromChannel(int channelHandle)
		{
			int result = 0;
			Invokes.alSourceUnqueueBuffers(channelHandle, 1, &result);
			return result;
		}

		public int GetNumberOfBuffersQueued(int channelHandle)
		{
			int result;
			Invokes.alGetSourcei(channelHandle, All.BuffersQueued, out result);
			return result;
		}

		public int GetNumberOfBuffersProcessed(int channelHandle)
		{
			int result;
			Invokes.alGetSourcei(channelHandle, All.BuffersProcessed, out result);
			return result;
		}

		public ChannelState GetChannelState(int channelHandle)
		{
			int result;
			Invokes.alGetSourcei(channelHandle, All.SourceState, out result);
			return ALSourceStateToChannelState((AudioState)result);
		}

		public void SetVolume(int channelHandle, float volume)
		{
			Invokes.alSourcef(channelHandle, All.Gain, volume);
		}

		public void SetPosition(int channelHandle, Vector3D position)
		{
			Invokes.alSource3f(channelHandle, All.Position, position.X, position.Y, position.Z);
		}

		public void SetPitch(int channelHandle, float pitch)
		{
			Invokes.alSourcef(channelHandle, All.Pitch, pitch);
		}

		public void Play(int channelHandle)
		{
			Invokes.alSourcePlay(channelHandle);
		}

		public void Stop(int channelHandle)
		{
			Invokes.alSourceStop(channelHandle);
		}

		public bool IsPlaying(int channelHandle)
		{
			int result;
			Invokes.alGetSourcei(channelHandle, All.SourceState, out result);
			return result == (int)AudioState.Playing;
		}

		private static ALFormat AudioFormatToALFormat(AudioFormat audioFormat)
		{
			switch (audioFormat)
			{
				case AudioFormat.Mono8:
					return ALFormat.Mono8;
				case AudioFormat.Mono16:
					return ALFormat.Mono16;
				case AudioFormat.Stereo8:
					return ALFormat.Stereo8;
				default:
					return ALFormat.Stereo16;
			}
		}

		private static ChannelState ALSourceStateToChannelState(AudioState alSourceState)
		{
			switch (alSourceState)
			{
				case AudioState.Playing:
					return ChannelState.Playing;
				case AudioState.Paused:
					return ChannelState.Paused;
				default:
					return ChannelState.Stopped;
			}
		}
	}
}