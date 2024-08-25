
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[Serializable]
	public class Pair
	{
		public TKey key = default;
		public TValue value = default;

		public Pair(TKey key, TValue value)
		{
			this.key = key;
			this.value = value;
		}
	}

	[SerializeField]
	private List<Pair> _list = null;

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		Clear();
		foreach (Pair pair in _list)
		{
			if (ContainsKey(pair.key))
			{
				continue;
			}
			Add(pair.key, pair.value);
		}
	}
    
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		// 処理なし
	}
}
