using System;
using UnityEngine;

[Serializable]
public class StatInitializer<T> where T : IComparable<T>
{
    [SerializeField] private StatData<T> statData;
    [field: SerializeField] public Stat<T> Stat { get; private set; }

    public void Initialize()
    {
        if (statData == null)
            return;

        Stat = new Stat<T>(statData);
    }
}