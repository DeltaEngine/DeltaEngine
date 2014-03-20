using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Multimedia.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	/// <summary>
	/// Test video playback. Xna video loading won't work from ReSharper, use Program.cs instead.
	/// </summary>
	public class VideoTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void LoadTestVideo()
		{
			testVideo = ContentLoader.Load<Video>("DefaultVideo");
		}

		private Video testVideo;

		[Test]
		public void ExpectExceptionIfVideoIsNotAvailable()
		{
			Assert.Throws<Video.VideoNotFoundOrAccessible>(
				() => ContentLoader.Load<Video>("NonExistingVideo"));
		}

		[Test]
		public void PlayVideo()
		{
			testVideo.Play();
		}

		[Test]
		public void PlayVideoOnClick()
		{
			new FontText(Font.Default, "Click to Play", Rectangle.One);
			new Command(() => { testVideo.Play(); }).Add(new MouseButtonTrigger());
		}

		[Test]
		public void PlayAndStop()
		{
			testVideo.Stop();
			Assert.IsFalse(testVideo.IsPlaying());
			testVideo.Play();
			Assert.IsTrue(testVideo.IsPlaying());
		}

		[Test]
		public void PlayAndStopWithEntitiesRunner()
		{
			int startedNumberOfEntities = EntitiesRunner.Current.NumberOfEntities;
			Assert.AreEqual(startedNumberOfEntities, EntitiesRunner.Current.NumberOfEntities);
			testVideo.Stop();
			Assert.AreEqual(startedNumberOfEntities, EntitiesRunner.Current.NumberOfEntities);
			testVideo.Play();
			Assert.AreEqual(startedNumberOfEntities + 1, EntitiesRunner.Current.NumberOfEntities);
			testVideo.Stop();
			Assert.AreEqual(startedNumberOfEntities, EntitiesRunner.Current.NumberOfEntities);
		}

		[Test]
		public void PlayVideoWhileOtherIsPlaying()
		{
			var otherTestVideo = ContentLoader.Load<Video>("DefaultVideo");
			testVideo.Play();
			Assert.False(MockVideo.VideoStopCalled);
			otherTestVideo.Play();
			Assert.False(MockVideo.VideoStopCalled);
		}

		[Test]
		public void CheckDurationAndPosition()
		{
			testVideo.Update();
			Assert.AreEqual(3.791f, testVideo.DurationInSeconds, 0.5f);
			Assert.AreEqual(1.0f, testVideo.PositionInSeconds);
		}

		[Test]
		public void StartAndStopVideo()
		{
			Assert.AreEqual(3.791f, testVideo.DurationInSeconds, 0.5f);
			var videoTester = new VideoPlayedOneSecondTester(testVideo);
			Assert.IsTrue(videoTester.IsPauseable);
			testVideo.Play();
		}

		private class VideoPlayedOneSecondTester : Entity, Updateable
		{
			public VideoPlayedOneSecondTester(Video video)
			{
				this.video = video;
			}

			private readonly Video video;

			public void Update()
			{
				if (Time.Total < 1)
					return;
				video.Stop();
				Assert.Less(0.99f, video.PositionInSeconds);
			}
		}
	}
}