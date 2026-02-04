using UnityEngine;

[CreateAssetMenu(fileName = "IntStatData", menuName = "Scriptable Object/Stat/Int Stat Data")]
public class IntStatData : StatData<int>
{
    public override IStatOperator<int> GetOperator()
    {
        return IntStatOperator.Instance;
    }
}