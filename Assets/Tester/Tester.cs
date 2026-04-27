using System;
using UnityEngine;
using TMPro;

public class Tester : MonoBehaviour
{
    public IntStat attack;
    public FloatStat ticker;
    public TextMeshProUGUI text;

    private void Start()
    {
        attack = new IntStat(0, 0, 100);

        ticker = new FloatStat(1f, 0, 3f);
        ticker.RegisterListener((_stat) =>
        {
            if (_stat.IsEmpty)
            {
                attack.ClearModifiers();

                _stat.SetFull();
            }
        });

        text.text = string.Format(LocalizationManager.Get("CoffinShop_RestockInfo"), 4);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            attack.AddValue(1);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            attack.SubtractValue(1);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            attack.AddModifier(new StatModifier(10, EStatModifierType.Flat, 0, 1));
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            attack.AddModifier(new StatModifier(0.5f, EStatModifierType.PercentMult, 1, 2));
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {

        }

        ticker.SubtractValue(Time.deltaTime);
    }
}