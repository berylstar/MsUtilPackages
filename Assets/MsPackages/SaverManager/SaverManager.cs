using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaverManager
{
    private static readonly List<SaveData> _registeredDatas = new List<SaveData>();

    private static string _savePath = string.Empty;
    public static string SavePath
    {
        get
        {
            if (string.IsNullOrEmpty(_savePath))
                Initialize();

            return _savePath;
        }
    }

    private static bool _isInitialized = false;

    private static void Initialize()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;

        _savePath = Path.Combine(Application.persistentDataPath, "SaveData");

        if (Directory.Exists(SavePath) == false)
        {
            Directory.CreateDirectory(SavePath);
        }
    }

    public static void Register(SaveData data)
    {
        if (data == null)
            return;

        // 중복 등록 방지
        if (_registeredDatas.Contains(data) == false)
        {
            _registeredDatas.Add(data);
        }
    }

    public static void UnRegister(SaveData data)
    {
        if (data == null)
            return;

        if (_registeredDatas.Contains(data))
        {
            _registeredDatas.Remove(data);
        }
    }

    public static void ClearAll()
    {
        _registeredDatas.Clear();
    }

    public static void LoadAll()
    {
        for (int i = 0; i < _registeredDatas.Count; i++)
        {
            _registeredDatas[i].Load(SavePath);
        }
    }

    public static async void SaveAll()
    {
        for (int i = 0; i < _registeredDatas.Count; i++)
        {
            // 리스트가 변경될 수 있으므로 안전하게 접근
            if (_registeredDatas[i] != null)
            {
                await _registeredDatas[i].SaveAsync(SavePath);
            }
        }

        Debug.Log("모든 데이터 저장 완료");
    }
}