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

    public override void OnOpen()
    {
        this.gameObject.SetActive(true);
    }

    public override void OnClose()
    {
        this.gameObject.SetActive(false);
    }

    public void Initialize()
    {
        count = 0;
        countText.text = "0";
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
