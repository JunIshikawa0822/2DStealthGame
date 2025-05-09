using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SelfState
{
    private Dictionary<string, object> _states = new Dictionary<string, object>();
    private HashSet<string> _changedStates = new HashSet<string>();
    public event Action<string> OnStateChanged;
    
    // 変更時刻を保存するdictionary　変更履歴にあたる
    private Dictionary<string, List<float>> _changeTimeHistories = new Dictionary<string, List<float>>();
    public bool EnableChangeTimeHistory { get; set; } = false;
    
    // 型安全にステートを設定
    public void SetState<T>(string key, T value)
    {
        //いまの状態に値が存在するか
        bool isExists = _states.ContainsKey(key);
        
        //存在していない場合、および、値が同じでない場合に変更可能フラグをオン
        bool isChange = !isExists || !EqualityComparer<T>.Default.Equals((T)_states[key], value);
        
        if (isChange)
        {
            //存在すれば上書き、存在しなければ追加、の書き方
            _states[key] = value;
            // 差分キーに登録
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
    }
    
    public T GetState<T>(string key) 
    {
        if (_states.TryGetValue(key, out object objectValue)) 
        {
            if (objectValue is T item) 
            {
                return item;
            } 
            else 
            {
                throw new InvalidCastException($"「{key}」というアイテムは {typeof(T).Name}という型を含んでいません");
            }
        }
        return default;
    }

    // ステートを複製して新しい SelfState を返す
    public SelfState Clone() 
    {
        SelfState copy = new SelfState();
        // ステート辞書をコピー（浅いコピー）
        copy._states = new Dictionary<string, object>(_states);
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
