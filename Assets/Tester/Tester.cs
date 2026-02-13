using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tester : MonoBehaviour
{
    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            UIPanel_TesterOne ui = UIManager.Instance.Open<UIPanel_TesterOne>();
            ui.Initialize();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            UIManager.Instance.Close<UIPanel_TesterOne>();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            //bool isActive = UIManager.Instance.TryGet(out UIPanel_TesterOne ui);

            //Debug.Log(isActive);
            //Debug.Log(ui);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            UIPanel_TesterTwo ui = UIManager.Instance.Open<UIPanel_TesterTwo>();
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            UIManager.Instance.Close<UIPanel_TesterTwo>();
        }
    }
}
