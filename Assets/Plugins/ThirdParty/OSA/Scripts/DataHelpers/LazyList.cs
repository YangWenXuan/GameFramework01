using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Com.TheFallenGames.OSA.DataHelpers
{
	/// <summary>
	/// <para>Very handy List implementation that delays object creation until it's accessed,</para>
	/// <para>although the underlying List is still allocated with all slots from the beginning, only that they have default values - default(T)</para>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class LazyList<T>
	{
		public T this[int key]
		{
			get
			{
				if (key >= Count)
					throw new ArgumentOutOfRangeException("key", key, "must be less than Count=" + Count);

				T res;
				if (!TryGet(key, out res))
				{
					res = _NewValueCreator(key);
					SetManuallyCreatedValueForFirstTime(key, res);
				}

				return res;
			}
			//private set { _BackingMap[key] = value; }
		}

		//public int Count { get { return _BackingList.Count; } }
		public int Count { get; private set; }

		/// <summary>
		/// Returns an enumerable version of this list, which only contains the already-cached values
		/// </summary>
		public EnumerableLazyList AsEnumerableForExistingItems { get { return new EnumerableLazyList(this); } }
		//public int InitializedCount { get; private set; }

		//public int VirtualCount
		//{
		//	get { return _VirtualCount; }
		//	private set
		//	{
		//		_VirtualCount = value;

		//		IndexOfFirstExistingValue = IndexOfLastExistingValue = -1;
		//	}
		//}
		//public int IndexOfFirstExistingValue { get; private set; }
		//public int IndexOfLastExistingValue { get; private set; }

		//public bool IsReadOnly { get { return false; } }


		//IDictionary<int, TValue> BackingDictionaryAsInterface { get { return (_BackingDictionary as IDictionary<int, TValue>); } }

		//List<T> _BackingList = new List<T>();
		Dictionary<int, T> _BackingMap;
		List<int> _Keys = new List<int>(); // todo maybe sort these
		Func<int, T> _NewValueCreator;
		//int _VirtualCount;


		public LazyList(Func<int, T> newValueCreator, int initialCount)
		{
			initialCount = initialCount > 0 ? initialCount : 0;
			_NewValueCreator = newValueCreator;

			_BackingMap = new Dictionary<int, T>(initialCount);

			InitWithNewCount(initialCount);
		}


		public void InitWithNewCount(int newCount)
		{
			Clear();
			int cap = Math.Min(newCount, 1024);
			_BackingMap = new Dictionary<int, T>(cap);
			_Keys = new List<int>(cap);
			Count = newCount;
		}
		public void Add(int count)
		{
			Count += count;
		}
		public void InsertAtStart(int count) { InsertWhenKeyStartIdxKnown(0, 0, count); }
		public void Insert(int index, int count)
		{
			int keyStartIdx = _Keys.BinarySearch(index);
			if (keyStartIdx < 0)
				keyStartIdx = ~keyStartIdx; // the index of the first potentially-existing key to be shifted to the right
			InsertWhenKeyStartIdxKnown(keyStartIdx, index, count);
		}
		void InsertWhenKeyStartIdxKnown(int keyStartIdx, int index, int count)
		{
			int keyStartIdxExcl = keyStartIdx - 1;

			int key, newKey;
			// Going from end to start, to not overwrite dictionary existing items when shifting values
			for (int i = _Keys.Count - 1; i > keyStartIdxExcl; --i)
			{
				key = _Keys[i];
				newKey = key + count;

				_Keys[i] = newKey;
				_BackingMap[newKey] = _BackingMap[key];
				_BackingMap.Remove(key);
			}

			Count += count;
		}
		public void Clear()
		{
			_BackingMap.Clear();
			_Keys.Clear();
			Count = 0;
		}
		[Obsolete("Use Remove(int index, int count) below")]
		public void RemoveAt(int index) { Remove(index, 1); }
		/// <summary>Returns the index, if found</summary>
		public int Remove(T value)
		{
			foreach (var kv in _BackingMap)
				if (kv.Value.Equals(value))
				{
					Remove(kv.Key, 1);
					return kv.Key;
				}

			return -1;
		}
		public void Remove(int index, int count)
		{
			int lastKeyToRemoveExcl = index + count;
			int keyStartIndex = _Keys.BinarySearch(index);
			if (keyStartIndex < 0)
				keyStartIndex = ~keyStartIndex; // the index of the first potentially-existing key to be removed

			int key, newKey;
			// Remove the matching keys, if existing
			int i = keyStartIndex;
			while (i < _Keys.Count)
			{
				key = _Keys[i];

				if (key < lastKeyToRemoveExcl)
				{
					_Keys.RemoveAt(i);
					_BackingMap.Remove(key);
				}
				else
					break;
			}

			// Decrement the following keys, if existing
			for (; i < _Keys.Count; ++i)
			{
				key = _Keys[i];
				newKey = key - count;

				_Keys[i] = newKey;
				_BackingMap[newKey] = _BackingMap[key];
				_BackingMap.Remove(key);
			}

			Count -= count;
		}
		IEnumerator<T> GetEnumeratorForExistingItems()
		{
			for (int i = 0; i < _Keys.Count; ++i)
			{
				var v = _BackingMap[_Keys[i]];
				if (v != null)
					yield return v;
			}
			yield break;
		}

		public bool TryGet(int index, out T val)
		{
			return _BackingMap.TryGetValue(index, out val);
		}

		public void SetOrUpdateManuallyCreatedValue(int index, T val)
		{
			if (_BackingMap.ContainsKey(index))
				_BackingMap[index] = val;
			else
				SetManuallyCreatedValueForFirstTime(index, val);
		}

		void SetManuallyCreatedValueForFirstTime(int index, T val)
		{
			_BackingMap.Add(index, val);
			_Keys.Insert(~_Keys.BinarySearch(index), index);
		}


		public class EnumerableLazyList : IEnumerable<T>
		{
			LazyList<T> _LazyList;


			public EnumerableLazyList(LazyList<T> lazyList) { _LazyList = lazyList; }

			public IEnumerator<T> GetEnumerator() { return _LazyList.GetEnumeratorForExistingItems(); }

			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		}


		//class RefInt// : IEquatable<RefInt>
		//{
		//	public int value;

		//	public override int GetHashCode() { return value.GetHashCode(); }
		//	public override bool Equals(object obj)
		//	{
		//		var refInt = obj as RefInt;
		//		if (refInt == null)
		//			return false;

		//		return value == refInt.value;
		//	}

		//	//public static bool operator ==(RefInt val, int val2) { return val.value == val2; }
		//	//public static bool operator !=(RefInt val, int val2) { return val.value != val2; }
		//	//public static bool operator ==(int val2, RefInt val) { return val.value == val2; }
		//	//public static bool operator !=(int val2, RefInt val) { return val.value != val2; }

		//	//public static implicit operator int(RefInt val) { return val.value; }
		//	//public static implicit operator RefInt(int val) { return new RefInt() { value = val }; }

		//	public override string ToString() { return value + ""; }

		//	//public bool Equals(RefInt other) { return value == other.value; }
		//}

		//public void Add(int key, TValue value) { _BackingDictionary.Add(key, value); }
		//public void Add(KeyValuePair<int, TValue> item) { BackingDictionaryAsInterface.Add(item); }
		//public bool Contains(KeyValuePair<int, TValue> item) { return _BackingDictionary.Contains(item); }
		//public bool ContainsKey(int key) { return key >= FirstIndex && key <= LastIndex; }
		//public void CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex) { BackingDictionaryAsInterface.CopyTo(array, arrayIndex); }
		//public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator() { return _BackingDictionary.GetEnumerator(); }
		//public bool Remove(int key) { return if (_BackingList.Remove(key); }
		//public bool Remove(KeyValuePair<int, TValue> item) { return BackingDictionaryAsInterface.Remove(item); }
		//public bool TryGetValue(int key, out TValue value) { return _BackingDictionary.TryGetValue(key, out value); }
		//IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }



		//class MyLinkedList
		//{
		//	public MyLinkedListNode First
		//	{
		//		get
		//		{
		//			if (_First == null)
		//			{
		//				if (Length == 0)
		//					return null;

		//			}
		//		}
		//	}
		//	public int Length { get; private set; }

		//	MyLinkedListNode _First;


		//	public void InitWithNewCount(int newCount) { Length = newCount; }
		//	public void Add(int count) { _BackingList.AddRange(Array.CreateInstance(typeof(T), count) as T[]); }
		//	public void InsertAtStart(int count) { _BackingList.InsertRange(0, Array.CreateInstance(typeof(T), count) as T[]); }
		//	public void Insert(int index, int count) { _BackingList.InsertRange(index, Array.CreateInstance(typeof(T), count) as T[]); }
		//	public void Clear() { _BackingList.Clear(); }
		//	public void Remove(T value) { _BackingList.Remove(value); }
		//	public void RemoveAt(int index) { _BackingList.RemoveAt(index); }
		//}

		//class MyLinkedListNode
		//{
		//	public int value;
		//	public MyLinkedListNode next;
		//}
	}
}
