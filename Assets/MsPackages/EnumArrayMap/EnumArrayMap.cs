using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// 열거형을 키로 사용하여 배열의 성능과 딕셔너리의 편의성을 챙기는 커스텀 컬렉션 클래스
/// </summary>
public class EnumArrayMap<TKey, TValue>
    where TKey : unmanaged, Enum
{
    private readonly TValue[] _values;
    private readonly bool[] _hasValue;

    public EnumArrayMap(int length)
    {
        _values = new TValue[length];
        _hasValue = new bool[length];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndex(TKey key)
    {
        return Unsafe.As<TKey, int>(ref key);
    }

    public TValue this[TKey key]
    {
        get
        {
            int index = GetIndex(key);

            if (index < 0 || _hasValue.Length <= index)
            {
                throw new IndexOutOfRangeException("Out of Value of Enum");
            }

            // 데이터가 없는 경우의 처리
            if (_hasValue[index] == false)
            {
                throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");
            }

            return _values[index];
        }

        set
        {
            int index = GetIndex(key);

            if (index < 0 || _hasValue.Length <= index)
            {
                throw new IndexOutOfRangeException("Out of Value of Enum");
            }

            _values[index] = value;
            _hasValue[index] = true;
        }
    }

    public void Add(TKey key, TValue value)
    {
        int index = GetIndex(key);

        if (index < 0 || index >= _hasValue.Length)
        {
            throw new IndexOutOfRangeException("Out of Value of Enum");
        }

        // 이미 값이 존재하는지 체크
        if (_hasValue[index])
        {
            throw new ArgumentException($"An item with the same key has already been added. Key: {key}");
        }

        _values[index] = value;
        _hasValue[index] = true;
    }

    public bool Remove(TKey key)
    {
        int index = GetIndex(key);

        if (index < 0 || _hasValue.Length <= index || _hasValue[index] == false)
            return false;

        _hasValue[index] = false;
        _values[index] = default;
        return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = GetIndex(key);

        // 범위를 벗어났거나 값이 추가된 적이 없는 경우
        if (index < 0 || _hasValue.Length <= index || _hasValue[index] == false)
        {
            value = default;
            return false;
        }

        value = _values[index];
        return true;
    }

    public void Clear()
    {
        Array.Clear(_values, 0, _values.Length);
        Array.Clear(_hasValue, 0, _hasValue.Length);
    }
}