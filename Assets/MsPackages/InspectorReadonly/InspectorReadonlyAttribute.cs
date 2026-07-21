using System;
using UnityEngine;

/// <summary>
/// 직렬화된 필드를 Unity Inspector에서 읽기 전용 표시
/// (필드에만 적용 가능, 중복 적용 불가, 상속 관계에서도 Attribute 정보 유지)
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class InspectorReadonlyAttribute : PropertyAttribute
{
}
