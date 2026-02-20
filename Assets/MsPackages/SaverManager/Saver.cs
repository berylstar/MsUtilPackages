using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Saver
{
    public int LoadSaveOrder = 0;

    private SaveData _saveData;

    private static Queue<Func<Task>> taskQueue = new Queue<Func<Task>>(); // 작업을 순차적으로 처리하기 위한 큐
    private static bool isProcessingQueue = false; // 작업 대기열 처리 중인지 여부

    public void InitializeSaver(string name)
    {
        _saveData = new SaveData(SaverManager.Instance.SavePath, name);
    }

    public async Task AddSaverData<T>(T data)
    {
        // 작업을 큐에 추가
        taskQueue.Enqueue(() => ProcessSaverData(data));

        // 만약 현재 작업이 진행 중이 아니라면, 큐 처리 시작
        if (isProcessingQueue == false)
        {
            isProcessingQueue = true;
            await ProcessQueue();
        }
    }

    // 큐에서 작업을 하나씩 처리하는 메서드
    private static async Task ProcessQueue()
    {
        while (taskQueue.Count > 0)
        {
            // 큐에서 다음 작업을 꺼내어 실행
            var taskToRun = taskQueue.Dequeue();
            await taskToRun();
        }

        isProcessingQueue = false; // 모든 작업이 완료되면 처리 중 상태를 false로 변경
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

    public T LoadJsonData<T>()
    {
        string fullPath = _saveData.GetFullPath();

        // 파일 존재 여부 확인 후 읽기 (안정성을 위해 권장)
        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            return JsonUtility.FromJson<T>(json);
        }

        return default(T);
    }

    public abstract void Save();

    public abstract void Load();
}