using System;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Input.Windows.Tests
{
	public class TouchHookTests
	{
		[SetUp]
		public void CreateHook()
		{
			resolver = new MockResolver();
			var window = resolver.Window;
			hook = new TouchHook(window);
		}

		private MockResolver resolver;
		private TouchHook hook;

		[TearDown]
		public void DisposeHook()
		{
			hook.Dispose();
			resolver.Dispose();
		}

		[Test]
		public void GetTouchDataFromHandleWithInvalidHandle()
		{
			Assert.IsEmpty(hook.nativeTouches);
			var nativeTouches = TouchHook.GetTouchDataFromHandle(1, IntPtr.Zero);
			Assert.Null(nativeTouches);
		}
	}
}