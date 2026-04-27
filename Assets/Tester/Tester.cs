using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tester : MonoBehaviour
{
    public IntStat coin;
    public FloatStat ticker;

    private void Start()
    {
        coin = new IntStat(0, 0, 100);

        ticker = new FloatStat(1f, 0, 1f);
        ticker.RegisterListener((_stat) =>
        {
            Debug.Log("Tick Empty " + ticker);
            _stat.SetFull();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            coin.AddValue(1);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            coin.SubtractValue(1);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            coin.MultiplyValue(2);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            coin.DivideValue(2);
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log(coin.ToString());
        }

        ticker.SubtractValue(Time.deltaTime);
    }
}