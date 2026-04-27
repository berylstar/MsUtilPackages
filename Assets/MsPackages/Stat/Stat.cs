using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 스탯 제너릭 클래스
/// </summary>
[Serializable]
public abstract class Stat<T, TStat> where T : IComparable<T> where TStat : Stat<T, TStat>
{
    [SerializeField] protected T _baseValue;
    [SerializeField] protected T _minValue;
    [SerializeField] protected T _maxValue;         
    [SerializeField] protected T _currentValue;

    /// <summary>
    /// 기본 값 - 수정자를 적용하지 않은 기본 값
    /// </summary>
    public T BaseValue => _baseValue;

    /// <summary>
    /// 최솟값
    /// </summary>
    public T MinValue => _minValue;

    /// <summary>
    /// 최댓값
    /// </summary>
    public T MaxValue => _maxValue;

    /// <summary>
    /// 현재 값 - 모든 수정자가 적용된 최종 사용 값
    /// </summary>
    public T CurrentValue => _currentValue;

    /// <summary>
    /// 현재 값이 변경되었을 때 호출되는 이벤트
    /// </summary>
    private event Action<TStat> OnValueChanged;

    // 최초 기본 값
    private readonly T _initialBaseValue;
    private readonly T _initialMinValue;
    private readonly T _initialMaxValue;

    /// <summary>
    /// 현재 적용 중인 스탯 수정자 리스트
    /// </summary>
    private readonly List<StatModifier> _modifiers = new List<StatModifier>();

    /// <summary>
    /// 제너릭 사칙연산을 수행하기 위한 객체
    /// </summary>
    protected readonly IStatOperator<T> _iOperator;

    /// <summary>
    /// 스탯 캐싱
    /// </summary>
    private readonly TStat _instance;

    protected Stat(T newInitialValue, T newMinValue, T newMaxValue, IStatOperator<T> newOperator)
    {
        this._initialBaseValue = newInitialValue;
        this._initialMinValue = newMinValue;
        this._initialMaxValue = newMaxValue;
        this._iOperator = newOperator;

        this._minValue = newMinValue;
        this._maxValue = newMaxValue;

        OnValueChanged = null;

        SetBaseValue(newInitialValue);

        _instance = (TStat)this;
    }

    public bool IsEmpty => _iOperator.IsLessThanOrEqual(_currentValue, _minValue);
    public bool IsFull => _iOperator.IsMoreThanOrEqual(_currentValue, _maxValue);
    public bool IsValid => _iOperator.IsMoreThan(_currentValue, _minValue);
    public float Ratio => _iOperator.Ratio(_currentValue, _minValue, _maxValue);
    public T Remain => _iOperator.Subtract(MaxValue, CurrentValue);

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
        _baseValue = _initialBaseValue;
        _minValue = _initialMinValue;
        _maxValue = _initialMaxValue;

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
    public void AddValue(T amount)
    {
        SetBaseValue(_iOperator.Add(_baseValue, amount));
    }

    /// <summary>
    /// 기본 값 뺄셈
    /// </summary>
    public void SubtractValue(T amount)
    {
        SetBaseValue(_iOperator.Subtract(_baseValue, amount));
    }

    /// <summary>
    /// 기본 값 곱셈
    /// </summary>
    public void MultiplyValue(T amount)
    {
        SetBaseValue(_iOperator.Multiply(_baseValue, amount));
    }

    /// <summary>
    /// 기본 값 나눗셈
    /// </summary>
    public void DivideValue(T amount)
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

        // 최솟값 변경시 baseValue 재설정
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

        // 최댓값 변경시 baseValue 재설정
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
    public void AddModifier(StatModifier modifier)
    {
        _modifiers.Add(modifier);
        _modifiers.Sort((a, b) => a.Order.CompareTo(b.Order)); // 우선순위에 따라 정렬

        UpdateCurrentValue();
    }

    /// <summary>
    /// 수정자 제거
    /// </summary>
    public bool RemoveModifier(StatModifier modifier)
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

        // 여러 수정자가 있을 수 있기 때문에 역순 순회
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
        float sumPercentAdd = 0f;

        for (int i = 0; i < _modifiers.Count; i++)
        {
            StatModifier modifier = _modifiers[i];

            switch (modifier.Type)
            {
                case EStatModifierType.Flat:
                    result = _iOperator.AddFloat(result, modifier.Value);
                    break;

                case EStatModifierType.PercentAdd:
                    sumPercentAdd += modifier.Value;

                    if (i + 1 < _modifiers.Count && _modifiers[i + 1].Type == EStatModifierType.PercentAdd)
                    {
                        // 다음 PercentAdd 수정자까지 합산 계속
                        continue;
                    }

                    result = _iOperator.MultiplyFloat(result, 1f + sumPercentAdd);
                    sumPercentAdd = 0f;
                    break;

                case EStatModifierType.PercentMult:
                    result = _iOperator.MultiplyFloat(result, 1f + modifier.Value);
                    break;
            }
        }

        result = _iOperator.Clamp(result, _minValue, _maxValue);

        if (_iOperator.IsEqual(_currentValue, result) == false)
        {
            // 실제 값의 변동이 생겼을 때 콜백 발생
            _currentValue = result;
            OnValueChanged?.Invoke(_instance);
        }
    }

    /// <summary>
    /// 새 콜백 등록하고 호출
    /// </summary>
    public void RegisterListener(Action<TStat> listener, bool isInvoke = true)
    {
        OnValueChanged += listener;

        // 등록 즉시 현재 상태 반영
        if (isInvoke)
        {
            listener?.Invoke(_instance);
        }
    }

    /// <summary>
    /// 콜백 해제
    /// </summary>
    public void UnregisterListener(Action<TStat> listener)
    {
        OnValueChanged -= listener;
    }

    /// <summary>
    /// 모든 콜백 해제
    /// </summary>
    public void ClearListener()
    {
        OnValueChanged = null;
    }

    public override string ToString()
    {
        return $"Base: {_baseValue}\t[{_minValue} ~ {_maxValue}]\t=> Curr: {_currentValue}";
    }
}