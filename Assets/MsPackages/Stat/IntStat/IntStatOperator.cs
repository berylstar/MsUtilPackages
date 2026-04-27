using UnityEngine;

/// <summary>
/// int 타입 값에 대한 연산 구현
/// </summary>
public class IntStatOperator : IStatOperator<int>
{
    public static readonly IntStatOperator Instance = new();

    public int Zero => 0;
    public int One => 1;

    public bool IsEqual(int a, int b) => a == b;
    public bool IsLessThan(int a, int b) => a < b;
    public bool IsMoreThan(int a, int b) => a > b;
    public bool IsLessThanOrEqual(int a, int b) => a <= b;
    public bool IsMoreThanOrEqual(int a, int b) => a >= b;
    public bool IsBetween(int value, int min, int max) => min <= value && value <= max;

    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
    public int Multiply(int a, int b) => a * b;
    public int Divide(int a, int b) => b != 0 ? a / b : 0;

    public int AddFloat(int a, float b) => a + Mathf.RoundToInt(b);
    public int SubtractFloat(int a, float b) => a - Mathf.RoundToInt(b);
    public int MultiplyFloat(int a, float b) => Mathf.RoundToInt(a * b);
    public int DivideFloat(int a, float b)
    {
        if (Mathf.Approximately(b, 0f))
            return 0;

        return Mathf.RoundToInt(a / b);
    }

    public int AddOne(int a) => a + 1;
    public int Clamp(int value, int min, int max) => Mathf.Clamp(value, min, max);

    public float Ratio(int current, int min, int max)
    {
        int range = max - min;

        if (range <= 0)
            return 0f;

        return Mathf.Clamp01((float)(current - min) / range);
    }
}