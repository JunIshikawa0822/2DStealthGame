using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldState
{
    private Dictionary<string, object> _states = new Dictionary<string, object>();
    private HashSet<string> _changedStates = new HashSet<string>();
    private event Action _onStateChanged;

    public void SetState(string key, object value)
    {
        bool changed = !_states.TryGetValue(key, out var oldValue) || !Equals(oldValue, value);
        _states[key] = value;
        
        if (changed)
        {
            _changedStates.Add(key);
            _onStateChanged?.Invoke();
        }
    }

    public T GetState<T>(string key, T defaultValue = default)
    {
        if (_states.TryGetValue(key, out object value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }
    
    public IEnumerable<string> GetAllKeys()
    {
        // 内部の_factsディクショナリからキーを返す
        return _states.Keys;
    }
    
    public WorldState Clone()
    {
        WorldState copy = new WorldState();
    
        foreach (var key in GetAllKeys())
        {
            var value = this.GetState<object>(key);
            // 値自体が参照型である場合、そのディープコピーも必要かもしれない
            copy.SetState(key, value);
        }
    
        return copy;
    }

    public bool HasFact(string key)
    {
        return _states.ContainsKey(key);
    }

    public HashSet<string> GetChangedFacts()
    {
        HashSet<string> result = new HashSet<string>(_changedStates);
        _changedStates.Clear();
        return result;
    }

    public void AddChangeListener(Action callback)
    {
        _onStateChanged += callback;
    }

    public void RemoveChangeListener(Action callback)
    {
        _onStateChanged -= callback;
    }
}
