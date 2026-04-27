public class IntStat : Stat<int>
{
    public IntStat(int newInitialValue, int newMinValue, int newMaxValue) : base(newInitialValue, newMinValue, newMaxValue, IntStatOperator.Instance)
    {

    }
}
