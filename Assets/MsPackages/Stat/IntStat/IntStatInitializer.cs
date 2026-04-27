using System;

[Serializable]
public class IntStatInitializer : StatInitializer<int, IntStat>
{
    protected override IntStat CreateStat(StatData<int> data)
    {
        return new IntStat(data.InitialValue, data.MinValue, data.MaxValue);
    }
}