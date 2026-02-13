using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel_TesterOne : UIPanel
{
    public override EUIType UIType => EUIType.UIPanel_TesterOne;

    [SerializeField] private Button logButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Text countText;

    private int count;

    private void Start()
    {
        BindOnClickButton(logButton, ClickLogButton);
        BindOnClickButton(plusButton, ClickPlusButton);
    }

    public void Initialize()
    {
        count = 0;
        countText.text = "0";
    }

    protected override void PlayCloseSequence(Action onFinished)
    {
        StartCoroutine(CoScale(onFinished));
    }

    private IEnumerator CoScale(Action onFinished)
    {
        float time = 1f;

        while (time > 0)
        {
            time -= Time.deltaTime;

            yield return null;

            RectTransform.localScale = Vector3.one * time;
        }

        RectTransform.localScale = Vector3.zero;
        onFinished?.Invoke();
    }

    private void ClickLogButton()
    {
        Debug.Log(count);
    }

    private void ClickPlusButton()
    {
        count += 1;

        countText.text = $"{count}";
    }
}
