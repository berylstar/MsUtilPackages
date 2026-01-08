#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;

public class LocalizationGoogleSheetsToCSV : EditorWindow
{
    [System.Serializable]
    public class LocalizationSheet
    {
        /// <summary>
        /// 시트 이름
        /// </summary>
        public string sheetName = "Localization";

        /// <summary>
        /// 구글 시트 ID
        /// </summary>
        public string sheetId = "1iHGPbTcZTESDm-mOzjssfaKWTPRRaq4eBJdjvou8QAE";

        /// <summary>
        /// GID
        /// </summary>
        public string gid = "0";

        /// <summary>
        /// 출력 파일 이름
        /// </summary>
        public string outputFileName = "localization.csv";

        /// <summary>
        /// 활성화 여부
        /// </summary>
        public bool isEnabled = true;

        /// <summary>
        /// CSV 변환 여부
        /// </summary>
        public bool autoConvertFormat = true;
    }

    private LocalizationSheet currentSheet;

    /// <summary>
    /// 출력 폴더
    /// </summary>
    private string outputFolder = "Assets/Resources/Localization";

    private Vector2 scrollPosition;

    /// <summary>
    /// 미리보기
    /// </summary>
    private bool showPreview = false;

    /// <summary>
    /// 미리보기 내용
    /// </summary>
    private string previewContent = string.Empty;

    /// <summary>
    /// 현재 다운로드중인지
    /// </summary>
    private bool isDownloading = false;

    // 스타일 변수들
    private GUIStyle headerStyle;
    private GUIStyle boxStyle;
    private GUIStyle buttonStyle;
    private bool stylesInitialized = false;

    [MenuItem("Tools/Localization Google Sheets to CSV")]
    public static void ShowWindow()
    {
        LocalizationGoogleSheetsToCSV window = GetWindow<LocalizationGoogleSheetsToCSV>();

        window.titleContent = new GUIContent("Localization CSV Manager");
        window.minSize = new Vector2(600, 700);
    }

    private void OnEnable()
    {
        LoadSettings();
    }

    private void OnDisable()
    {
        SaveSettings();
    }

    private void OnGUI()
    {
        InitializeStyles();

        // 헤더
        EditorGUI.DrawRect(new Rect(0, 0, position.width, 70), new Color(0.2f, 0.4f, 0.8f, 1f));
        GUILayout.Space(25);
        GUILayout.Label("Google Sheets Localization CSV Manager", headerStyle);
        GUILayout.Space(25);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 다운로드 중일 땐 GUI 비활성화
        GUI.enabled = isDownloading == false;

        {
            // 사용법 및 주의사항
            DrawInstructions();

            // 기본 설정
            DrawBasicSettings();

            // Google Sheets 목록
            DrawSheetsManagement();

            // 미리보기
            DrawPreview();

            // 설정 관리
            DrawSetting();
        }

        GUI.enabled = true;

        EditorGUILayout.EndScrollView();
    }

    #region OnGUI
    private void InitializeStyles()
    {
        if (stylesInitialized)
            return;

        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 18,
            normal = { textColor = Color.white },
            alignment = TextAnchor.MiddleCenter
        };

        boxStyle = new GUIStyle("box")
        {
            padding = new RectOffset(15, 15, 10, 10),
            margin = new RectOffset(5, 5, 5, 5)
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            fixedHeight = 30
        };

        stylesInitialized = true;
    }

    private void DrawInstructions()
    {
        EditorGUILayout.BeginVertical(boxStyle);
        {
            EditorGUILayout.LabelField("📖 사용법 및 주의사항", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox
            (
                "🔗 Google Sheets 설정:\n" +
                "1. Google Sheets를 '링크가 있는 모든 사용자' 보기 권한으로 공유\n" +
                "2. 시트 URL에서 Sheet ID 복사 (예: 1iHGPbTcZTESDm-mOzjssfaKWTPRRaq4eBJdjvou8QAE)\n" +
                "3. 필요시 특정 탭의 GID 확인 (URL 끝부분의 gid=숫자)\n\n" +

                "📋 로컬라이제이션 형식:\n" +
                "• TSV → CSV 변환: 탭 구분 데이터를 쉼표 구분 + 따옴표 감싸기로 변환\n" +
                "• KEY, KOR, ENG 등의 컬럼을 가진 로컬라이제이션 시트에 최적화\n\n" +

                "⚡ 팁:\n" +
                "• 자동 새로고침을 사용하여 주기적으로 업데이트 가능 (최소 30초 간격)\n" +
                "• 미리보기 기능으로 다운로드 전 내용 확인 가능\n" +
                "• 다운로드 중에는 UI가 일시적으로 비활성화됩니다",
                MessageType.Info
            );
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawBasicSettings()
    {
        EditorGUILayout.BeginVertical(boxStyle);
        {
            EditorGUILayout.LabelField("📁 기본 설정", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("출력 폴더:", GUILayout.Width(80));
                outputFolder = EditorGUILayout.TextField(outputFolder);
                if (GUILayout.Button("📂 선택", GUILayout.Width(60)))
                {
                    SelectOutputFolder();
                }
            }
            GUILayout.EndHorizontal();

            // 폴더가 존재하지 않으면 경고 표시
            if (Directory.Exists(outputFolder) == false)
            {
                EditorGUILayout.HelpBox($"경고: 출력 폴더 '{outputFolder}'가 존재하지 않습니다. 다운로드 시 자동으로 생성됩니다.", MessageType.Warning);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void SelectOutputFolder()
    {
        string path = EditorUtility.OpenFolderPanel("CSV 출력 폴더 선택", "Assets", string.Empty);
        if (string.IsNullOrEmpty(path) == false)
        {
            if (path.StartsWith(Application.dataPath))
            {
                outputFolder = $"Assets{path.Substring(Application.dataPath.Length)}";
            }
            else
            {
                EditorUtility.DisplayDialog("경로 오류", "Unity 프로젝트 내의 폴더를 선택해주세요.", "확인");
            }
        }
    }

    private void DrawSheetsManagement()
    {
        EditorGUILayout.BeginVertical(boxStyle);
        {
            EditorGUILayout.LabelField("📊 Google Sheets 관리", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                {
                    currentSheet.isEnabled = EditorGUILayout.Toggle(currentSheet.isEnabled, GUILayout.Width(20));
                    EditorGUILayout.LabelField($"📋 시트", EditorStyles.boldLabel, GUILayout.Width(80));

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                if (currentSheet.isEnabled)
                {
                    currentSheet.sheetName = EditorGUILayout.TextField("시트 이름", currentSheet.sheetName);
                    currentSheet.sheetId = EditorGUILayout.TextField("Google Sheets ID", currentSheet.sheetId);
                    currentSheet.gid = EditorGUILayout.TextField("GID", currentSheet.gid);
                    currentSheet.outputFileName = EditorGUILayout.TextField("출력 파일명", currentSheet.outputFileName);
                    currentSheet.autoConvertFormat = EditorGUILayout.Toggle("TSV → CSV 변환", currentSheet.autoConvertFormat);

                    if (string.IsNullOrEmpty(currentSheet.outputFileName) && string.IsNullOrEmpty(currentSheet.sheetName) == false)
                    {
                        currentSheet.outputFileName = $"{currentSheet.sheetName}.csv";
                    }

                    // URL 미리보기
                    GUI.enabled = false;
                    EditorGUILayout.LabelField("다운로드 URL:", EditorStyles.miniBoldLabel);
                    EditorGUILayout.SelectableLabel(GetDownloadUrl(currentSheet), EditorStyles.helpBox);
                    GUI.enabled = true;
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("👁️ 미리보기", buttonStyle))
                {
                    _ = PreviewSheetAsync(currentSheet);
                }

                if (GUILayout.Button("⬇️ 다운로드", buttonStyle))
                {
                    _ = DownloadSheetAsync(currentSheet);
                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawSetting()
    {
        EditorGUILayout.BeginVertical(boxStyle);
        {
            EditorGUILayout.LabelField("⚙️ 설정 관리", EditorStyles.boldLabel);

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("설정 불러오기", buttonStyle))
                {
                    LoadSettings();
                }

                if (GUILayout.Button("설정 저장하기", buttonStyle))
                {
                    SaveSettings();
                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawPreview()
    {
        if (showPreview)
        {
            EditorGUILayout.BeginVertical(boxStyle);
            {
                EditorGUILayout.LabelField("👀 미리보기", EditorStyles.boldLabel);

                EditorGUILayout.BeginVertical("box");
                {
                    Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(previewContent));
                    EditorGUILayout.SelectableLabel(previewContent, GUILayout.Height(Mathf.Min(textSize.y, 200)));
                }
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("미리보기 닫기"))
                {
                    showPreview = false;
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
    #endregion

    private string GetDownloadUrl(LocalizationSheet sheet)
    {
        return $"https://docs.google.com/spreadsheets/d/{sheet.sheetId}/export?format=csv&gid={sheet.gid}";
    }

    private async Task PreviewSheetAsync(LocalizationSheet sheet)
    {
        try
        {
            isDownloading = true;
            EditorUtility.DisplayProgressBar("미리보기 로딩...", $"'{sheet.sheetName}' 시트를 가져오는 중...", 0.5f);

            string content = await DownloadSheetContentAsync(sheet);

            if (string.IsNullOrEmpty(content) == false)
            {
                string processedContent = sheet.autoConvertFormat ? ConvertToQuotedCSV(content) : content;

                // 처음 10줄만 미리보기
                string[] lines = processedContent.Split('\n');
                int previewLines = Mathf.Min(lines.Length, 10);
                previewContent = string.Join("\n", lines, 0, previewLines);
                if (lines.Length > previewLines)
                {
                    previewContent += "\n.\n.\n.";
                }

                showPreview = true;
                Repaint(); // UI 업데이트
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"미리보기 오류: {e.Message}");
            EditorUtility.DisplayDialog("미리보기 오류", $"미리보기를 가져올 수 없습니다.\n\n{e.Message}", "확인");
        }
        finally
        {
            isDownloading = false;
            EditorUtility.ClearProgressBar();
        }
    }

    private async Task DownloadSheetAsync(LocalizationSheet sheet)
    {
        try
        {
            isDownloading = true;
            EditorUtility.DisplayProgressBar("다운로드 중...", $"'{sheet.sheetName}' 시트를 다운로드하고 있습니다...", 0.5f);

            string content = await DownloadSheetContentAsync(sheet);

            if (string.IsNullOrEmpty(content) == false)
            {
                // TSV를 CSV로 변환 (필요한 경우)  
                if (sheet.autoConvertFormat)
                {
                    content = ConvertToQuotedCSV(content);
                }

                SaveToFile(sheet, content);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"다운로드 오류: {e.Message}");
            EditorUtility.DisplayDialog("다운로드 오류", $"'{sheet.sheetName}' 시트를 다운로드할 수 없습니다.\n\n{e.Message}", "확인");
        }
        finally
        {
            isDownloading = false;
            EditorUtility.ClearProgressBar();
        }
    }

    private async Task<string> DownloadSheetContentAsync(LocalizationSheet sheet)
    {
        string url = GetDownloadUrl(sheet);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = 30;
            request.SetRequestHeader("User-Agent", "Unity-LocalizationTool/1.0");

            var operation = request.SendWebRequest();

            // UnityWebRequest 완료 대기
            while (operation.isDone == false)
            {
                await Task.Delay(50);
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                string errorMessage = GetDetailedErrorMessage(request, url);
                Debug.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            string content = request.downloadHandler.text;

            // HTML 응답인지 확인 (로그인 페이지 등)
            if (content.Trim().StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase) ||
                content.Contains("<html", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("HTML 페이지를 받았습니다. Google Sheets가 공개 공유되지 않았을 가능성이 있습니다.");
            }

            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("빈 응답을 받았습니다. Sheet ID 또는 GID를 확인해주세요.");
            }

            return content;
        }
    }

    private string GetDetailedErrorMessage(UnityWebRequest request, string url)
    {
        string baseMessage = request.responseCode switch
        {
            403 => "권한 오류 (403): Google Sheets가 공개 공유되지 않았습니다.",
            404 => "찾을 수 없음 (404): Sheet ID 또는 GID가 잘못되었습니다.",
            429 => "요청 한도 초과 (429): 잠시 후 다시 시도해주세요.",
            0 => "네트워크 오류: 인터넷 연결 또는 방화벽을 확인해주세요.",
            _ => $"HTTP 오류 ({request.responseCode}): {request.error}"
        };

        return $"{baseMessage}\nURL: {url}";
    }

    private string ConvertToQuotedCSV(string tsvContent)
    {
        if (string.IsNullOrEmpty(tsvContent)) return string.Empty;

        string[] lines = tsvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        StringBuilder csvBuilder = new StringBuilder();

        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] columns = line.Split('\t');
            List<string> processedColumns = new List<string>();

            for (int columnIndex = 0; columnIndex < columns.Length; columnIndex++)
            {
                string column = columns[columnIndex].Trim();

                // 첫 번째 컬럼(KEY)은 따옴표 없이, 나머지는 따옴표로 감싸기
                if (columnIndex == 0)
                {
                    // KEY 컬럼: 따옴표 제거하고 그대로 사용
                    if (column.StartsWith("\"") && column.EndsWith("\"") && column.Length >= 2)
                    {
                        column = column.Substring(1, column.Length - 2);
                    }
                    processedColumns.Add(column);
                }
                else
                {
                    // 언어 컬럼들: 따옴표로 감싸기
                    if (column.StartsWith("\"") && column.EndsWith("\"") && column.Length >= 2)
                    {
                        // 이미 감싸져 있으면 그대로 사용
                        processedColumns.Add(column);
                    }
                    else
                    {
                        // 감싸져 있지 않으면 내부 따옴표를 이스케이프하고 감싸기
                        string escapedContent = column.Replace("\"", "\"\"");
                        processedColumns.Add($"\"{escapedContent}\"");
                    }
                }
            }

            csvBuilder.AppendLine(string.Join(",", processedColumns));
        }

        return csvBuilder.ToString();
    }

    private void SaveToFile(LocalizationSheet sheet, string content, bool showDialog = true)
    {
        try
        {
            // 출력 폴더가 없으면 생성
            if (Directory.Exists(outputFolder) == false)
            {
                Directory.CreateDirectory(outputFolder);
            }

            string filePath = Path.Combine(outputFolder, sheet.outputFileName);
            File.WriteAllText(filePath, content, Encoding.UTF8);

            // Unity에서 파일 인식하도록 새로고침
            AssetDatabase.Refresh();

            string logMessage = $"✅ '{sheet.sheetName}' 시트가 성공적으로 저장되었습니다: {filePath}";
            Debug.Log(logMessage);

            if (showDialog)
            {
                EditorUtility.DisplayDialog
                (
                    "다운로드 완료",
                    $"'{sheet.sheetName}' 시트가 성공적으로 저장되었습니다\n\n저장 위치: {filePath}",
                    "확인"
                );
            }
        }
        catch (Exception e)
        {
            string errorMessage = $"❌ '{sheet.sheetName}' 시트 저장 중 오류 발생: {e.Message}";
            Debug.LogError(errorMessage);

            if (showDialog)
            {
                EditorUtility.DisplayDialog
                (
                    "저장 오류",
                    $"'{sheet.sheetName}' 시트를 저장하는 중 오류가 발생했습니다.\n\n오류 내용: {e.Message}",
                    "확인"
                );
            }
        }
    }

    private void SaveSettings()
    {
        string savePrefs = JsonUtility.ToJson(new SettingWrapper
        {
            sheet = currentSheet,
            outputFolder = outputFolder,
        }, true);

        EditorPrefs.SetString("LocalizationGoogleSheetsToCSV", savePrefs);
    }

    private void LoadSettings()
    {
        string savedPrefs = EditorPrefs.GetString("LocalizationGoogleSheetsToCSV", string.Empty);

        if (string.IsNullOrEmpty(savedPrefs) == false)
        {
            try
            {
                SettingWrapper wrapper = JsonUtility.FromJson<SettingWrapper>(savedPrefs);
                currentSheet = wrapper.sheet ?? new();
                outputFolder = wrapper.outputFolder ?? "Assets/StreamingAssets/Localization";
            }
            catch (Exception e)
            {
                Debug.LogWarning($"설정 로드 중 오류: {e.Message}");
                currentSheet = new();
            }
        }
    }

    [System.Serializable]
    public class SettingWrapper
    {
        public LocalizationSheet sheet;
        public string outputFolder;
    }
}

#endif