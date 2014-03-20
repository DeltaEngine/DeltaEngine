using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Multimedia.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	public class SoundDeviceTests : TestWithMocksOrVisually
	{
		[Test]
		public void PlayMusicWhileOtherIsPlaying()
		{
			var music1 = ContentLoader.Load<Music>("DefaultMusic");
			var music2 = ContentLoader.Load<Music>("DefaultMusic");
			music1.Play();
			Assert.False(MockMusic.MusicStopCalled);
			music2.Play();
			Assert.False(MockMusic.MusicStopCalled);
		}

		[Test]
		public void RunWithVideoAndMusic()
		{
			var video = ContentLoader.Load<Video>("DefaultVideo");
			var music = ContentLoader.Load<Music>("DefaultMusic");
			video.Play();
			music.Play();
		}

		[Test]
		public void TestIfPLayingMusic()
		{
			var video = ContentLoader.Load<Video>("DefaultVideo");
			video.Play();
			Assert.IsTrue(video.IsPlaying());
			Assert.AreEqual(3.33333325f, video.DurationInSeconds);
			Assert.AreEqual(1.0f, video.PositionInSeconds);
		}

		[Test, CloseAfterFirstFrame]
		public void PlayMusicAndVideo()
		{
			var device = new MockSoundDevice();
			Assert.IsTrue(device.IsInitialized);
			var music = ContentLoader.Load<Music>("DefaultMusic");
			var video = ContentLoader.Load<Video>("DefaultVideo");
			music.Play();
			video.Play();
			Assert.False(MockMusic.MusicStopCalled);
			Assert.False(MockVideo.VideoStopCalled);
			device.RegisterCurrentMusic(music);
			device.RegisterCurrentVideo(video);
			Assert.IsTrue(device.IsActive);
			Assert.IsTrue(device.IsInitialized);
			device.RapidUpdate();
			device.Dispose();
		}

		[Test, CloseAfterFirstFrame]
		public void PlayAndStopMusic()
		{
			var device = new MockSoundDevice();
			Assert.IsFalse(device.IsPauseable);
			Assert.IsTrue(device.IsInitialized);
			var musicTime = ContentLoader.Load<Music>("DefaultMusic");
			musicTime.StreamFinished = () => { };
			musicTime.Play();
			Assert.AreEqual(Settings.Current.MusicVolume, musicTime.Volume);
			device.RegisterCurrentMusic(musicTime);
			musicTime.Stop();
			device.RapidUpdate();
			musicTime.Loop = true;
			musicTime.Stop();
			device.RapidUpdate();
			device.Dispose();
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeMusicVolume()
		{
			var device = new MockSoundDevice();
			float defaultMusicVolume = Settings.Current.MusicVolume;
			Assert.IsTrue(device.IsInitialized);
			try
			{
				device.MusicVolume = 1.0f;
				Assert.AreEqual(1.0f, device.MusicVolume);
			}
			finally
			{
				device.MusicVolume = defaultMusicVolume;
			}
			device.RapidUpdate();
			device.Dispose();
		}
	}
}