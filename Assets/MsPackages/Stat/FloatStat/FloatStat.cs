[System.Serializable]
public class FloatStat : Stat<float, FloatStat>
{
    public FloatStat(float newInitialValue, float newMinValue, float newMaxValue) : base(newInitialValue, newMinValue, newMaxValue, FloatStatOperator.Instance)
    {

    }
}
