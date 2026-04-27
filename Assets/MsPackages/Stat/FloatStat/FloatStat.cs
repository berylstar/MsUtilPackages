[System.Serializable]
public class FloatStat : Stat<float>
{
    public FloatStat(float newInitialValue, float newMinValue, float newMaxValue) : base(newInitialValue, newMinValue, newMaxValue, FloatStatOperator.Instance)
    {

    }
}
