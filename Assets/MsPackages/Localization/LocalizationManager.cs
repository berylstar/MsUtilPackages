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
    private static ELanguage _language;
    public static ELanguage Language => _language;

    private static bool _isInitialized = false;
    private static readonly Dictionary<ELanguage, Dictionary<string, string>> _tables = new();

    private static event Action OnLocalize;

    private static readonly string RESOURCE_PATH = "Localization/Localization";
    private static readonly string PREFS_LANGUAGE = "Language";

    public static void Initailize()
    {
        int langPrefs = PlayerPrefs.GetInt(PREFS_LANGUAGE, 0);

        LoadCsv();

        _language = (ELanguage)langPrefs;
        OnLocalize = null;

        _isInitialized = true;
    }

    /// <summary>
    /// 언어가 변경될 때 실행할 콜백 등록
    /// </summary>
    public static void AddListener(Action callback)
    {
        if (_isInitialized == false)
            Initailize();

        OnLocalize += callback;
    }

    /// <summary>
    /// 언어가 변경될 때 실행할 콜백 해제
    /// </summary>
    public static void RemoveListener(Action callback)
    {
        if (_isInitialized == false)
            Initailize();

        OnLocalize -= callback;
    }

    /// <summary>
    /// 언어 변경
    /// </summary>
    public static void ChangeLanguage(ELanguage language)
    {
        if (_isInitialized == false)
            Initailize();

        _language = language;

        PlayerPrefs.SetInt(PREFS_LANGUAGE, (int)_language);
        PlayerPrefs.Save();

        // 등록된 콜백 발생
        OnLocalize?.Invoke();
    }

    /// <summary>
    /// 다국어 키로 가져오기
    /// </summary>
    public static string Get(string key)
    {
        if (_isInitialized == false)
            Initailize();

        if (string.IsNullOrEmpty(key))
            return string.Empty;

        // 현재 언어에서 찾기
        if (_tables.TryGetValue(_language, out var cur))
        {
            if (cur.TryGetValue(key, out string v))
            {
                return v;
            }
        }

        // 마지막 폴백: 키 그대로 반환
        return key;
    }

    private static void LoadCsv()
    {
        _tables.Clear();

        // 1) CSV 텍스트 로드 (Resources/Localization/Localization.csv)        
        TextAsset ta = Resources.Load<TextAsset>(RESOURCE_PATH);
        if (ta == null)
            throw new Exception($"CSV not found at Resources/{RESOURCE_PATH}.csv");

        // 2) BOM 제거 + 개행 정규화
        string text = ta.text;
        if (!string.IsNullOrEmpty(text) && text[0] == '\uFEFF') // BOM
            text = text.Substring(1);

        // 3) 구분자 자동 감지 (첫 줄 기준 , vs ;)
        int nl = text.IndexOf('\n');
        string firstLine = nl >= 0 ? text.Substring(0, nl) : text;
        char delimiter = (firstLine.Split(';').Length > firstLine.Split(',').Length) ? ';' : ',';

        // 4) 파싱 (CsvUtil.Parse 안에도 BOM 제거가 들어가 있어도 무방)
        var rows = Parse(text, delimiter);
        if (rows.Count == 0) return;

        // 5) 헤더 검사 (BOM/공백 이중 가드)
        var header = rows[0];
        if (header.Length < 2)
            throw new Exception("CSV WRONG HEADER");

        string header0 = (header[0] ?? "").Trim();
        if (!string.IsNullOrEmpty(header0) && header0[0] == '\uFEFF')
            header0 = header0.Substring(1);

        if (!header0.Equals("KEY", StringComparison.OrdinalIgnoreCase))
            throw new Exception($"CSV WRONG COLUMN");

        // 6) 언어 열 수집 (ELanguage로 매핑)
        var langs = new List<ELanguage>();
        for (int c = 1; c < header.Length; c++)
        {
            string token = (header[c] ?? "").Trim();
            if (string.IsNullOrEmpty(token)) continue;

            if (!Enum.TryParse<ELanguage>(token, true, out var langEnum))
            {
                Debug.LogWarning($"[Localization] Unknown language column: '{token}' (skip)");
                continue;
            }

            langs.Add(langEnum);
            if (!_tables.ContainsKey(langEnum))
                _tables[langEnum] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        // 7) 본문
        for (int r = 1; r < rows.Count; r++)
        {
            var row = rows[r];
            if (row.Length == 0) continue;

            string key = (row[0] ?? "").Trim();
            if (string.IsNullOrEmpty(key)) continue;

            for (int i = 0; i < langs.Count; i++)
            {
                int col = i + 1; // KEY 다음
                var lang = langs[i];
                string val = (col < row.Length ? row[col] : null)?.Trim() ?? "";

                // (선택) 중복 키 경고
                if (_tables[lang].ContainsKey(key))
                    Debug.LogWarning($"[Localization] Duplicate key '{key}' for {lang}");

                _tables[lang][key] = val;
            }
        }
    }

    private static List<string[]> Parse(string csv, char delimiter = ',')
    {
        var rows = new List<string[]>();
        if (string.IsNullOrEmpty(csv)) return rows;

        if (csv.Length > 0 && csv[0] == '\uFEFF')
            csv = csv.Substring(1);

        var cur = new StringBuilder();
        var fields = new List<string>();
        bool inQuotes = false;

        for (int i = 0; i < csv.Length; i++)
        {
            char ch = csv[i];

            if (inQuotes)
            {
                if (ch == '"')
                {
                    bool hasNext = (i + 1) < csv.Length;
                    if (hasNext && csv[i + 1] == '"')
                    {
                        // 이스케이프된 따옴표 ("")
                        cur.Append('"');
                        i++;
                    }
                    else
                    {
                        // 따옴표 종료
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
                else if (ch == '\r')
                {
                    // 무시하고 \n에서 줄바꿈 처리
                }
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

        // 마지막 필드/행 추가
        fields.Add(cur.ToString());
        rows.Add(fields.ToArray());
        return rows;
    }
}
