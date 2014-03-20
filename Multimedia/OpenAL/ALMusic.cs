using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Multimedia.OpenAL.Helpers;
using System.Diagnostics;
using DeltaEngine.Multimedia.MusicStreams;

namespace DeltaEngine.Multimedia.OpenAL
{
	public class ALMusic : Music
	{
		protected readonly ALSoundDevice openAL;
		protected readonly int channelHandle;
		protected readonly int[] buffers;
		protected readonly byte[] bufferData;
		protected const int BufferSize = 1024 * 16;
		protected BaseMusicStream musicStream;
		protected AudioFormat format;
		protected DateTime playStartTime;

		public override float DurationInSeconds
		{
			get
			{
				return musicStream.LengthInSeconds;
			}
		}

		public override float PositionInSeconds
		{
			get
			{
				float seconds = (float)DateTime.Now.Subtract(playStartTime).TotalSeconds;
				return seconds.Clamp(0f, DurationInSeconds).Round(2);
			}
		}

		protected ALMusic(string contentName, ALSoundDevice soundDevice)
			: base(contentName, soundDevice)
		{
			openAL = soundDevice;
			channelHandle = openAL.CreateChannel();
			buffers = openAL.CreateBuffers(NumberOfBuffers);
			bufferData = new byte[BufferSize];
		}

		protected override void LoadData(Stream fileData)
		{
			try
			{
				TryLoadData(fileData);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (Debugger.IsAttached)
					throw new CouldNotLoadMusicFromFilestream(Name, ex);
			}
		}

		private void TryLoadData(Stream fileData)
		{
			MemoryStream stream = new MemoryStream();
			fileData.CopyTo(stream);
			stream.Seek(0, SeekOrigin.Begin);
			musicStream = new MusicStreamFactory().Load(stream);
			format = musicStream.Channels == 2 ? AudioFormat.Stereo16 : AudioFormat.Mono16;
		}

		protected override void PlayNativeMusic()
		{
			musicStream.Rewind();
			for (int index = 0; index < NumberOfBuffers; index++)
				if (!Stream(buffers[index]))
					break;
			openAL.Play(channelHandle);
			playStartTime = DateTime.Now;
		}

		protected override void SetPlayingVolume(float value)
		{
			openAL.SetVolume(channelHandle, value);
		}

		protected override void StopNativeMusic()
		{
			openAL.Stop(channelHandle);
			EmptyBuffers();
		}

		private void EmptyBuffers()
		{
			int queued = openAL.GetNumberOfBuffersQueued(channelHandle);
			while (queued-- > 0)
				openAL.UnqueueBufferFromChannel(channelHandle);
		}

		public override bool IsPlaying()
		{
			return GetState() != ChannelState.Stopped;
		}

		public override void Run()
		{
			if (UpdateBuffersAndCheckFinished())
				HandleStreamFinished();
			else if (!IsPlaying())
				openAL.Play(channelHandle);
		}

		private ChannelState GetState()
		{
			return openAL.GetChannelState(channelHandle);
		}

		private bool UpdateBuffersAndCheckFinished()
		{
			int processed = openAL.GetNumberOfBuffersProcessed(channelHandle);
			while (processed-- > 0)
			{
				int buffer = openAL.UnqueueBufferFromChannel(channelHandle);
				if (!Stream(buffer))
					return true;
			}
			return false;
		}

		private bool Stream(int buffer)
		{
			try
			{
				return TryStream(buffer);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool TryStream(int buffer)
		{
			int bytesRead = musicStream.Read(bufferData, BufferSize);
			if (bytesRead == 0)
				return false;
			openAL.BufferData(buffer, format, bufferData, bytesRead, musicStream.Samplerate);
			openAL.QueueBufferInChannel(buffer, channelHandle);
			return true;
		}

		protected override void DisposeData()
		{
			base.DisposeData();
			openAL.DeleteBuffers(buffers);
			openAL.DeleteChannel(channelHandle);
			musicStream = null;
		}
	}
}