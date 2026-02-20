using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Saver
{
    public int LoadSaveOrder { get; protected set; }

    private SaveData _saveData;

    private static readonly Queue<Func<Task>> _taskQueue = new Queue<Func<Task>>(); // 작업을 순차적으로 처리하기 위한 큐
    private static bool isProcessingQueue = false; // 작업 대기열 처리 중인지 여부

    public void InitializeSaver(string name)
    {
        _saveData = new SaveData(SaverManager.Instance.SavePath, name);
    }

    /// <summary>
    /// 데이터를 JSON으로 변환하여 저장 큐에 추가합니다.
    /// </summary>
    public async Task AddSaverData<T>(T data)
    {
        // 작업을 큐에 추가
        _taskQueue.Enqueue(() => ProcessSaverData(data));

        // 현재 처리 중이 아니라면 큐 가동
        if (isProcessingQueue == false)
        {
            isProcessingQueue = true;
            await ProcessQueue();
        }
    }

    // 큐에서 작업을 하나씩 처리하는 메서드
    private static async Task ProcessQueue()
    {
        while (_taskQueue.Count > 0)
        {
            // 큐에서 다음 작업을 꺼내어 실행
            Func<Task> taskToRun = _taskQueue.Dequeue();

            if (taskToRun != null)
            {
                await taskToRun();
            }
        }

        isProcessingQueue = false;
    }

    // 실제 데이터를 처리하는 비동기 작업
    private async Task ProcessSaverData<T>(T data)
    {
        await Task.Run(() =>
        {
            try
            {
                _saveData.SetJson(JsonUtility.ToJson(data));

                SaverManager.Instance.AddSaveDataWithStorege(this, _saveData);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Data saving operation was canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving data: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// 저장된 파일을 읽어와 객체로 역직렬화합니다.
    /// </summary>
    public T LoadJsonData<T>()
    {
        string fullPath = _saveData.GetFullPath();

        if (File.Exists(fullPath) == false)
        {
            return default(T);
        }

        try
        {
            string json = File.ReadAllText(fullPath);
            return JsonUtility.FromJson<T>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"파일 읽기 중 오류 발생 ({fullPath}): {ex.Message}");
            return default(T);
        }
    }

    public abstract void Save();

    public abstract void Load();
}