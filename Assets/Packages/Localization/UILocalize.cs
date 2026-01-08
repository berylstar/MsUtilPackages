using TMPro;
using UnityEngine;

/// <summary>
/// 다국어 적용 컴포넌트
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class UILocalize : MonoBehaviour
{
    /// <summary>
    /// 다국어 키
    /// </summary>
    [SerializeField] private string key;

    private TextMeshProUGUI mainText;

    private void Awake()
    {
        if (string.IsNullOrEmpty(key))
            return;

        mainText = GetComponent<TextMeshProUGUI>();

        LocalizationManager.AddListener(OnLocalize);
    }

    private void OnEnable()
    {
        OnLocalize();
    }

    private void OnDestroy()
    {
        if (string.IsNullOrEmpty(key))
            return;

        LocalizationManager.RemoveListener(OnLocalize);
    }

    /// <summary>
    /// 언어가 바뀌었을 때 콜백
    /// </summary>
    private void OnLocalize()
    {
        if (string.IsNullOrEmpty(key))
            return;

        if (mainText == null)
        {
            mainText = GetComponent<TextMeshProUGUI>();
        }

        mainText.text = LocalizationManager.Get(key);
    }

    /// <summary>
    /// 다국어 키 새로 설정
    /// </summary>
    public void SetKey(string newKey)
    {
        key = newKey;

        OnLocalize();
    }
}
