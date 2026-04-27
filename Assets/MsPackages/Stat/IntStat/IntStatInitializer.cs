using System;

[Serializable]
public class IntStatInitializer : StatInitializer<int>
{
    protected override Stat<int> CreateStat(StatData<int> data)
    {
        return new IntStat(data.InitialValue, data.MinValue, data.MaxValue);
    }
}