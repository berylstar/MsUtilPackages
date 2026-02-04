using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 스탯의 종류 열거형
/// </summary>
public enum EStatType
{
    None = 0,

}


/// <summary>
/// 스탯 제너릭 클래스
/// </summary>
[Serializable]
public class Stat<T> where T : IComparable<T>
{
    [SerializeField, InspectorReadonly] private T _baseValue;        // 기본값
    [SerializeField, InspectorReadonly] private T _minValue;         // 최솟값
    [SerializeField, InspectorReadonly] private T _maxValue;         // 최댓값

    [Space(10)]
    [SerializeField, InspectorReadonly] private T _currentValue;     // 현재값

    private readonly T _initialValue;                                // 최초값
    private readonly List<StatModifier<T>> _modifiers = new List<StatModifier<T>>();
    private readonly IStatOperator<T> _iOperator;

    /// <summary>
    /// 현재 값이 변경되었을 때 호출되는 이벤트
    /// </summary>
    private event Action<Stat<T>> OnValueChanged;

    public Stat(T newBaseValue, T newMinValue, T newMaxValue, IStatOperator<T> newIOperator)
    {
        //this.baseValue = baseValue;
        this._minValue = newMinValue;
        this._maxValue = newMaxValue;
        this._iOperator = newIOperator;

        SetBaseValue(newBaseValue);

        _initialValue = this._baseValue;
        _currentValue = this._baseValue;

        OnValueChanged = null;
    }

    public Stat(StatData<T> statData) : this(statData.BaseValue, statData.MinValue, statData.MaxValue, statData.GetOperator()) { }

    /// <summary>
    /// 기본 값 - 수정자를 적용하지 않은 기본 값
    /// </summary>
    public T BaseValue => _baseValue;

    /// <summary>
    /// 현재 값 - 모든 수정자가 적용된 최종 사용 값
    /// </summary>
    public T CurrentValue => _currentValue;

    public T MinValue => _minValue;

    public T MaxValue => _maxValue;

    public bool IsEmpty => _currentValue.CompareTo(_minValue) <= 0;
    public bool IsFull => _currentValue.CompareTo(_maxValue) >= 0;
    public bool IsValid => _currentValue.CompareTo(_minValue) >= 0 && _currentValue.CompareTo(_maxValue) <= 0;
    public float Ratio => _iOperator.Ratio(_currentValue, _minValue, _maxValue);

    /// <summary>
    /// 현재 값과 비교
    /// </summary>
    public T CompareTo(T value)
    {
        return _iOperator.Subtract(_currentValue, value);
    }

    public void SetBaseValue(T newBaseValue)
    {
        _baseValue = _iOperator.Clamp(newBaseValue, _minValue, _maxValue);
        UpdateCurrentValue();
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

        // ClearEvent(); 는 하지 않는다.

        UpdateCurrentValue();
    }

    public void AddValue(T amount)
    {
        SetBaseValue(_iOperator.Add(_baseValue, amount));
    }

    public void SubtractValue(T amount)
    {
        SetBaseValue(_iOperator.Subtract(_baseValue, amount));
    }

    public void MultiplyValue(T amount)
    {
        SetBaseValue(_iOperator.Multiply(_baseValue, amount));
    }

    public void DivideValue(T amount)
    {
        SetBaseValue(_iOperator.Divide(_baseValue, amount));
    }

    public void SetFull()
    {
        SetBaseValue(_maxValue);
    }

    public void SetEmpty()
    {
        SetBaseValue(_minValue);
    }

    public void SetMaxValue(T newMaxValue)
    {
        _maxValue = newMaxValue;
        UpdateCurrentValue();
    }

    public void AddMaxValue(T amount)
    {
        SetMaxValue(_iOperator.Add(_maxValue, amount));
    }

    public void SetMinValue(T newMinValue)
    {
        _minValue = newMinValue;
        UpdateCurrentValue();
    }

    public void AddModifier(StatModifier<T> modifier)
    {
        _modifiers.Add(modifier);
        _modifiers.Sort((a, b) => a.Order.CompareTo(b.Order)); // 우선순위에 따라 정렬

        UpdateCurrentValue();
    }

    public bool RemoveModifier(StatModifier<T> modifier)
    {
        bool removed = _modifiers.Remove(modifier);
        if (removed)
        {
            UpdateCurrentValue();
        }
        return removed;
    }

    public bool RemoveModifiersFromSource(object source)
    {
        bool removed = false;
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            if (_modifiers[i].Source == source)
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

    public void ClearModifiers()
    {
        if (_modifiers.Count > 0)
        {
            _modifiers.Clear();
            UpdateCurrentValue();
        }
    }

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

    public void RegisterEvent(Action<Stat<T>> _event)
    {
        OnValueChanged += _event;
        // OnValueChanged?.Invoke(this);
    }

    public void ClearEvent()
    {
        OnValueChanged = null;
    }
}