using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tester : MonoBehaviour
{
    public PlayData playDataOne;

    private void Start()
    {
        playDataOne = new PlayData(0, 2, 11);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            playDataOne.coin.AddValue(1);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            playDataOne.coin.AddValue(1);
            _ = playDataOne.SaveAsync(SaverManager.SavePath);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            playDataOne.Load(SaverManager.SavePath);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            playDataOne.Delete(SaverManager.SavePath);
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {

        }
    }
}

[Serializable]
public class PlayData : SaveData
{
    protected override string FileName => $"PlayerInfo_{slot}";

    private readonly int slot;

    public Stat<int> coin;
    public int stage;

    public PlayData(int slot, int coin, int stage)
    {
        this.slot = slot;
        this.coin = new Stat<int>(coin, 0, 99, IntStatOperator.Instance);
        this.stage = stage;
    }
}