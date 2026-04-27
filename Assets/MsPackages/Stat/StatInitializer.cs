using System;
using UnityEngine;

/// <summary>
/// 스탯 생성을 담당하는 클래스
/// </summary>
[Serializable]
public abstract class StatInitializer<T, TStat> where T : IComparable<T> where TStat : Stat<T, TStat>
{
    /// <summary>
    /// 스탯 원본 데이터
    /// </summary>
    [SerializeField] private StatData<T> statData;

    /// <summary>
    /// 스탯
    /// </summary>
    [field: SerializeField] public TStat Stat { get; private set; }

    /// <summary>
    /// 초기화
    /// </summary>
    public void Initialize()
    {
        if (statData == null)
            return;

        Stat = CreateStat(statData);
    }

    /// <summary>
    /// 스탯 생성
    /// </summary>
    protected abstract TStat CreateStat(StatData<T> data);
}