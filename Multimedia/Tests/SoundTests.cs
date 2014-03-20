using System.Diagnostics;
using System.Threading;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Multimedia.Tests
{
	public class SoundTests : TestWithMocksOrVisually
	{
		[Test]
		public void PlaySound()
		{
			ContentLoader.Load<Sound>("DefaultSound").Play();
		}

		[Test]
		public void PlaySoundVerySilent()
		{
			ContentLoader.Load<Sound>("DefaultSound").Play(0.1f);
		}

		[Test]
		public void PlaySoundOnClick()
		{
			new Command(() => ContentLoader.Load<Sound>("DefaultSound").Play()).Add(
				new MouseButtonTrigger());
		}

		[Test]
		public void PlaySoundAndDispose()
		{
			var sound = ContentLoader.Load<Sound>("DefaultSound");
			sound.Dispose();
		}

		[Test]
		public void PlaySoundLeft()
		{
			ContentLoader.Load<Sound>("DefaultSound").Play(1, -1);
		}

		[Test]
		public void PlaySoundRightAndPitched()
		{
			var sound = ContentLoader.Load<Sound>("DefaultSound");
			var instance = sound.CreateSoundInstance();
			instance.Panning = 1.0f;
			instance.Pitch = 2.0f;
			instance.Play();
		}

		[Test]
		public void PlaySoundInstance()
		{
			var sound = ContentLoader.Load<Sound>("DefaultSound");
			var instance = sound.CreateSoundInstance();
			Assert.AreEqual(0.48f, sound.LengthInSeconds, 0.01f);
			Assert.AreEqual(false, instance.IsPlaying);
			instance.Play();
			Assert.AreEqual(true, instance.IsPlaying);
		}

		[Test]
		public void PlayMultipleSoundInstances()
		{
			var sound = ContentLoader.Load<Sound>("DefaultSound");
			var instance1 = sound.CreateSoundInstance();
			var instance2 = sound.CreateSoundInstance();
			Assert.AreEqual(false, instance1.IsPlaying);
			instance1.Play();
			Assert.AreEqual(true, instance1.IsPlaying);
			Assert.AreEqual(false, instance2.IsPlaying);
			instance2.Volume = 0.5f;
			instance2.Panning = -1.0f;
			instance2.Play();
			Assert.AreEqual(true, instance2.IsPlaying);
		}

		[Test]
		public void NumberOfPlayingInstances()
		{
			var sound = ContentLoader.Load<Sound>("DefaultSound");
			Assert.AreEqual(0, sound.NumberOfPlayingInstances);
			sound.Play();
			Assert.AreEqual(1, sound.NumberOfPlayingInstances);
			sound.Play();
			Assert.AreEqual(2, sound.NumberOfPlayingInstances);
		}

		[Test]
		public void PlayAndStop()
		{
			var sound = ContentLoader.Load<Sound>("DefaultSound");
			Assert.IsFalse(sound.IsAnyInstancePlaying);
			sound.Play();
			Assert.IsTrue(sound.IsAnyInstancePlaying);
			sound.StopAll();
			WaitUntilSoundStateIsUpdated();
			Assert.IsFalse(sound.IsAnyInstancePlaying);
			sound.Play();
		}

		[Test]
		public void PlayAndStopEvents()
		{
			var sound = ContentLoader.Load<Sound>("DefaultSound");
			sound.OnPlay += instance => Assert.True(sound.IsAnyInstancePlaying);
			sound.OnStop += instance => Assert.False(sound.IsAnyInstancePlaying);
			sound.Play();
			sound.StopAll();
			WaitUntilSoundStateIsUpdated();
		}

		[Test]
		public void PlayAndStopInstance()
		{
			var sound = ContentLoader.Load<Sound>("DefaultSound");
			var instance = sound.CreateSoundInstance();
			Assert.IsFalse(sound.IsAnyInstancePlaying);
			Assert.AreEqual(0f, instance.PositionInSeconds);
			instance.Play();
			Assert.IsTrue(sound.IsAnyInstancePlaying);
			WaitUntilSoundStateIsUpdated();
			Assert.Greater(instance.PositionInSeconds, 0f);
			sound.StopAll();
			WaitUntilSoundStateIsUpdated();
			Assert.AreEqual(0f, instance.PositionInSeconds);
			Assert.IsFalse(sound.IsAnyInstancePlaying);
		}

		private static void WaitUntilSoundStateIsUpdated()
		{
			Thread.Sleep(20);
		}

		[Test]
		public void DisposeSoundInstance()
		{
			var sound = ContentLoader.Load<Sound>("DefaultSound");
			var instance = sound.CreateSoundInstance();
			Assert.AreEqual(1, sound.NumberOfInstances);
			Assert.AreEqual(0, sound.NumberOfPlayingInstances);
			instance.Dispose();
			Assert.AreEqual(0, sound.NumberOfInstances);
			Assert.AreEqual(0, sound.NumberOfPlayingInstances);
		}

		[Test]
		public void DisposeSoundInstancesFromSoundClass()
		{
			var sound = ContentLoader.Load<Sound>("DefaultSound");
			sound.CreateSoundInstance();
			sound.Play();
			Assert.AreEqual(2, sound.NumberOfInstances);
			Assert.AreEqual(1, sound.NumberOfPlayingInstances);
			sound.Dispose();
			Assert.AreEqual(0, sound.NumberOfInstances);
			Assert.AreEqual(0, sound.NumberOfPlayingInstances);
		}

		[Test]
		public void ShouldThrowIfSoundNotLoadedInDebugModeOrWithDebuggerAttached()
		{
			if (!Debugger.IsAttached)
				return;
			//ncrunch: no coverage start
			Assert.Throws<ContentLoader.ContentNotFound>(
				() => ContentLoader.Load<Sound>("UnavailableSound"));
		}
	}
}