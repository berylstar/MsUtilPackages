using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public static class Extensions
{
    /// <summary>
    /// 버튼 클릭 이벤트 초기화
    /// </summary>
    public static void InitializeButtonEvent(this Button btn, UnityAction clickAction)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(clickAction);
    }

    /// <summary>
    /// 토글 변화 이벤트 초기화
    /// </summary>
    public static void InitializeToggleEvent(this Toggle toggle, UnityAction<bool> changeAction)
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(changeAction);
    }
}
