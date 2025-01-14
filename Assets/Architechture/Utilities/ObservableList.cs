using System;
using System.Collections.Generic;
public class ObservableList<T>
{
    private List<T> _List;

    public event Action<int, T> OnValueChanged;
    public event Action OnArrayCleared;

    public ObservableList(int size)
    {
        _List = new List<T>();
    }

    public T this[int index]
    {
        get => _List[index];
        set
        {
            _List[index] = value;
            OnValueChanged?.Invoke(index, value); // 値が変更されたことを通知
        }
    }

    public void Clear()
    {
        _List.Clear();
        OnArrayCleared?.Invoke();
    }

    public int Count => _List.Count;
    public List<T> Value => _List;
}
