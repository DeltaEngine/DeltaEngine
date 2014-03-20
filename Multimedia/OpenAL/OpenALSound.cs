using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Multimedia.OpenAL.Helpers;

namespace DeltaEngine.Multimedia.OpenAL
{
	public class OpenALSound : Sound
	{
		protected readonly ALSoundDevice openAL;
		protected WaveSoundData soundData;
		protected float length;
		protected int bufferHandle;
		protected const int InvalidHandle = -1;

		public override float LengthInSeconds
		{
			get
			{
				return length;
			}
		}

		protected OpenALSound(string contentName, ALSoundDevice openAL)
			: base(contentName)
		{
			this.openAL = openAL;
		}

		protected override void LoadData(Stream fileData)
		{
			try
			{
				TryLoadSound(fileData);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (Debugger.IsAttached)
					throw new SoundNotFoundOrAccessible(Name, ex);
			}
		}

		private void TryLoadSound(Stream fileData)
		{
			BinaryReader streamReader = new BinaryReader(fileData);
			soundData = new WaveSoundData(streamReader);
			length = CacheLengthInSeconds();
			bufferHandle = CreateNativeBuffer();
		}

		protected float CacheLengthInSeconds()
		{
			float blockAlign = soundData.Channels * 2f;
			return (soundData.BufferData.Length / blockAlign) / soundData.SampleRate;
		}

		protected int CreateNativeBuffer()
		{
			int newHandle = openAL.CreateBuffer();
			if (newHandle <= 0)
				throw new UnableToCreateSoundBufferOpenALMightNotBeInitializedCorrectly();
			openAL.BufferData(newHandle, soundData.Format, soundData.BufferData, soundData.BufferData.Length, soundData.SampleRate);
			return newHandle;
		}

		protected override void DisposeData()
		{
			base.DisposeData();
			if (bufferHandle != InvalidHandle)
				openAL.DeleteBuffer(bufferHandle);
			bufferHandle = InvalidHandle;
		}

		public override void PlayInstance(SoundInstance instanceToPlay)
		{
			int channelHandle = (int)instanceToPlay.Handle;
			if (channelHandle == InvalidHandle)
				return;
			openAL.SetVolume(channelHandle, instanceToPlay.Volume);
			openAL.SetPosition(channelHandle, new Vector3D(instanceToPlay.Panning, 0.0f, 0.0f));
			openAL.SetPitch(channelHandle, instanceToPlay.Pitch);
			openAL.Play(channelHandle);
		}

		public override void StopInstance(SoundInstance instanceToStop)
		{
			int channelHandle = (int)instanceToStop.Handle;
			if (channelHandle == InvalidHandle)
				return;
			openAL.Stop(channelHandle);
		}

		protected override void CreateChannel(SoundInstance instanceToAdd)
		{
			int channelHandle = openAL.CreateChannel();
			if (channelHandle <= 0)
				Logger.Error(new UnableToCreateSoundChannelOpenALMightNotBeInitializedCorrectly());
			openAL.AttachBufferToChannel(bufferHandle, channelHandle);
			instanceToAdd.Handle = channelHandle;
		}

		protected override void RemoveChannel(SoundInstance instanceToRemove)
		{
			int channelHandle = (int)instanceToRemove.Handle;
			if (channelHandle != InvalidHandle)
				openAL.DeleteChannel(channelHandle);
			instanceToRemove.Handle = InvalidHandle;
		}

		public override bool IsPlaying(SoundInstance instanceToCheck)
		{
			int channelHandle = (int)instanceToCheck.Handle;
			return channelHandle != InvalidHandle && openAL.IsPlaying(channelHandle);
		}

		public class UnableToCreateSoundBufferOpenALMightNotBeInitializedCorrectly : Exception {}

		public class UnableToCreateSoundChannelOpenALMightNotBeInitializedCorrectly : Exception {}
	}
}