using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum EScene
{
    // 열거형의 값은 Build Settings의 Scene Index
}

public static class Utils
{
    public static void Log(object message, string header = "LOG")
    {
#if UNITY_EDITOR
        Debug.Log($"[{header}] {message}");
#endif
    }

    #region Scene Management
    /// <summary>
    /// buildIndex 기반으로 현재 씬 타입 반환
    /// </summary>
    public static EScene GetActiveScene()
    {
        return (EScene)SceneManager.GetActiveScene().buildIndex;
    }

    /// <summary>
    /// 씬 전환
    /// </summary>
    public static void LoadScene(EScene sceneName)
    {
        SceneManager.LoadScene((int)sceneName);
    }
    #endregion

    #region Resource
    /// <summary>
    /// Resources에서 로딩
    /// </summary>
    public static T Load<T>(string resourceName) where T : UnityEngine.Object
    {
        return Resources.Load<T>(resourceName) ?? throw new Exception($"No Resource : {resourceName}");
    }
    #endregion

    #region Game Object
    /// <summary>
    /// 게임 오브젝트의 레이어 확인
    /// </summary>
    public static bool IsLayer(GameObject contactedObject, string layerName)
    {
        return ((1 << contactedObject.layer) & (1 << LayerMask.NameToLayer(layerName))) != 0;
    }
    #endregion

    #region Transform
    /// <summary>
    /// 모든 자식 게임 오브젝트 파괴
    /// </summary>
    public static void DestoryAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    #endregion

    #region Time
    /// <summary>
    /// 초를 HH:MM:SS 형태로 반환
    /// </summary>
    public static string SecondsToHMS(float seconds)
    {
        if (seconds < 0)
        {
            return $"-{SecondsToHMS(-seconds)}";
        }

        int totalSeconds = Mathf.FloorToInt(seconds);

        int hh = totalSeconds / 3600;
        int mm = (totalSeconds % 3600) / 60;
        int ss = totalSeconds % 60;

        return $"{hh:D2}:{mm:D2}:{ss:D2}";
    }

    /// <summary>
    /// 초를 MM:SS 형태로 반환
    /// </summary>
    public static string SecondsToMS(float seconds)
    {
        if (seconds < 0)
        {
            return $"-{SecondsToMS(-seconds)}";
        }

        int totalSeconds = Mathf.FloorToInt(seconds);
        int mm = totalSeconds / 60;
        int ss = totalSeconds % 60;
        return $"{mm:D2}:{ss:D2}";
    }
    #endregion

    #region Random
    /// <summary>
    /// min과 max 사이의 랜덤 정수 값
    /// </summary>
    public static int RandomInt(int min, int max)
    {
        return Random.Range(min, max);
    }

    /// <summary>
    /// 랜덤 true or false
    /// </summary>
    public static bool RandomBool()
    {
        return Random.Range(0, 2) == 0;
    }
    #endregion

    #region Math
    #endregion

    #region JSON
    #endregion
}