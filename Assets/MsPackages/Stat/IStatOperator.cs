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

    T Add(T a, T b);
    T Subtract(T a, T b);
    T Multiply(T a, T b);
    T Divide(T a, T b);
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

    public float Add(float a, float b) => a + b;
    public float Subtract(float a, float b) => a - b;
    public float Multiply(float a, float b) => a * b;
    public float Divide(float a, float b) => b != 0 ? a / b : 0;
    public float AddOne(float a) => a + 1f;
    public float Clamp(float value, float min, float max) => Mathf.Clamp(value, min, max);
    public float Ratio(float current, float min, float max) => max > min ? Mathf.Clamp01((current - min) / (max - min)) : 0f;
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

    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
    public int Multiply(int a, int b) => a * b;
    public int Divide(int a, int b) => b != 0 ? a / b : 0;
    public int AddOne(int a) => a + 1;
    public int Clamp(int value, int min, int max) => Mathf.Clamp(value, min, max);
    public float Ratio(int current, int min, int max) => max > min ? Mathf.Clamp01((float)(current - min) / (max - min)) : 0f;
}