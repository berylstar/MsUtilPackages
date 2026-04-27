using System;
using UnityEngine;

[Serializable]
public abstract class StatInitializer<T, TStat> where T : IComparable<T> where TStat : Stat<T, TStat>
{
    [SerializeField] private StatData<T> statData;
    [field: SerializeField] public TStat Stat { get; private set; }

    public void Initialize()
    {
        if (statData == null)
            return;

        Stat = CreateStat(statData);
    }

    protected abstract TStat CreateStat(StatData<T> data);
}