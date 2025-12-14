using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public static class Extensions
{
    #region UI
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
    #endregion

    #region Enumerate
    /// <summary>
    /// 배열 셔플
    /// </summary>
    public static void Shuffle<T>(this T[] array)
    {
        if (array == null || array.Length <= 1)
            return;

        int randomIndex;
        T tempValue;

        // Fisher-Yates 셔플
        for (int i = array.Length - 1; i > 0; i--)
        {
            randomIndex = Random.Range(0, i + 1);

            tempValue = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = tempValue;
        }
    }

    /// <summary>
    /// 리스트 셔플
    /// </summary>
    public static void Shuffle<T>(this List<T> list)
    {
        if (list == null || list.Count <= 1)
            return;

        int randomIndex;
        T tempValue;

        // Fisher-Yates 셔플
        for (int i = list.Count - 1; i > 0; i--)
        {
            randomIndex = Random.Range(0, i + 1);

            tempValue = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = tempValue;
        }
    }
    #endregion
}
