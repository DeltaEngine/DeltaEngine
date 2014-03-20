using System.Threading;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	public class ThreadExtensionTests
	{
		[Test]
		public void Start1()
		{
			int num = 1;
			Assert.AreEqual(1, num);
			Thread t = ThreadExtensions.Start(() => IncrementNumber(ref num));
			t.Join();
			Assert.AreEqual(2, num);
		}

		private static void IncrementNumber(ref int someNumber)
		{
			Thread.Sleep(5);
			someNumber++;
		}

		[Test]
		public void Start2()
		{
			Thread t = ThreadExtensions.Start("thread name", () => { });
			Assert.AreEqual("thread name", t.Name);
		}

		[Test]
		public void Start3()
		{
			const string Param = "name from parameter";
			Thread t = ThreadExtensions.Start(UpdateNameViaParameter, Param);
			t.Join();
			Assert.AreEqual(Param, t.Name);
		}

		private static void UpdateNameViaParameter(object name)
		{
			Thread.CurrentThread.Name = (string)name;
		}

		[Test]
		public void EnqueueWorkerThreadNoArgs()
		{
			Assert.IsTrue(ThreadExtensions.EnqueueWorkerThread(() => ValueSettingTestAction()));
		}

		[Test]
		public void EnqueueWorkerThreadOneArg()
		{
			const int Num = 1;
			ThreadExtensions.EnqueueWorkerThread(Num, val => { ValueSettingTestAction(Num); });
		}

		[Test]
		public void EnqueueWorkerThreadTwoArgs()
		{
			const int Num1 = 1;
			const int Num2 = 2;
			ThreadExtensions.EnqueueWorkerThread(Num1, Num2,
				(val1, val2) => { ValueSettingTestAction(val1, val2); });
		}

		private static void ValueSettingTestAction(int val1 = 0, int val2 = 0)
		{
			testValue = 5 + val1 + val2;
			Assert.AreEqual(5 + val1 + val2, testValue);
		}

		private static int testValue;
	}
}