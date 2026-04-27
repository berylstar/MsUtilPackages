using UnityEngine;

/// <summary>
/// float 타입 값에 대한 연산 구현
/// </summary>
public class FloatStatOperator : IStatOperator<float>
{
    public static readonly FloatStatOperator Instance = new();

    public float Zero => 0f;
    public float One => 1f;

    public bool IsEqual(float a, float b) => Mathf.Approximately(a, b);
    public bool IsLessThan(float a, float b) => a < b;
    public bool IsMoreThan(float a, float b) => a > b;
    public bool IsLessThanOrEqual(float a, float b) => a < b || IsEqual(a, b);
    public bool IsMoreThanOrEqual(float a, float b) => a > b || IsEqual(a, b);
    public bool IsBetween(float value, float min, float max) => IsMoreThanOrEqual(value, min) && IsLessThanOrEqual(value, max);

    public float Add(float a, float b) => a + b;
    public float Subtract(float a, float b) => a - b;
    public float Multiply(float a, float b) => a * b;
    public float Divide(float a, float b) => b != 0 ? a / b : 0;

    public float AddFloat(float a, float b) => Add(a, b);
    public float SubtractFloat(float a, float b) => Subtract(a, b);
    public float MultiplyFloat(float a, float b) => Multiply(a, b);
    public float DivideFloat(float a, float b)
    {
        if (Mathf.Approximately(b, 0f))
            return 0;

        return Mathf.RoundToInt(a / b);
    }

    public float AddOne(float a) => a + 1f;
    public float Clamp(float value, float min, float max) => Mathf.Clamp(value, min, max);

    public float Ratio(float current, float min, float max)
    {
        float range = max - min;

        if (range <= Mathf.Epsilon)
            return 0f;

        return Mathf.Clamp01((current - min) / range);
    }
}