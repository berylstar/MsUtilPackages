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
    T AddOne(T a);
    T Clamp(T value, T min, T max);

    float Ratio(T current, T min, T max);    
}

/// <summary>
/// 정적 생성자에서 연산자를 캐싱하는 클래스
/// </summary>
public static class StatOperatorProvider<T>
{
    public static readonly IStatOperator<T> Operator;

    static StatOperatorProvider()
    {
        var type = typeof(T);

        if (type == typeof(int))
        {
            Operator = IntStatOperator.Instance as IStatOperator<T>;
        }
        else if (type == typeof(float))
        {
            Operator = FloatStatOperator.Instance as IStatOperator<T>;
        }

        if (Operator == null)
        {
            Debug.LogError($"{type.Name} 타입에 대한 StatOperator가 등록되지 않았습니다!");
        }
    }
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