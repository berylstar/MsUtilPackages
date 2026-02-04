using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 스탯의 종류 열거형
/// </summary>
public enum EStatType
{
    None        = 0,
    MoveSpeed   = 1 << 0,
    HP          = 1 << 1,
    AP          = 1 << 2,
}

/// <summary>
/// 스탯 제너릭 클래스
/// </summary>
[Serializable]
public class Stat<T> where T : IComparable<T>
{
    [SerializeField, InspectorReadonly] private T _baseValue;
    /// <summary>
    /// 기본 값 - 수정자를 적용하지 않은 기본 값
    /// </summary>
    public T BaseValue => _baseValue;

    [SerializeField, InspectorReadonly] private T _minValue;
    /// <summary>
    /// 최솟값
    /// </summary>
    public T MinValue => _minValue;

    [SerializeField, InspectorReadonly] private T _maxValue;         
    /// <summary>
    /// 최댓값
    /// </summary>
    public T MaxValue => _maxValue;

    [SerializeField, InspectorReadonly] private T _currentValue;
    /// <summary>
    /// 현재 값 - 모든 수정자가 적용된 최종 사용 값
    /// </summary>
    public T CurrentValue => _currentValue;

    /// <summary>
    /// 현재 값이 변경되었을 때 호출되는 이벤트
    /// </summary>
    public event Action<Stat<T>> OnValueChanged;

    // 최초 기본 값
    private readonly T _initialValue;

    // 스탯 수정자 리스트
    private readonly List<StatModifier<T>> _modifiers = new List<StatModifier<T>>();

    // 연산 처리기
    private readonly IStatOperator<T> _iOperator;

    public Stat(T newInitialValue, T newMinValue, T newMaxValue, IStatOperator<T> newIOperator)
    {
        this._minValue = newMinValue;
        this._maxValue = newMaxValue;
        this._iOperator = newIOperator;

        SetBaseValue(newInitialValue);

        _initialValue = this._baseValue;
        _currentValue = this._baseValue;

        OnValueChanged = null;
    }

    public Stat(StatData<T> statData) : this(statData.InitialValue, statData.MinValue, statData.MaxValue, statData.GetOperator()) { }

    public bool IsEmpty => _iOperator.IsLessThanOrEqual(_currentValue, _minValue);
    public bool IsFull => _iOperator.IsMoreThanOrEqual(_currentValue, _maxValue);
    public bool IsValid => _iOperator.IsBetween(_currentValue, _minValue, _maxValue);
    public float Ratio => _iOperator.Ratio(_currentValue, _minValue, _maxValue);

    /// <summary>
    /// 현재 값과의 차이
    /// </summary>
    public T GetDifference(T value)
    {
        return _iOperator.Subtract(_currentValue, value);
    }

    /// <summary>
    /// 현재 값이 해당 범위 사이에 있는지
    /// </summary>
    public bool IsInRange(T min, T max)
    {
        return _iOperator.IsBetween(_currentValue, min, max);
    }

    /// <summary>
    /// 초기 상태로 되돌린다
    /// </summary>
    public void Reset()
    {
        _baseValue = _iOperator.Clamp(_initialValue, _minValue, _maxValue);  // SetBaseValue();

        if (_modifiers.Count > 0)    // ClearModifiers();
        {
            _modifiers.Clear();
        }

        UpdateCurrentValue();
    }

    #region Base Value
    /// <summary>
    /// 기본 값 변경
    /// </summary>
    public void SetBaseValue(T newBaseValue)
    {
        _baseValue = _iOperator.Clamp(newBaseValue, _minValue, _maxValue);

        UpdateCurrentValue();
    }

    /// <summary>
    /// 기본 값 덧셈
    /// </summary>
    public void AddBaseValue(T amount)
    {
        SetBaseValue(_iOperator.Add(_baseValue, amount));
    }

    /// <summary>
    /// 기본 값 뺄셈
    /// </summary>
    public void SubtractBaseValue(T amount)
    {
        SetBaseValue(_iOperator.Subtract(_baseValue, amount));
    }

    /// <summary>
    /// 기본 값 곱셈
    /// </summary>
    public void MultiplyBaseValue(T amount)
    {
        SetBaseValue(_iOperator.Multiply(_baseValue, amount));
    }

    /// <summary>
    /// 기본 값 나눗셈
    /// </summary>
    public void DivideBaseValue(T amount)
    {
        if (_iOperator.IsMoreThan(amount, _iOperator.Zero))
        {
            SetBaseValue(_iOperator.Divide(_baseValue, amount));
        }
    }

    /// <summary>
    /// 최댓값으로 설정
    /// </summary>
    public void SetFull()
    {
        SetBaseValue(_maxValue);
    }

    /// <summary>
    /// 최솟값으로 설정
    /// </summary>
    public void SetEmpty()
    {
        SetBaseValue(_minValue);
    }
    #endregion

    #region Min Value
    public void SetMinValue(T newMinValue)
    {
        _minValue = newMinValue;

        // baseValue 재설정
        SetBaseValue(_baseValue);
    }

    public void AddMinValue(T amount)
    {
        SetMinValue(_iOperator.Add(_minValue, amount));
    }

    public void SubtractMinValue(T amount)
    {
        SetMinValue(_iOperator.Subtract(_minValue, amount));
    }
    #endregion

    #region Max Value
    public void SetMaxValue(T newMaxValue)
    {
        _maxValue = newMaxValue;

        // baseValue 재설정
        SetBaseValue(_baseValue);
    }

    public void AddMaxValue(T amount)
    {
        SetMaxValue(_iOperator.Add(_maxValue, amount));
    }

    public void SubtractMaxValue(T amount)
    {
        SetMaxValue(_iOperator.Subtract(_maxValue, amount));
    }
    #endregion

    #region Modifier
    /// <summary>
    /// 수정자 추가
    /// </summary>
    public void AddModifier(StatModifier<T> modifier)
    {
        _modifiers.Add(modifier);
        _modifiers.Sort((a, b) => a.Order.CompareTo(b.Order)); // 우선순위에 따라 정렬

        UpdateCurrentValue();
    }

    /// <summary>
    /// 수정자 제거
    /// </summary>
    public bool RemoveModifier(StatModifier<T> modifier)
    {
        bool removed = _modifiers.Remove(modifier);

        if (removed)
        {
            UpdateCurrentValue();
        }

        return removed;
    }

    /// <summary>
    /// 특정 소스의 수정자 모두 제거
    /// </summary>
    public bool RemoveModifiersFromSourceId(int sourceId)
    {
        bool removed = false;

        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            if (_modifiers[i].SourceId == sourceId)
            {
                _modifiers.RemoveAt(i);
                removed = true;
            }
        }

        if (removed)
        {
            UpdateCurrentValue();
        }

        return removed;
    }

    /// <summary>
    /// 수정자 초기화
    /// </summary>
    public void ClearModifiers()
    {
        if (_modifiers.Count > 0)
        {
            _modifiers.Clear();

            UpdateCurrentValue();
        }
    }
    #endregion

    /// <summary>
    /// 현재 값 최신화
    /// </summary>
    private void UpdateCurrentValue()
    {
        T result = _baseValue;
        T sumPercentAdd = _iOperator.Zero;

        for (int i = 0; i < _modifiers.Count; i++)
        {
            StatModifier<T> modifier = _modifiers[i];

            switch (modifier.Type)
            {
                case EStatModifierType.Flat:
                    result = _iOperator.Add(result, modifier.Value);
                    break;

                case EStatModifierType.PercentAdd:
                    sumPercentAdd = _iOperator.Add(sumPercentAdd, modifier.Value);

                    if (i + 1 < _modifiers.Count && _modifiers[i + 1].Type == EStatModifierType.PercentAdd)
                    {
                        // 다음 PercentAdd 수정자까지 합산 계속
                    }
                    else
                    {
                        result = _iOperator.Multiply(result, _iOperator.AddOne(sumPercentAdd));
                        sumPercentAdd = _iOperator.Zero;
                    }
                    break;

                case EStatModifierType.PercentMult:
                    result = _iOperator.Multiply(result, _iOperator.AddOne(modifier.Value));
                    break;
            }
        }

        result = _iOperator.Clamp(result, _minValue, _maxValue);

        if (_iOperator.IsEqual(_currentValue, result) == false)
        {
            _currentValue = result;
            OnValueChanged?.Invoke(this);
        }
    }
}