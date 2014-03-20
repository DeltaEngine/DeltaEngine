using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Extensions;
using NUnit.Framework;
using System.Collections;

namespace DeltaEngine.Tests.Extensions
{
	public class UniqueListTests
	{
		[Test]
		public void TestUniqueList()
		{
			var list = new UniqueList<int> { 2, 3, 4, 2 };
			Assert.AreEqual(3, list.Count);
			Assert.AreEqual(2, list[0]);
			Assert.AreEqual("2, 3, 4", list.ToText());
			list.Remove(2);
			list.Remove(5);
			Assert.AreEqual("3, 4", list.ToText());
		}

		[Test]
		public void ToArray()
		{
			var list = new UniqueList<int> { 3, 4 };
			int[] intList = list.ToArray();
			Assert.AreEqual(3, intList[0]);
			Assert.AreEqual(4, intList[1]);
			Assert.AreEqual(2, intList.Length);
		}

		[Test]
		public void GetInfoFromUniqueList()
		{
			var list = new UniqueList<int> { 2, 9, 5, 3, 0, 1 };
			Assert.AreEqual(false, list.IsReadOnly);
			Assert.AreEqual(true, list.Contains(9));
			Assert.AreEqual(2, list.IndexOf(5));
		}

		[Test]
		public void OrderUniqueList()
		{
			var orderlist1 = new UniqueList<int> { 2, 9, 5, 3, 0, 1 };
			var orderlist2 = new UniqueList<int> { 1, 8, 4, 2, 9, 7 };
			var myComparer = new NumberComparer();
			orderlist1.Sort();
			orderlist2.Sort(myComparer);
			Assert.AreEqual(2, orderlist1[2]);
			Assert.AreEqual(4, orderlist2[2]);
			orderlist1.Clear();
			Assert.AreEqual(0, orderlist1.Count);
		}

		public class NumberComparer : Comparer<int>
		{
			public override int Compare(int x, int y)
			{
				return x < y ? -1 : x > y ? 1 : 0;
			}
		}

		[Test]
		public void MixingUniqueList()
		{
			int[] list = { 4, 5, 6, 7, 8, 9 };
			var uniquelist = new UniqueList<int>();
			uniquelist.Insert(0, 3);
			uniquelist.Insert(0, 2);
			uniquelist.Insert(0, 1);
			uniquelist.AddRange(list);
			Assert.AreEqual(1, uniquelist[0]);
			Assert.AreEqual(2, uniquelist[1]);
			Assert.AreEqual(3, uniquelist[2]);
			uniquelist.RemoveAt(0);
			uniquelist.RemoveAt(0);
			uniquelist.RemoveAt(0);
			Assert.AreEqual(4, uniquelist[0]);
		}

		[Test]
		public void InsertRemoveFromUniqueList()
		{
			int[] list = { 4, 5, 6 };
			var uniquelist = new UniqueList<int>();
			uniquelist.Insert(0, 3);
			uniquelist.Insert(0, 2);
			uniquelist.Insert(0, 1);
			uniquelist.AddRange(list);
			Assert.AreEqual(1, uniquelist[0]);
			Assert.AreEqual(2, uniquelist[1]);
			Assert.AreEqual(3, uniquelist[2]);
			uniquelist.RemoveAt(0);
			uniquelist.RemoveAt(0);
			uniquelist.RemoveAt(0);
			Assert.AreEqual(4, uniquelist[0]);
		}

		[Test]
		public void CopyAndAdaptUniqueList()
		{
			int[] list = { 0, 0, 0 };
			var copylist = new UniqueList<int> { 4, 5, 6 };
			var uniquelist = new UniqueList<int> { 1, 2, 3 };
			uniquelist.CopyTo(list, 0);
			int i = 0;
			foreach (object p in copylist) {
				uniquelist[i] = copylist.ToList()[i];
				++i;
			}
			Assert.AreEqual(4, uniquelist[0]);
			Assert.AreEqual(5, uniquelist[1]);
			Assert.AreEqual(6, uniquelist[2]);
			Assert.AreEqual(1, list[0]);
			Assert.AreEqual(2, list[1]);
			Assert.AreEqual(3, list[2]);
		}

		[Test]
		public void EmptyUniqueList()
		{
			var list = new UniqueList<int> { 1, 2, 3 };
			IEnumerable weak = list.AsWeakEnumerable();
			var sequence = weak.Cast<int>().Take(3).ToArray();
			CollectionAssert.AreEqual(sequence,
				new[] { 1, 2, 3 });
		}

		[Test]
		public void TestConstructorWithDuplicatesToBeRemoved()
		{
			var listWithDuplicates = new List<int> { 3, 5, 7, 3 };
			var copiedUniqueList = new UniqueList<int>(listWithDuplicates);
			Assert.AreEqual(listWithDuplicates.Count, 4);
			Assert.AreEqual(copiedUniqueList.Count, 3);
			Assert.AreEqual(copiedUniqueList[0], 3);
			Assert.AreEqual(copiedUniqueList[1], 5);
			Assert.AreEqual(copiedUniqueList[2], 7);
		}

		[Test]
		public void PredicateExistsInUniqueList()
		{
			var list = new UniqueList<int>() { 1, 5, 9, 7, 3 };
			Predicate<int> pre = x => x == 5;
			Assert.IsTrue(list.Exists(pre));
		}

		[Test]
		public void FindPredicateInUniqueList()
		{
			var list = new UniqueList<int>() { 7, 5, 3, 9, 1, 4 };
			Predicate<int> pre = x => x == 4;
			Assert.AreEqual(list.Find(pre), 4);
		}

		[Test]
		public void RemoveAllMatchedItemsInUniqueList()
		{
			var list = new UniqueList<int>() { 3, 6, 9, 8, 4, 1, 2 };
			Predicate<int> pre = x => x == 3;
			list.RemoveAll(pre);
			Assert.AreEqual(list.Count, 6);
		}

		[Test]
		public void FindAllPredicatesInUniqueList()
		{
			var list = new UniqueList<int>() { 1, 4, 7, 5, 2, 6 };
			Predicate<int> pre = x => x == 1;
			var result = list.FindAll(pre);
			Assert.AreEqual(result.Count, 1);
		}
	}

	public static class TestEnumerable
	{
		public static IEnumerable AsWeakEnumerable(this IEnumerable source)
		{
			foreach (object o in source)
			{
				yield return o;
			}
		} //ncrunch: no coverage
	}
}