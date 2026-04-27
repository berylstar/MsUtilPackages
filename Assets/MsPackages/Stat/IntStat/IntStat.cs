[System.Serializable]
public class IntStat : Stat<int, IntStat>
{
    public IntStat(int newInitialValue, int newMinValue, int newMaxValue) : base(newInitialValue, newMinValue, newMaxValue, IntStatOperator.Instance)
    {

    }
}
