using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LocalizedString))]
public class LocalizedStringDrawer : PropertyDrawer
{
    // 1. 인스펙터에서 차지할 전체 높이 계산
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 키 입력줄(1) + 한국어줄(1) + 영어줄(1) + 여백 = 총 3.2줄 정도 사용
        return EditorGUIUtility.singleLineHeight * 3.2f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 데이터 연결
        SerializedProperty keyProp = property.FindPropertyRelative("key");
        LocalizationManager.CheckInitialization();

        // 한 줄의 높이 설정
        float lineHeight = EditorGUIUtility.singleLineHeight;

        // 2. 영역 계산 (Y값을 더해가며 배치)
        // 첫 번째 줄: 레이블과 키 입력 필드
        Rect keyRect = new Rect(position.x, position.y, position.width, lineHeight);

        // 두 번째 줄: 한국어 정보
        Rect korRect = new Rect(position.x + 15, position.y + lineHeight + 2, position.width - 15, lineHeight);

        // 세 번째 줄: 영어 정보
        Rect engRect = new Rect(position.x + 15, position.y + (lineHeight * 2) + 4, position.width - 15, lineHeight);

        // 3. 필드 그리기
        EditorGUI.PropertyField(keyRect, keyProp, label);

        // 4. 언어별 텍스트 미리보기
        string keyValue = keyProp.stringValue;
        GUIStyle previewStyle = new GUIStyle(EditorStyles.miniLabel);
        previewStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f); // 가독성을 위한 회색

        if (string.IsNullOrEmpty(keyValue) == false)
        {
            // 한국어 표시
            string korValue = LocalizationManager.Get(keyValue, ELanguage.KOR);
            EditorGUI.LabelField(korRect, $"[KOR]   {korValue}", previewStyle);

            // 영어 표시
            string engValue = LocalizationManager.Get(keyValue, ELanguage.ENG);
            EditorGUI.LabelField(engRect, $"[ENG]   {engValue}", previewStyle);
        }
        else
        {
            EditorGUI.LabelField(korRect, "(Empty Key)", previewStyle);
        }

        EditorGUI.EndProperty();
    }
}

public class LocalizationTools
{
    [MenuItem("Tools/Refresh Localization")]
    public static void RefreshLocalization()
    {
        LocalizationManager.Initialize();

        // 사용자 피드백을 위해 로그와 알림 표시
        Debug.Log("<color=cyan>[Localization]</color> 다국어 최신화");

        //// 유니티 오른쪽 하단에 잠시 나타나는 알림창
        //SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Localization Initialized!"));
    }
}