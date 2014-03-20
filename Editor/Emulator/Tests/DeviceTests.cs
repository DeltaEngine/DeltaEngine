using System.Windows;
using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class DeviceTests
	{
		[SetUp]
		public void RegisterTypesForConversion()
		{
			EmulatorExtensions.RegisterTypesForConversion();
		}

		[Test]
		public void DefaultDeviceHasNoImageAndCannotBeRotatedOrRescaled()
		{
			var device = new Device(EmulatorTestExtensions.CreateDefaultDeviceData());
			Assert.AreEqual("Default", device.Type);
			Assert.AreEqual("Default", device.Name);
			Assert.AreEqual("", device.ImageFile);
			Assert.AreEqual(new Point(0, 0), device.ScreenPoint);
			Assert.AreEqual(new Size(0, 0), device.ScreenSize);
			Assert.IsFalse(device.CanRotate);
			Assert.IsFalse(device.CanScale);
			Assert.AreEqual(2, device.DefaultScaleIndex);
		}

		[Test]
		public void EmulatorDeviceCanBeRotatedAndScaled()
		{
			var device = new Device(EmulatorTestExtensions.CreateWindows8DeviceData());
			Assert.AreEqual("Windows", device.Type);
			Assert.AreEqual("Windows 8 1080p", device.Name);
			Assert.AreEqual("W8Emulator1080p", device.ImageFile);
			Assert.AreEqual(new Point(33, 33), device.ScreenPoint);
			Assert.AreEqual(new Size(1920, 1080), device.ScreenSize);
			Assert.IsTrue(device.CanRotate);
			Assert.IsTrue(device.CanScale);
			Assert.AreEqual(0, device.DefaultScaleIndex);
		}

		[Test]
		public void ToStringContainsTypeAndName()
		{
			var defaultDevice = new Device(EmulatorTestExtensions.CreateDefaultDeviceData());
			var win8Device = new Device(EmulatorTestExtensions.CreateWindows8DeviceData());
			Assert.AreEqual("Default", defaultDevice.ToString());
			Assert.AreEqual("Windows - Windows 8 1080p", win8Device.ToString());
		}
	}
}