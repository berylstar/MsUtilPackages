using UnityEngine;

/// <summary>
/// 스탯 초기값 스크립터블 오브젝트
/// </summary>
public abstract class StatData<T> : ScriptableObject
{
    [SerializeField] protected EStatType type;
    public EStatType Type => type;

    [SerializeField] protected T baseValue;
    public T BaseValue => baseValue;

    [SerializeField] protected T minValue;
    public T MinValue => minValue;

    [SerializeField] protected T maxValue;
    public T MaxValue => maxValue;

    public abstract IStatOperator<T> GetOperator();
}