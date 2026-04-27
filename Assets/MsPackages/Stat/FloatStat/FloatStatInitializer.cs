using System;

[Serializable]
public class FloatStatInitializer : StatInitializer<float>
{
    protected override Stat<float> CreateStat(StatData<float> data)
    {
        return new FloatStat(data.InitialValue, data.MinValue, data.MaxValue);
    }
}