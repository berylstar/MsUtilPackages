using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tester : MonoBehaviour
{
    private int test01;
    public IntStat coin;
    [SerializeField] private Vector3 test02;

    private void Start()
    {
        coin = new IntStat(0, 0, 100);
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

        }
    }
}