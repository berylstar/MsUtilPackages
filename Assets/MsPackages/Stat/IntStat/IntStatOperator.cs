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