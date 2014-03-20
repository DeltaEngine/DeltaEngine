using System.Collections.Generic;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	public class ChangeableListTests
	{
		[SetUp]
		public void SetUp()
		{
			list = new ChangeableList<int> { 1, 3, 5 };
		}

		private ChangeableList<int> list;

		[Test]
		public void AddAndRemoveWhileEnumerating()
		{
			foreach (int num in list)
				if (num == 5)
					list.Add(7);
				else if (num > 1)
					list.Remove(num);
			Assert.AreEqual("1, 5, 7", list.ToText());
		}

		[Test]
		public void AddRangeOfElementsWhileEnumerating()
		{
			foreach (int num in list)
				if (num == 5)
					list.AddRange(new[] { 2, 2, 2 });
			Assert.IsTrue(list.Contains(2));
			Assert.IsFalse(list.Contains(4));
			Assert.AreEqual(6, list.Count);
		}

		[Test]
		public void AddElementWhileEnumeratingInInnerLoop()
		{
			foreach (int num in list)
			{
				if (num == 5)
					foreach (int num2 in list)
						if (num2 == 1)
							list.Add(10);
				Assert.AreEqual(3, list.Count);
			}
			Assert.AreEqual(4, list.Count);
			list.Remove(10);
			Assert.AreEqual(3, list.Count);
		}

		[Test]
		public void TestCloningChangeableList()
		{
			foreach (int num1 in list)
			{
				Assert.AreEqual(1, num1);
				list.Add(1);
				var testList2 = new ChangeableList<int>(list);
				foreach (int num2 in testList2)
				{
					Assert.AreEqual(1, num2);
					testList2.Add(2);
					// The lists should be different here (testList2 is cloned)
					Assert.False(list == testList2);
					// But the data in it should be still equal.
					Assert.AreEqual(list.ToText(), testList2.ToText());
					break;
				}
				break;
			}
		}

		[Test]
		public void GetEmulatorAndResetAndClearIt()
		{
			Assert.IsFalse(list.IsReadOnly);
			var emulator = list.GetEnumerator();
			Assert.AreEqual(emulator.Current, 0);
			emulator.MoveNext();
			emulator.MoveNext();
			emulator.MoveNext();
			emulator.MoveNext();
			emulator.Reset();
			list.Clear();
		}

		[Test]
		public void ConvertChangebleListToArray()
		{
			var array = list.ToArray();
			Assert.AreEqual(new[] { 1, 3, 5 }, array);
		}

		[Test]
		public void RemoveItemFromList()
		{
			list.RemoveAt(1);
			Assert.AreEqual(new List<int> { 1, 5 }, list);
		}

		[Test]
		public void RemoveItemFromListFromEnumerationDepth()
		{
			list.Remove(5);
			var emulator = list.GetEnumerator();
			emulator.MoveNext();
			emulator.MoveNext();
			emulator.MoveNext();
			emulator.Reset();
			list.RemoveAt(1);
			Assert.AreEqual(new List<int> { 1, 3 }, list);
		}

		[Test]
		public void RemoveItemImmediatelyAfterAddingWhileEnumerating()
		{
			foreach (int num in list)
			{
				list.Add(9);
				list.Remove(9);
			}
			Assert.IsFalse(list.Contains(9));
		}
	}
}