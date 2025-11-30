using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    /// 현재 씬 타입 반환
    /// </summary>
    public static EScene CurrentScene => Enum.TryParse(SceneManager.GetActiveScene().name, out EScene scene) ? scene : default;

    /// <summary>
    /// 씬 전환
    /// </summary>
    public static void LoadScene(EScene sceneName)
    {
        SceneManager.LoadScene((int)sceneName);
    }
    #endregion

    #region Game Object
    /// <summary>
    /// Resources에서 로딩
    /// </summary>
    public static T Load<T>(string _resourceName) where T : UnityEngine.Object
    {
        return Resources.Load<T>(_resourceName) ?? throw new Exception($"No Resource : {_resourceName}");
    }

    /// <summary>
    /// 게임 오브젝트의 레이어 확인
    /// </summary>
    public static bool IsLayer(this GameObject contactedObject, string layerName)
    {
        return ((1 << contactedObject.layer) & (1 << LayerMask.NameToLayer(layerName))) != 0;
    }
    #endregion
}