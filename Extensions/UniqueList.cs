using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Based on <see cref="List{T}"/>, but this also checks if elements do already exist. It will
	/// not add duplicates, they are ignored and no exception is thrown! Each element is unique.
	/// Unlike <see cref="HashSet{T}"/>, which behaves similarly this one preserves the adding order.
	/// </summary>
	public class UniqueList<T> : IList<T>
	{
		public UniqueList() {}

		public UniqueList(IEnumerable<T> copyFrom)
		{
			AddRange(copyFrom);
		}

		public T this[int index]
		{
			get { return data[index]; }
			set { data[index] = value; }
		}

		private readonly List<T> data = new List<T>();

		public int Count
		{
			get { return data.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void Add(T item)
		{
			if (data.IndexOf(item) < 0)
				data.Add(item);
		}

		public void Clear()
		{
			data.Clear();
		}

		public bool Contains(T item)
		{
			return data.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			data.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			return data.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return data.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return data.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (data.IndexOf(item) < 0)
				data.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			data.RemoveAt(index);
		}

		public void AddRange(IEnumerable<T> c)
		{
			foreach (T obj in c)
				Add(obj);
		}

		public void Sort(IComparer<T> comparer)
		{
			data.Sort(comparer);
		}

		public void Sort()
		{
			data.Sort();
		}

		public T[] ToArray()
		{
			return data.ToArray();
		}

		public List<T> ToList()
		{
			return data.ToList();
		}

		public bool Exists(Predicate<T> match)
		{
			return data.Exists(match);
		}

		public T Find(Predicate<T> match)
		{
			return data.Find(match);
		}

		public void RemoveAll(Predicate<T> match)
		{
			data.RemoveAll(match);
		}

		public List<T> FindAll(Predicate<T> match)
		{
			return data.FindAll(match);
		}
	}
}