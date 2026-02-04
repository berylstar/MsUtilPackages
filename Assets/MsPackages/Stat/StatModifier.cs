using System;

/// <summary>
/// 스탯 수정자 적용 방식 타입
/// </summary>
public enum EStatModifierType
{
    /// <summary>
    /// 고정값 추가 : + Flat
    /// </summary>
    Flat,

    /// <summary>
    /// 합산형 곱셈 : * (1 + PercentAdd 합산 값)
    /// </summary>
    PercentAdd,

    /// <summary>
    /// 복리형 곱셈 : * (1 + PercentMult) * (1 + PercentAdd) ...
    /// </summary>
    PercentMult
}

/// <summary>
/// 제너릭 타입 스탯 수정자
/// </summary>
[Serializable]
public class StatModifier<T>
{
    /// <summary>
    /// 수정자 타입
    /// </summary>
    public EStatModifierType Type { get; }

    /// <summary>
    /// 수정자 값
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// 수정자 우선 순위
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// 수정자 출처
    /// </summary>
    public int SourceId { get; }

    public StatModifier(T value, EStatModifierType type, int order, int sourceId)
    {
        this.Value = value;
        this.Type = type;
        this.Order = order;
        this.SourceId = sourceId;
    }

    public override string ToString()
    {
        return $"[{Type}] : {Value} (From {SourceId}, Order={Order})";
    }
}