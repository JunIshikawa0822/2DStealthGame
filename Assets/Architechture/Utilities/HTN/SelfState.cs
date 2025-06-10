using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SelfState
{
    private Dictionary<string, (object Value, Type Type)> _states = new Dictionary<string, (object, Type)>();
    private HashSet<string> _changedStates = new HashSet<string>();
    public event Action<string> OnStateChanged;
    // 変更時刻を保存するdictionary　変更履歴にあたる
    private Dictionary<string, List<float>> _changeTimeHistories = new Dictionary<string, List<float>>();
    public bool EnableChangeTimeHistory { get; set; } = false;
    
    // 型安全にステートを設定
    public void SetState<T>(string key, T value)
    {
        if (_states.TryGetValue(key, out (object Value, Type Type) state))
        {
            if (state.Type != typeof(T))
            {
                Debug.LogWarning($"型が違うので上書きしません。key:{key}, value:{value}, type:{typeof(T).Name}");
                return;
            }
        }
        
        _states[key] = (value, typeof(T));
        _changedStates.Add(key);
        // 時刻履歴への記録（オプション）
        if (EnableChangeTimeHistory) 
        {
            if (!_changeTimeHistories.ContainsKey(key))
            {
                _changeTimeHistories[key] = new List<float>();
            }
            _changeTimeHistories[key].Add(Time.time);
        }
        // 値変更通知イベントを発行
        OnStateChanged?.Invoke(key);
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
    
    // 型安全な存在チェック
    public bool HasState<T>(string key)
    {
        return _states.TryGetValue(key, out var state) && state.Type == typeof(T);
    }
    
    // 型情報の取得
    public Type GetStateType(string key)
    {
        return _states.TryGetValue(key, out var state) ? state.Type : null;
    }
    
    // 型安全な値の取得（存在しない場合は例外を投げる）
    public T GetStateRequired<T>(string key)
    {
        if (!HasState<T>(key))
        {
            throw new KeyNotFoundException($"Required state '{key}' of type {typeof(T).Name} not found");
        }
        return GetState<T>(key);
    }

    // ステートを複製して新しい SelfState を返す
    public SelfState Clone() 
    {
        SelfState copy = new SelfState();
        // ステート辞書をコピー（浅いコピー）
        copy._states = new Dictionary<string, (object, Type)>(_states);
        // 差分キューは新規（クローン時点では未変更とする）
        copy._changedStates = new HashSet<string>();
        // 履歴設定をコピー（履歴データは保持しない場合は省略可能）
        copy.EnableChangeTimeHistory = this.EnableChangeTimeHistory;
        if (this.EnableChangeTimeHistory) 
        {
            // 履歴データをディープコピーする例
            copy._changeTimeHistories = new Dictionary<string, List<float>>();
            foreach (var kvp in _changeTimeHistories) 
            {
                copy._changeTimeHistories[kvp.Key] = new List<float>(kvp.Value);
            }
        }
        // OnStateChanged イベントはコピーせず、新規インスタンス用に空のまま
        return copy;
    }
}
