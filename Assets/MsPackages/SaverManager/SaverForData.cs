using System;
using UnityEngine;

/// <summary>
/// 특정 데이터 타입(T)을 관리하고 저장/로드 로직을 처리하는 클래스
/// </summary>
/// <typeparam name="T">직렬화 가능한 데이터 타입</typeparam>
public class SaverForData<T> : Saver
{
    private event Action OnAfterLoading;
    private event Action OnAfterSaving;

    public T data;

    public SaverForData(T data)
    {
        this.data = data;
    }

    /// <summary>
    /// Saver의 기본 정보를 설정하고 매니저에 등록합니다.
    /// </summary>
    /// <param name="name">저장될 파일 이름</param>
    /// <param name="loadSaveOrder">실행 우선순위 (낮을수록 먼저 로드)</param>
    /// <param name="afterLoad">로드 완료 후 실행할 콜백</param>
    /// <param name="afterSave">저장 완료 후 실행할 콜백</param>
    public void Initialize(string name, int loadSaveOrder = 0, Action afterLoad = null, Action afterSave = null)
    {
        this.LoadSaveOrder = loadSaveOrder;
        this.OnAfterLoading = afterLoad;
        this.OnAfterSaving = afterSave;

        InitializeSaver(name);

        // 매니저 존재 여부 확인 후 안전하게 등록
        if (SaverManager.Instance != null)
        {
            SaverManager.Instance.AddSaverForLoad(this);
        }
        else
        {
            Debug.LogError($"[SaverForData] {name} 등록 실패: SaverManager 인스턴스가 존재하지 않습니다.");
        }
    }

    public override void Load()
    {
        DataCapsule<T> capsule = LoadJsonData<DataCapsule<T>>();

        // 로드된 데이터가 유효한지 확인
        if (capsule != null)
        {
            data = capsule.data;
        }
        else
        {
            Debug.LogWarning($"[SaverForData] 데이터를 불러올 수 없습니다. 기본값을 유지하거나 새로 생성합니다.");
        }

        // 콜백 호출
        OnAfterLoading?.Invoke();
    }

    public override void Save()
    {
        // 데이터가 없는 상태에서 저장을 시도하는지 체크
        if (data == null)
        {
            Debug.LogError("[SaverForData] 저장할 데이터(data)가 null입니다.");
            return;
        }

        // 부모 클래스의 비동기 작업 큐에 추가 (Fire-and-forget으로 실행)
        _ = AddSaverData(new DataCapsule<T>(data));

        // 콜백 호출
        OnAfterSaving?.Invoke();
    }
}

[Serializable]
public class DataCapsule<T>
{
    public T data;

    public DataCapsule(T data)
    {
        this.data = data;
    }
}