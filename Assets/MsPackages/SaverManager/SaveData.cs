using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public abstract class SaveData
{
    protected abstract string FileName { get; }

    /// <summary>
    /// 비동기 데이터 저장
    /// </summary>
    public async Task SaveAsync(string path)
    {
        string fullPath = Path.Combine(path, $"{FileName}.txt");
        string json = JsonUtility.ToJson(this, true); // 보기 편하게 prettyPrint 활성화

        await Task.Run(() =>
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            File.WriteAllText(fullPath, json);

            Debug.Log($"[SAVE] {fullPath}");
        });
    }

    /// <summary>
    /// 데이터 로딩
    /// </summary>
    public void Load(string path)
    {
        string fullPath = Path.Combine(path, $"{FileName}.txt");

        if (File.Exists(fullPath) == false)
        {
            // 세이브 파일 없음
            return;
        }

        string json = File.ReadAllText(fullPath);
        JsonUtility.FromJsonOverwrite(json, this); // 현재 인스턴스에 데이터 덮어쓰기

        Debug.Log($"[LOAD] {fullPath}");
    }

    /// <summary>
    /// 데이터 제거
    /// </summary>
    public void Delete(string path)
    {
        string fullPath = Path.Combine(path, $"{FileName}.txt");

        if (File.Exists(fullPath) == false)
            return;

        try
        {
            File.Delete(fullPath);

            Debug.Log($"[DELETE] {fullPath}");
        }
        catch (Exception)
        {

        }
    }
}