using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Multimedia.OpenAL.Helpers;
using DeltaEngine.Multimedia.VideoStreams;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Multimedia.OpenAL
{
	public abstract class OpenALVideo : Video
	{
		protected readonly ALSoundDevice openAL;
		protected int channelHandle;
		protected int[] buffers;
		protected const int NumberOfBuffers = 4;
		protected BaseVideoStream video;
		protected AudioFormat format;
		protected Sprite surface;
		protected float elapsedSeconds;

		public override float DurationInSeconds
		{
			get
			{
				return video.LengthInSeconds;
			}
		}

		public override float PositionInSeconds
		{
			get
			{
				return MathExtensions.Round(elapsedSeconds.Clamp(0f, DurationInSeconds), 2);
			}
		}

		protected OpenALVideo(string contentName, ALSoundDevice soundDevice)
			: base(contentName, soundDevice)
		{
			openAL = soundDevice;
		}

		protected override void LoadData(Stream fileData)
		{
			try
			{
				video = new VideoStreamFactory().Load(fileData, "Content/" + Name);
				format = video.Channels == 2 ? AudioFormat.Stereo16 : AudioFormat.Mono16;
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (Debugger.IsAttached)
					throw new VideoNotFoundOrAccessible(Name, ex);
			}
		}

		protected override void DisposeData()
		{
			base.DisposeData();
			openAL.DeleteBuffers(buffers);
			openAL.DeleteChannel(channelHandle);
			video.Dispose();
			video = null;
		}

		protected bool Stream(int buffer)
		{
			try
			{
				byte[] bufferData = new byte[4096];
				video.ReadMusicBytes(bufferData, bufferData.Length);
				openAL.BufferData(buffer, format, bufferData, bufferData.Length, video.Samplerate);
				openAL.QueueBufferInChannel(buffer, channelHandle);
			}
			catch
			{
				return false;
			}
			return true;
		}

		protected override void StopNativeVideo()
		{
			if (surface != null)
				surface.IsActive = false;
			elapsedSeconds = 0;
			surface = null;
			openAL.Stop(channelHandle);
			EmptyBuffers();
			video.Stop();
		}

		protected void EmptyBuffers()
		{
			int queued = openAL.GetNumberOfBuffersQueued(channelHandle);
			while (queued-- > 0)
				openAL.UnqueueBufferFromChannel(channelHandle);
		}

		public override bool IsPlaying()
		{
			return GetState() != ChannelState.Stopped;
		}

		private ChannelState GetState()
		{
			return openAL.GetChannelState(channelHandle);
		}

		public override void Update()
		{
			if (GetState() == ChannelState.Paused)
				return;
			elapsedSeconds += Time.Delta;
			bool isFinished = UpdateBuffersAndCheckFinished();
			if (isFinished)
			{
				Stop();
				return;
			}
			UpdateVideoTexture();
			if (GetState() != ChannelState.Playing)
				openAL.Play(channelHandle);
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

		protected abstract void UpdateVideoTexture();
	}
}