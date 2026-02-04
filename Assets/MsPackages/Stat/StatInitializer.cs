using System;

[Serializable]
public class StatInitializer<T> where T : IComparable<T>
{
    public StatData<T> statData;
    public Stat<T> stat;

    public void Initialize()
    {
        if (stat == null)
        {
            stat = new Stat<T>(statData);
        }
    }
}