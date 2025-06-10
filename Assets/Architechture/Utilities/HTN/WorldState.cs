using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WorldState
{
    private Dictionary<string, (object Value, Type Type)> _states = new Dictionary<string, (object, Type)>();
    private HashSet<string> _changedStates = new HashSet<string>();
    private event Action _onStateChanged;
    private Dictionary<string, List<float>> _changeTimeHistories = new Dictionary<string, List<float>>();
    public bool EnableChangeTimeHistory { get; set; } = false;

    public void SetState<T>(string key, T value)
    {
        // 型チェック
        if (_states.TryGetValue(key, out (object Value, Type Type) state))
        {
            if (state.Type != typeof(T))
            {
                Debug.LogWarning($"型が違うので上書きしません。key:{key}, value:{value}, type:{typeof(T).Name}");
                return;
            }
        }

        bool changed = !_states.TryGetValue(key, out var oldState) || !Equals(oldState.Value, value);
        _states[key] = (value, typeof(T));
        
        if (changed)
        {
            _changedStates.Add(key);
            if (EnableChangeTimeHistory)
            {
                if (!_changeTimeHistories.ContainsKey(key))
                {
                    _changeTimeHistories[key] = new List<float>();
                }
                _changeTimeHistories[key].Add(Time.time);
            }
            _onStateChanged?.Invoke();
        }
    }

    public T GetState<T>(string key, T defaultValue = default)
    {
        if (_states.TryGetValue(key, out var state))
        {
            if (state.Type == typeof(T))
            {
                return (T)state.Value;
            }
            Debug.LogWarning($"Type mismatch for key '{key}'. Expected {typeof(T).Name}, got {state.Type.Name}");
        }
        return defaultValue;
    }

    public bool TryGetState<T>(string key, out T value)
    {
        value = default;
        if (_states.TryGetValue(key, out var state))
        {
            if (state.Type == typeof(T))
            {
                value = (T)state.Value;
                return true;
            }
            Debug.LogWarning($"Type mismatch for key '{key}'. Expected {typeof(T).Name}, got {state.Type.Name}");
        }
        return false;
    }

    public bool HasState<T>(string key)
    {
        return _states.TryGetValue(key, out var state) && state.Type == typeof(T);
    }

    public Type GetStateType(string key)
    {
        return _states.TryGetValue(key, out var state) ? state.Type : null;
    }
    
    public IEnumerable<string> GetAllKeys()
    {
        return _states.Keys;
    }
    
    public WorldState Clone()
    {
        WorldState copy = new WorldState();
        copy._states = new Dictionary<string, (object, Type)>(_states);
        copy._changedStates = new HashSet<string>();
        copy.EnableChangeTimeHistory = this.EnableChangeTimeHistory;
        
        if (this.EnableChangeTimeHistory)
        {
            copy._changeTimeHistories = new Dictionary<string, List<float>>();
            foreach (var kvp in _changeTimeHistories)
            {
                copy._changeTimeHistories[kvp.Key] = new List<float>(kvp.Value);
            }
        }
        
        return copy;
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
