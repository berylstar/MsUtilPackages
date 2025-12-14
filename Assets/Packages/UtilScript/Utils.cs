using System;
using System.Collections.Generic;
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
    /// Resources에서 로드
    /// </summary>
    public static T Load<T>(string resourceName) where T : UnityEngine.Object
    {
        return Resources.Load<T>(resourceName) ?? throw new Exception($"No Resource : {resourceName}");
    }

    /// <summary>
    /// Resources에서 로드 시도
    /// </summary>
    public static bool TryLoad<T>(string resourceName, out T loadedResource) where T : UnityEngine.Object
    {
        loadedResource = null;

        if (string.IsNullOrEmpty(resourceName))
            return false;

        loadedResource = Resources.Load<T>(resourceName);
        return loadedResource != null;
    }
    #endregion

    #region Game Object
    /// <summary>
    /// 게임 오브젝트의 레이어 확인
    /// </summary>
    public static bool IsLayer(GameObject contactedObject, string layerName)
    {
        if (contactedObject == null)
            return false;

        int layer = LayerMask.NameToLayer(layerName);
        if (layer < 0)
            return false;

        return contactedObject.layer == layer;
    }

    /// <summary>
    /// 자신 포함 및 자식 오브젝트에서 컴포넌트 탐색
    /// </summary>
    /// <param name="root">탐색을 시작할 기준 게임 오브젝트</param>
    /// <param name="foundComponent">찾은 컴포넌트</param>
    /// <param name="includeInactive">비활성화된 자식 포함 여부</param>
    /// <returns>탐색 성공 여부</returns>
    public static bool TryGetComponentInChildren<T>(GameObject root, out T foundComponent, bool includeInactive = true) where T : Component
    {
        foundComponent = null;

        if (root == null)
            return false;

        foundComponent = root.GetComponentInChildren<T>(includeInactive);
        return foundComponent != null;
    }

    /// <summary>
    /// 자신 포함 및 자식 오브젝트에서 모든 컴포넌트 탐색
    /// </summary>
    /// <param name="root">탐색을 시작할 기준 게임 오브젝트</param>
    /// <param name="foundComponents">찾은 컴포넌트 리스트 (없으면 빈 리스트)</param>
    /// <param name="includeInactive">비활성화된 자식 오브젝트 포함 여부</param>
    /// <returns>탐색 성공 여부</returns>
    public static bool TryGetComponentsInChildren<T>(GameObject root, out List<T> foundComponents, bool includeInactive = true) where T : Component
    {
        foundComponents = null;

        if (root == null)
            return false;

        T[] components = root.GetComponentsInChildren<T>(includeInactive);

        if (components == null || components.Length == 0)
            return false;

        foundComponents = new List<T>(components);
        return true;
    }

    #endregion

    #region Transform
    /// <summary>
    /// 모든 자식 게임 오브젝트 파괴
    /// </summary>
    public static void DestroyAllChildren(Transform parent)
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
    /// min과 max 사이의 랜덤 정수 값 (max 미포함)
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

    #region String
    /// <summary>
    /// UI용 컬러 태그 감싸기
    /// </summary>
    public static string SetColorTag(string originalText, string htmlColor)
    {
        if (string.IsNullOrEmpty(originalText))
            return string.Empty;

        if (string.IsNullOrEmpty(htmlColor))
            return originalText;

        return $"<color={htmlColor}>{originalText}</color>";
    }
    #endregion
}