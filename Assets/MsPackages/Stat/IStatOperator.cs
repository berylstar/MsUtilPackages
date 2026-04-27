using UnityEngine;

/// <summary>
/// 제너릭 타입 연산을 위한, 연산 처리 인터페이스
/// </summary>
public interface IStatOperator<T>
{
    T Zero { get; }
    T One { get; }

    bool IsEqual(T a, T b);
    bool IsLessThan(T a, T b);
    bool IsMoreThan(T a, T b);
    bool IsLessThanOrEqual(T a, T b);
    bool IsMoreThanOrEqual(T a, T b);
    bool IsBetween(T value, T min, T max);

    T Add(T a, T b);
    T Subtract(T a, T b);
    T Multiply(T a, T b);
    T Divide(T a, T b);

    T AddFloat(T a, float b);
    T SubtractFloat(T a, float b);
    T MultiplyFloat(T a, float b);
    T DivideFloat(T a, float b);

    T AddOne(T a);
    T Clamp(T value, T min, T max);

    float Ratio(T current, T min, T max);    
}

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