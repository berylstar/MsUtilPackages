using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IntStat))]
//[CustomPropertyDrawer(typeof(FloatStat))]
public class StatDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // PropertyDrawer 시작
        label = EditorGUI.BeginProperty(position, label, property);

        // 접두사 라벨(변수명) 그리기
        position = EditorGUI.PrefixLabel(position, label);

        // 가로 영역 분할 계산 (4등분)
        float spacing = 2f;
        float width = (position.width - (spacing * 3)) / 4f;

        // 각 필드의 Rect 계산
        Rect baseRect = new Rect(position.x, position.y, width, position.height);
        Rect minRect = new Rect(position.x + width + spacing, position.y, width, position.height);
        Rect maxRect = new Rect(position.x + (width + spacing) * 2, position.y, width, position.height);
        Rect curRect = new Rect(position.x + (width + spacing) * 3, position.y, width, position.height);

        // 들여쓰기 초기화 (필드 내 라벨 정렬을 위함)
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // 각 필드 읽기 전용처럼 보이게 렌더링
        GUI.enabled = false;
        DrawStatField(baseRect, property, "_baseValue", "B");
        DrawStatField(minRect, property, "_minValue", "m");
        DrawStatField(maxRect, property, "_maxValue", "M");

        DrawStatField(curRect, property, "_currentValue", "C");
        GUI.enabled = true;

        // 들여쓰기 복구 및 종료
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    private void DrawStatField(Rect rect, SerializedProperty parent, string propName, string label)
    {
        // 자동 프로퍼티용 이름 변환 로직 제거, 전달받은 변수명을 그대로 찾음
        SerializedProperty prop = parent.FindPropertyRelative(propName);

        if (prop != null)
        {
            float labelWidth = 15f;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, rect.height), label);
            EditorGUI.PropertyField(new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height), prop, GUIContent.none);
        }
    }
}