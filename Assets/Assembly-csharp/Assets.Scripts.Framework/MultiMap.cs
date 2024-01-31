using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Framework
{
	public class MultiMap<TKey, TValue> : Dictionary<TKey, List<TValue>>
	{
		public struct Iterator
		{
			private Dictionary<TKey, List<TValue>>.Enumerator m_outerEnumerator;

			private int m_valIndex;

			private MultiMap<TKey, TValue> m_multiMap;

			private bool m_bEnd;

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					KeyValuePair<TKey, List<TValue>> current = this.m_outerEnumerator.get_Current();
					TKey key = current.get_Key();
					KeyValuePair<TKey, List<TValue>> current2 = this.m_outerEnumerator.get_Current();
					List<TValue> value = current2.get_Value();
					if (this.m_valIndex < 0 || this.m_valIndex >= value.get_Count())
					{
						DebugHelper.Assert(false);
					}
					TValue tValue = value.get_Item(this.m_valIndex);
					KeyValuePair<TKey, TValue> result = new KeyValuePair<TKey, TValue>(key, tValue);
					return result;
				}
			}

			public Iterator(MultiMap<TKey, TValue> inMultiMap)
			{
				this.m_outerEnumerator = inMultiMap.GetEnumerator();
				this.m_valIndex = -1;
				this.m_multiMap = inMultiMap;
				this.m_bEnd = false;
			}

			public bool MoveNext()
			{
				if (this.m_bEnd)
				{
					return false;
				}
				if (this.m_valIndex < 0 && !this.m_outerEnumerator.MoveNext())
				{
					this.m_bEnd = true;
					return false;
				}
				this.m_valIndex++;
				int arg_5E_0 = this.m_valIndex;
				KeyValuePair<TKey, List<TValue>> current = this.m_outerEnumerator.get_Current();
				if (arg_5E_0 >= current.get_Value().get_Count())
				{
					if (!this.m_outerEnumerator.MoveNext())
					{
						this.m_bEnd = true;
						return false;
					}
					this.m_valIndex = 0;
				}
				return true;
			}

			public void Reset()
			{
				this.m_outerEnumerator = this.m_multiMap.GetEnumerator();
				this.m_valIndex = -1;
				this.m_bEnd = false;
			}

			public bool IsEnd()
			{
				return this.m_bEnd;
			}
		}

		public void Add(TKey key, TValue value)
		{
			List<TValue> list = null;
			if (!this.TryGetValue(key, ref list))
			{
				list = new List<TValue>();
				base.Add(key, list);
			}
			list.Add(value);
		}

		public List<TValue> GetValues(TKey key, bool returnEmptySet = true)
		{
			List<TValue> result = null;
			if (!base.TryGetValue(key, ref result) && returnEmptySet)
			{
				result = new List<TValue>();
			}
			return result;
		}

		public TValue[] GetValuesAll()
		{
			List<TValue> list = new List<TValue>();
			Dictionary<TKey, List<TValue>>.Enumerator enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<TKey, List<TValue>> current = enumerator.get_Current();
				List<TValue> value = current.get_Value();
				if (value != null)
				{
					IEnumerator enumerator2 = value.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						TValue tValue = (TValue)((object)enumerator2.get_Current());
						list.Add(tValue);
					}
				}
			}
			return list.ToArray();
		}

		public int GetCountAll()
		{
			int num = 0;
			Dictionary<TKey, List<TValue>>.Enumerator enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<TKey, List<TValue>> current = enumerator.get_Current();
				List<TValue> value = current.get_Value();
				if (value != null)
				{
					num += value.get_Count();
				}
			}
			return num;
		}

		public MultiMap<TKey, TValue>.Iterator GetIterator()
		{
			return new MultiMap<TKey, TValue>.Iterator(this);
		}
	}
}
