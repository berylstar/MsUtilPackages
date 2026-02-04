using System;

/// <summary>
/// 스탯 수정자 적용 방식 타입
/// </summary>
public enum EStatModifierType
{
    Flat,       // 고정값 추가 : + Flat
    PercentAdd, // 퍼센트 덧셈 : * (1 + PercentAdd 합산 값)
    PercentMult // 퍼센트 곱셈 : * (1 + PercentMult) * (1 + PercentAdd) ...
}

/// <summary>
/// 제너릭 타입 스탯 수정자
/// </summary>
[Serializable]
public class StatModifier<T>
{
    public T Value;                 // 수정자 값
    public EStatModifierType Type;  // 수정자 타입
    public int Order;               // 적용 우선순위
    public object Source;           // 수정자 출처

    public StatModifier(T value, EStatModifierType type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }

    public override string ToString()
    {
        return $"[{Type}] : {Value} (From {Source}, Order={Order})";
    }
}