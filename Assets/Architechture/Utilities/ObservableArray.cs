using System;
public class ObservableArray<T>
{
    private T[] _array;

    public event Action<int, T> OnValueChanged;
    public event Action ValueChangeEvent;
    public event Action<T> OnArrayCleared;

    public ObservableArray(int size)
    {
        _array = new T[size];
    }

    public T this[int index]
    {
        get => _array[index];
        set
        {
            _array[index] = value;
            OnValueChanged?.Invoke(index, value); // 値が変更されたことを通知
            ValueChangeEvent?.Invoke();
        }
    }

    public void Clear()
    {
        for(int i = 0; i < _array.Length; i++)
        {
            _array[i] = default(T);
        }
    }

    public void Clear(int index, int length)
    {
        for(int i = index; i < index + length; i++)
        {
            _array[i] = default(T);
        }
    }

    public int Length => _array.Length;

    public T[] Value => _array;
}
