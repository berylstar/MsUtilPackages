using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum ELanguage
{
    KOR = 0,
    ENG = 1,
}

/// <summary>
/// 다국어를 위한 매니저
/// </summary>
public static class LocalizationManager
{
    public static ELanguage Language { get; private set; }

    private static readonly Dictionary<ELanguage, Dictionary<string, string>> _tables = new Dictionary<ELanguage, Dictionary<string, string>>();

    // 성능 최적화: 현재 언어의 테이블을 캐싱하여 이중 조회를 피함
    private static Dictionary<string, string> _currentTable = null;

    private static event Action OnLocalize;

    private static readonly string RESOURCE_PATH = "Localization/Localization";
    private static readonly string PREFS_LANGUAGE = "Language";
    
    private static bool _isInitialized = false;

    public static void CheckInitialization()
    {
        if (_isInitialized)
            return;

        Initialize();
    }

    /// <summary>
    /// 초기화
    /// </summary>
    public static void Initialize()
    {
        LoadCsv();

        int langPrefs = PlayerPrefs.GetInt(PREFS_LANGUAGE, 0);
        Language = (ELanguage)langPrefs;

        UpdateCurrentTable();

        _isInitialized = true;
    }

    /// <summary>
    /// 현재 언어 테이블 캐시 업데이트
    /// </summary>
    private static void UpdateCurrentTable()
    {
        if (_tables.TryGetValue(Language, out Dictionary<string, string> table))
        {
            _currentTable = table;
        }
        else
        {
            _currentTable = null;
        }
    }

    /// <summary>
    /// 언어가 변경될 때 실행할 콜백 등록
    /// </summary>
    public static void AddListener(Action callback)
    {
        CheckInitialization();
        OnLocalize += callback;
    }

    /// <summary>
    /// 언어가 변경될 때 실행할 콜백 해제
    /// </summary>
    public static void RemoveListener(Action callback)
    {
        CheckInitialization();
        OnLocalize -= callback;
    }

    /// <summary>
    /// 언어 변경
    /// </summary>
    public static void ChangeLanguage(ELanguage language)
    {
        CheckInitialization();

        if (Language == language)
            return;

        Language = language;
        UpdateCurrentTable();

        PlayerPrefs.SetInt(PREFS_LANGUAGE, (int)Language);
        PlayerPrefs.Save();

        OnLocalize?.Invoke();
    }

    /// <summary>
    /// 다국어 키로 가져오기
    /// </summary>
    public static string Get(string key)
    {
        CheckInitialization();

        if (string.IsNullOrEmpty(key))
            return string.Empty;

        // 캐싱된 테이블에서 즉시 조회 (최적화)
        if (_currentTable != null)
        {
            if (_currentTable.TryGetValue(key, out string value))
            {
                return value;
            }
        }

        return key;
    }

    /// <summary>
    /// 에디터 미리보기 등을 위해 특정 언어의 값을 직접 조회
    /// </summary>
    public static string Get(string key, ELanguage lang)
    {
        CheckInitialization();

        if (_tables.TryGetValue(lang, out var table) == true)
        {
            if (table.TryGetValue(key, out string value) == true)
            {
                return value;
            }
        }

        return "Not Found";
    }

    private static void LoadCsv()
    {
        _tables.Clear();

        TextAsset ta = Resources.Load<TextAsset>(RESOURCE_PATH);
        if (ta == null)
            throw new Exception($"CSV not found at Resources/{RESOURCE_PATH}.csv");

        string text = ta.text;
        if (string.IsNullOrEmpty(text) == false && text[0] == '\uFEFF')
            text = text.Substring(1);

        int nl = text.IndexOf('\n');
        string firstLine = nl >= 0 ? text.Substring(0, nl) : text;
        char delimiter = (firstLine.Split(';').Length > firstLine.Split(',').Length) ? ';' : ',';

        List<string[]> rows = Parse(text, delimiter);
        if (rows.Count == 0) return;

        string[] header = rows[0];
        if (header.Length < 2)
            throw new Exception("CSV WRONG HEADER");

        string header0 = (header[0] ?? "").Trim();
        if (string.IsNullOrEmpty(header0) == false && header0[0] == '\uFEFF')
            header0 = header0.Substring(1);

        if (header0.Equals("KEY", StringComparison.OrdinalIgnoreCase) == false)
            throw new Exception("CSV WRONG COLUMN");

        List<ELanguage> langs = new List<ELanguage>();
        for (int c = 1; c < header.Length; c++)
        {
            string token = (header[c] ?? "").Trim();
            if (string.IsNullOrEmpty(token)) continue;

            if (Enum.TryParse<ELanguage>(token, true, out var langEnum))
            {
                langs.Add(langEnum);
                if (_tables.ContainsKey(langEnum) == false)
                {
                    _tables[langEnum] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
            }
            else
            {
                Debug.LogWarning($"[Localization] Unknown language column: '{token}'");
            }
        }

        for (int r = 1; r < rows.Count; r++)
        {
            string[] row = rows[r];
            if (row.Length == 0) continue;

            string key = (row[0] ?? "").Trim();
            if (string.IsNullOrEmpty(key)) continue;

            for (int i = 0; i < langs.Count; i++)
            {
                int col = i + 1; // KEY 다음
                ELanguage lang = langs[i];
                string val = (col < row.Length ? row[col] : null)?.Trim() ?? "";

                _tables[lang][key] = val;
            }
        }
    }

    private static List<string[]> Parse(string csv, char delimiter = ',')
    {
        List<string[]> rows = new List<string[]>(100); // 초기 용량 설정으로 확장 비용 감소
        if (string.IsNullOrEmpty(csv))
            return rows;

        StringBuilder cur = new StringBuilder(50);
        List<string> fields = new List<string>(10);
        bool inQuotes = false;

        for (int i = 0; i < csv.Length; i++)
        {
            char ch = csv[i];

            if (inQuotes)
            {
                if (ch == '"')
                {
                    if ((i + 1) < csv.Length && csv[i + 1] == '"')
                    {
                        cur.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    cur.Append(ch);
                }
            }
            else
            {
                if (ch == '"')
                {
                    inQuotes = true;
                }
                else if (ch == delimiter)
                {
                    fields.Add(cur.ToString());
                    cur.Length = 0;
                }
                else if (ch == '\r') { /* Ignore */ }
                else if (ch == '\n')
                {
                    fields.Add(cur.ToString());
                    cur.Length = 0;
                    rows.Add(fields.ToArray());
                    fields.Clear();
                }
                else
                {
                    cur.Append(ch);
                }
            }
        }

        if (fields.Count > 0 || cur.Length > 0)
        {
            fields.Add(cur.ToString());
            rows.Add(fields.ToArray());
        }

        return rows;
    }
}