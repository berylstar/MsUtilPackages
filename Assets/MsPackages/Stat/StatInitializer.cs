using System;
using UnityEngine;

[Serializable]
public abstract class StatInitializer<T> where T : IComparable<T>
{
    [SerializeField] private StatData<T> statData;
    [field: SerializeField] public Stat<T> Stat { get; private set; }

    public void Initialize()
    {
        if (statData == null)
            return;

        Stat = CreateStat(statData);
    }

    protected abstract Stat<T> CreateStat(StatData<T> data);
}