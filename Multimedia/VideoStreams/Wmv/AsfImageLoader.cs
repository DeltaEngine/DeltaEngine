using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AsfMojo.File;
using AsfMojo.Media;
using WindowsMediaLib;

namespace DeltaEngine.Multimedia.VideoStreams.Wmv
{
	internal class AsfImageLoader : IDisposable
	{
		public AsfImageLoader(AsfFile file)
		{
			stream = new AsfStream(file, AsfStreamType.asfImage, 0);
			WMUtils.WMCreateSyncReader(IntPtr.Zero, Rights.Playback, out syncReader);
			syncReader.OpenStream(new AsfIStream(stream));
			syncReader.SetReadStreamSamples((short)stream.Configuration.AsfVideoStreamId, false);
			Width = stream.Configuration.ImageWidth;
			Height = stream.Configuration.ImageHeight;
		}

		private AsfStream stream;
		private IWMSyncReader syncReader;
		public int Width { get; private set; }
		public int Height { get; private set; }

		public Bitmap GetImageData(float delta)
		{
			if (timeToNextImage > 0)
			{
				timeToNextImage -= delta;
				return lastData;
			}
			while (!TryGetNextImage()) {}
			return lastData;
		}

		private float timeToNextImage;
		private Bitmap lastData;

		private bool TryGetNextImage()
		{
			bool result = false;
			INSSBuffer buffer;
			long duration;
			SampleFlag sampleFlag;
			if (!ReadNextSample(out buffer, out duration, out sampleFlag))
			{
				lastData = null;
				return true;
			}
			if ((sampleFlag & SampleFlag.CleanPoint) == SampleFlag.CleanPoint)
			{
				ExtractSampleData(buffer, duration);
				result = true;
			}
			Marshal.FinalReleaseComObject(buffer);
			return result;
		}

		private bool ReadNextSample(out INSSBuffer buffer, out long duration,
			out SampleFlag sampleFlag)
		{
			try
			{
				long sampleTime;
				int outputNum;
				short streamNum;
				syncReader.GetNextSample(0, out buffer, out sampleTime, out duration, out sampleFlag,
					out outputNum, out streamNum);
			}
			catch (Exception)
			{
				buffer = null;
				duration = 0;
				sampleFlag = 0;
				return false;
			}
			return true;
		}

		private void ExtractSampleData(INSSBuffer buffer, long duration)
		{
			IntPtr source;
			int dataLength;
			buffer.GetBufferAndLength(out source, out dataLength);
			var array = new byte[dataLength];
			Marshal.Copy(source, array, 0, dataLength);
			SaveBytesToLastData(array);
			timeToNextImage += duration / 100000f;
		}

		private void SaveBytesToLastData(byte[] data)
		{
			lastData = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
			BitmapData bitmapData = lastData.LockBits(new Rectangle(0, 0, Width, Height),
				ImageLockMode.WriteOnly, lastData.PixelFormat);
			Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);
			lastData.UnlockBits(bitmapData);
			lastData.RotateFlip(RotateFlipType.Rotate180FlipX);
		}

		public void Dispose()
		{
			if (syncReader != null)
				Marshal.FinalReleaseComObject(syncReader);
			syncReader = null;
			if (stream != null)
				stream.Dispose();
			stream = null;
		}
	}
}