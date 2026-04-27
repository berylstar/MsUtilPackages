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