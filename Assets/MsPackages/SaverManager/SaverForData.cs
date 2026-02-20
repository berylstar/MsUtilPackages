using System;
using System.Threading.Tasks;

public class SaverForData<T> : Saver
{
    public delegate void AfterSaveLoad();
    private AfterSaveLoad afterLoad;
    private AfterSaveLoad afterSave;
    public T data;

    public SaverForData(T data)
    {
        this.data = data;
    }

    /// <summary>
    /// Start에서 호출할것
    /// </summary>
    /// <param name="name">저장할 파일 이름</param>
    /// <param name="afterLoad">로드후 호출 될 함수</param>
    /// <param name="loadSaveOrder">저장,불러오기 순서</param>
    /// <param name="afterSave">저장후 호출 될 함수</param>
    public void Initialize(string name, int loadSaveOrder = 0, AfterSaveLoad afterLoad = null, AfterSaveLoad afterSave = null)
    {
        InitializeSaver(name);
        SaverManager.Instance.AddSaverForLoad(this);

        this.afterLoad = afterLoad;
        this.LoadSaveOrder = loadSaveOrder;
        this.afterSave = afterSave;
    }

    public override void Load()
    {
        data = LoadJsonData<DataCapsule<T>>().data;

        if (afterLoad != null)
            afterLoad();
    }

    public override void Save()
    {
        AddSaverData(new DataCapsule<T>(data));

        if (afterSave != null)
            afterSave();
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