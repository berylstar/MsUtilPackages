using System;

[Serializable]
public class FloatStatInitializer : StatInitializer<float, FloatStat>
{
    protected override FloatStat CreateStat(StatData<float> data)
    {
        return new FloatStat(data.InitialValue, data.MinValue, data.MaxValue);
    }
}