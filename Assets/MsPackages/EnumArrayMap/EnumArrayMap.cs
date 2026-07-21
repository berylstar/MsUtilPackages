using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// 0 이상의 Int32 기반 열거형 값을 인덱스로 사용하여 값을 배열에 저장하는 컬렉션입니다.
/// </summary>
public sealed class EnumArrayMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : unmanaged, Enum
{
    private const int MaximumCapacity = 1_048_576;

    private readonly TValue[] _values;
    private readonly bool[] _hasValue;
    private int _version;

    /// <summary>
    /// 열거형에 선언된 값으로 용량을 계산하여 맵을 초기화합니다.
    /// Flags, 음수, 지나치게 희소하거나 큰 열거형은 지원하지 않습니다.
    /// </summary>
    public EnumArrayMap()
    {
        int capacity = GetValidatedCapacity();

        _values = new TValue[capacity];
        _hasValue = new bool[capacity];
    }

    /// <summary>
    /// 저장된 키-값 쌍의 개수를 가져옵니다.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// 사용 가능한 배열 슬롯의 개수를 가져옵니다.
    /// </summary>
    public int Capacity => _values.Length;

    public TValue this[TKey key]
    {
        get
        {
            int index = GetValidatedIndex(key);

            if (_hasValue[index] == false)
            {
                throw new KeyNotFoundException($"지정한 키 '{key}'가 맵에 존재하지 않습니다.");
            }

            return _values[index];
        }

        set
        {
            int index = GetValidatedIndex(key);

            if (_hasValue[index] == false)
            {
                _hasValue[index] = true;
                Count++;
            }

            _values[index] = value;
            _version++;
        }
    }

    public void Add(TKey key, TValue value)
    {
        int index = GetValidatedIndex(key);

        if (_hasValue[index])
        {
            throw new ArgumentException($"동일한 키를 가진 항목이 이미 추가되어 있습니다. 키: {key}", nameof(key));
        }

        AddAt(index, value);
    }

    public bool TryAdd(TKey key, TValue value)
    {
        int index = GetValidatedIndex(key);

        if (_hasValue[index])
        {
            return false;
        }

        AddAt(index, value);
        return true;
    }

    public bool ContainsKey(TKey key)
    {
        return TryGetIndex(key, out int index) && _hasValue[index];
    }

    public bool Remove(TKey key)
    {
        if (TryGetIndex(key, out int index) == false || _hasValue[index] == false)
        {
            return false;
        }

        _hasValue[index] = false;
        _values[index] = default;
        Count--;
        _version++;
        return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (TryGetIndex(key, out int index) == false || _hasValue[index] == false)
        {
            value = default;
            return false;
        }

        value = _values[index];
        return true;
    }

    public void Clear()
    {
        if (Count == 0)
        {
            return;
        }

        Array.Clear(_values, 0, _values.Length);
        Array.Clear(_hasValue, 0, _hasValue.Length);
        Count = 0;
        _version++;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        int version = _version;

        for (int index = 0; index < _hasValue.Length; index++)
        {
            ThrowIfCollectionChanged(version);

            if (_hasValue[index])
            {
                yield return new KeyValuePair<TKey, TValue>(GetKey(index), _values[index]);
            }
        }

        ThrowIfCollectionChanged(version);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void AddAt(int index, TValue value)
    {
        _values[index] = value;
        _hasValue[index] = true;
        Count++;
        _version++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetIndex(TKey key)
    {
        return EnumArrayMapKeyInfo<TKey>.GetIndex(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TKey GetKey(int index)
    {
        return EnumArrayMapKeyInfo<TKey>.GetKey(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryGetIndex(TKey key, out int index)
    {
        index = GetIndex(key);
        return (uint)index < (uint)_values.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetValidatedIndex(TKey key)
    {
        int index = GetIndex(key);

        if ((uint)index >= (uint)_values.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(key),
                key,
                $"열거형 값은 0 이상 {_values.Length - 1} 이하여야 합니다.");
        }

        return index;
    }

    private void ThrowIfCollectionChanged(int version)
    {
        if (version != _version)
        {
            throw new InvalidOperationException("열거 작업 중에 컬렉션이 수정되었습니다.");
        }
    }

    private static int GetValidatedCapacity()
    {
        ValidateEnumType();

        if (EnumArrayMapKeyInfo<TKey>.RequiredCapacity > MaximumCapacity)
        {
            throw new NotSupportedException(
                $"열거형 '{typeof(TKey).Name}'에는 {EnumArrayMapKeyInfo<TKey>.RequiredCapacity}개의 슬롯이 필요합니다. 딕셔너리 기반 컬렉션을 사용하세요.");
        }

        if (EnumArrayMapKeyInfo<TKey>.IsHighlySparse)
        {
            throw new NotSupportedException(
                $"열거형 '{typeof(TKey).Name}'은(는) 값이 지나치게 희소합니다. " +
                "딕셔너리 기반 컬렉션을 사용하세요.");
        }

        return (int)EnumArrayMapKeyInfo<TKey>.RequiredCapacity;
    }

    private static void ValidateEnumType()
    {
        if (EnumArrayMapKeyInfo<TKey>.HasInt32UnderlyingType == false)
        {
            throw new NotSupportedException(
                $"EnumArrayMap은 Int32 기반 열거형만 지원합니다. '{typeof(TKey).Name}'의 기반형은 '{EnumArrayMapKeyInfo<TKey>.UnderlyingType.Name}'입니다.");
        }

        if (EnumArrayMapKeyInfo<TKey>.IsFlags)
        {
            throw new NotSupportedException($"Flags 열거형 '{typeof(TKey).Name}'은(는) 지원하지 않습니다.");
        }

        if (EnumArrayMapKeyInfo<TKey>.HasNegativeValue)
        {
            throw new NotSupportedException($"열거형 '{typeof(TKey).Name}'에 음수 값이 포함되어 있어 지원하지 않습니다.");
        }
    }
}

internal static class EnumArrayMapKeyInfo<TKey> where TKey : unmanaged, Enum
{
    private const int SparsityCheckThreshold = 256;
    private const int MaximumAutomaticSparsityRatio = 8;

    public static readonly Type UnderlyingType;
    public static readonly bool HasInt32UnderlyingType;
    public static readonly bool IsFlags;
    public static readonly bool HasNegativeValue;
    public static readonly long RequiredCapacity;
    public static readonly bool IsHighlySparse;

    static EnumArrayMapKeyInfo()
    {
        UnderlyingType = Enum.GetUnderlyingType(typeof(TKey));
        HasInt32UnderlyingType = UnderlyingType == typeof(int);
        IsFlags = typeof(TKey).IsDefined(typeof(FlagsAttribute), false);

        if (HasInt32UnderlyingType == false)
        {
            return;
        }

        Array values = Enum.GetValues(typeof(TKey));
        var uniqueNonNegativeValues = new HashSet<int>();
        int maximumValue = -1;

        foreach (TKey value in values)
        {
            int numericValue = GetIndex(value);

            if (numericValue < 0)
            {
                HasNegativeValue = true;
                continue;
            }

            uniqueNonNegativeValues.Add(numericValue);
            maximumValue = Math.Max(maximumValue, numericValue);
        }

        RequiredCapacity = (long)maximumValue + 1L;
        IsHighlySparse = RequiredCapacity > SparsityCheckThreshold && RequiredCapacity > (long)Math.Max(1, uniqueNonNegativeValues.Count) * MaximumAutomaticSparsityRatio;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIndex(TKey key)
    {
        return Unsafe.As<TKey, int>(ref key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TKey GetKey(int index)
    {
        return Unsafe.As<int, TKey>(ref index);
    }
}
