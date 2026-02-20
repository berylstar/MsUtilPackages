using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaverManager : MonoSingleton<SaverManager>
{
    [Header("Settings")]
    public string saveFileName = "Save0"; // 인스턴스화하는 이름
    public float saveCycle = 0.5f;
    public bool isBatchSave = false; // 활성화하면 자동저장기능이 꺼짐 저장을 원할때마다 doStore 함수를 호출

    private readonly Dictionary<Saver, SaveData> _saverList = new Dictionary<Saver, SaveData>();
    private readonly List<Saver> _savers = new List<Saver>();

    private Coroutine _savingCoroutine;

    public string SavePath { get; private set; }

    private bool _isSorted = false;
    private bool _isNewDataToSave;

    private WaitForSeconds _waitCycle;

    protected override void Awake()
    {
        base.Awake();

        _waitCycle = new WaitForSeconds(saveCycle);
        SetSaveName(saveFileName);
    }

    private void Start()
    {
        StartCoroutine(CoRunSaverManager());
    }

    protected override void OnDestroy()
    {
        if (_saverList.Count > 0)
        {
            foreach (SaveData data in _saverList.Values)
            {
                try
                {
                    data.Save(); // 저장 작업 수행
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error saving data during OnDestroy: {ex.Message}");
                }
            }
        }

        base.OnDestroy();
    }

    public IEnumerator CoRunSaverManager()
    {
        while (true)
        {
            if (_isNewDataToSave)
            {
                yield return _waitCycle;

                foreach (SaveData data in _saverList.Values)
                {
                    try
                    {
                        data.Save(); // 저장 작업 수행
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error saving data: {ex.Message}");
                    }
                }

                _saverList.Clear();
                _isNewDataToSave = false;
            }

            yield return new WaitUntil(() => _isNewDataToSave);
        }
    }

    public void DoStore()
    {
        StartCoroutine(CoDoStoreIE());
    }

    private IEnumerator CoDoStoreIE()
    {
        _isNewDataToSave = true;

        yield return null;
    }


    public void AddSaveDataWithStorege(Saver saver, SaveData saveData)
    {
        if (_saverList.ContainsKey(saver))
        {
            _saverList[saver] = saveData;
        }
        else
        {
            _saverList.Add(saver, saveData);
        }

        if (isBatchSave == false)
        {
            _isNewDataToSave = true;
        }
    }

    private void SetSaveName(string saveName)
    {
        SavePath = Path.Combine(Application.streamingAssetsPath, saveName);
    }

    public void AddSaverForLoad(Saver saver)
    {
        _savers.Add(saver);
        _isSorted = false;
    }

    public void LoadAll()
    {
        SortSaversByLoadOrder();

        for (int i = 0; i < _savers.Count; i++)
        {
            try
            {
                _savers[i].Load();  // 데이터를 로드
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading saver {_savers[i]}: {ex.Message}");

            }
        }
    }

    public void SaveAll()
    {
        if (_savingCoroutine != null)
        {
            StopCoroutine(_savingCoroutine);
        }

        _savingCoroutine = StartCoroutine(SaveAllIE());
    }

    private IEnumerator SaveAllIE()
    {
        SortSaversByLoadOrder();

        for (int i = 0; i < _savers.Count; i++)
        {
            try
            {
                _savers[i].Save();  // 데이터를 저장
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading saver {_savers[i]}: {ex.Message}");
            }

            yield return null;
        }

        Debug.Log("일괄저장 완료");
    }

    private void SortSaversByLoadOrder()
    {
        if (_isSorted == false)
        {
            _savers.Sort((s1, s2) => s1.LoadSaveOrder.CompareTo(s2.LoadSaveOrder));
            _isSorted = true;
        }
    }
}