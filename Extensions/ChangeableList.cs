using System.Collections;
using System.Collections.Generic;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Like <see cref="List{T}"/>, but allows you to add and remove elements while enumerating. When
	/// all enumerations are complete, all remembered removed and added elements will be applied.
	/// Use when adding and removing is rare, otherwise just create a new list before enumerating.
	/// </summary>	
	public sealed class ChangeableList<T> : ICollection<T>
	{
		public ChangeableList()
		{
			data = new List<T>();
		}

		private readonly List<T> data;

		public ChangeableList(IEnumerable<T> copyFrom)
		{
			data = new List<T>(copyFrom);
		}

		public T this[int index]
		{
			get { return data[index]; }
			set { data[index] = value; } //ncrunch: no coverage 
		}

		public int Count
		{
			get { return data.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Remembered elements to be added and removed when finished enumerating. If we are not
		/// enumerating, adding and removing is done directly (this list is empty then).
		/// </summary>
		public void Add(T item)
		{
			if (enumerationDepth > 0)
				elementsToBeAdded.Add(item);
			else
				data.Add(item);
		}

		/// <summary>
		/// This value will be greater than 0 if currently in any enumeration!
		/// </summary>
		private int enumerationDepth;
		private readonly List<T> elementsToBeAdded = new List<T>();

		/// <summary>
		/// Remove element. If we are currently enumerating, we will remember elements to remove later.
		/// </summary>
		public bool Remove(T item)
		{
			if (enumerationDepth > 0)
			{
				int index = IndexOf(item);
				if (index >= 0)
					elementsToBeRemoved.Add(item);
				else
					return elementsToBeAdded.Remove(item);
				return index >= 0;
			}
			return data.Remove(item);
		}

		private readonly List<T> elementsToBeRemoved = new List<T>();

		public void Clear()
		{
			if (enumerationDepth > 0)
				elementsToBeRemoved.AddRange(data);
			else
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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ChangeableEnumerator(this);
		}

		private class ChangeableEnumerator : IEnumerator<T>
		{
			public ChangeableEnumerator(ChangeableList<T> setList)
			{
				list = setList;
				if (enumerationStarted == false)
					list.enumerationDepth++;
				enumerationStarted = true;
			}

			private readonly ChangeableList<T> list;
			private bool enumerationStarted;

			public T Current
			{
				get { return index < 0 || index >= list.Count ? default(T) : list[index]; }
			}

			private int index = -1;
			object IEnumerator.Current
			{
				get { return Current; }
			}

			public void Dispose()
			{
				if (enumerationStarted)
					list.ReduceEnumerationCount();
				enumerationStarted = false;
			}

			public bool MoveNext()
			{
				index++;
				if (index < list.Count)
					return true;
				if (enumerationStarted)
					list.ReduceEnumerationCount();
				enumerationStarted = false;
				return false;
			}

			public void Reset()
			{
				index = -1;
				if (enumerationStarted == false)
					list.enumerationDepth++;
				enumerationStarted = true;
			}
		}

		public int IndexOf(T item)
		{
			return data.IndexOf(item);
		}

		public void RemoveAt(int index)
		{
			if (enumerationDepth > 0)
			{
				if (index >= 0 && index < data.Count)
					elementsToBeRemoved.Add(data[index]);
			}
			else
				data.RemoveAt(index);
		}

		public void AddRange(IEnumerable<T> c)
		{
			foreach (T obj in c)
				Add(obj);
		}

		public T[] ToArray()
		{
			return data.ToArray();
		}

		/// <summary>
		/// Reduces the enumerationCount and when reaching 0 again all the remembered elements to be
		/// added and removed will be added and removed. This function is called from the enumerator.
		/// </summary>
		private void ReduceEnumerationCount()
		{
			enumerationDepth--;
			if (enumerationDepth != 0 || elementsToBeAdded.Count == 0 && elementsToBeRemoved.Count == 0)
				return;
			foreach (T obj in elementsToBeAdded)
				data.Add(obj);
			elementsToBeAdded.Clear();
			foreach (T obj in elementsToBeRemoved)
				data.Remove(obj);
			elementsToBeRemoved.Clear();
		}
	}
}