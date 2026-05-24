using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class PixelSpriteSettingTool
{
    // 공통 설정 적용 메서드
    private static void ApplyCommonSettings(TextureImporter importer)
    {
        // Texture Type을 Sprite로 변경
        importer.textureType = TextureImporterType.Sprite;

        // PPU 16 설정
        importer.spritePixelsPerUnit = 16;

        // Read/Write 해제
        importer.isReadable = false;

        // Filter Mode: Point (No Filter)
        importer.filterMode = FilterMode.Point;

        // Max Size : 256
        importer.maxTextureSize = 256;

        // Compression: None
        importer.textureCompression = TextureImporterCompression.Uncompressed;
    }

    // Single 모드
    [MenuItem("Assets/Sprite/Apply Pixel Settings (Single)")]
    private static void ApplyPixelSettings()
    {
        // 현재 선택된 오브젝트들의 가이드 (에셋들)
        Object[] selectedObjects = Selection.objects;

        if (selectedObjects.Length == 0)
            return;

        foreach (Object obj in selectedObjects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            // 선택한 파일이 텍스처(이미지)인 경우에만 실행
            if (importer != null)
            {
                ApplyCommonSettings(importer);

                // Sprite Mode Single 설정
                importer.spriteImportMode = SpriteImportMode.Single;

                // 변경사항 저장 및 리임포트
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();

                Debug.Log($"{obj.name} Single 픽셀 설정 완료");
            }
        }
    }

    // Multiple 모드 및 16x16 슬라이스 기능
    [MenuItem("Assets/Sprite/Apply Pixel Settings (Multiple 16x16)")]
    private static void ApplyPixelSettingsMultiple()
    {
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects.Length == 0)
            return;

        foreach (Object obj in selectedObjects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            bool isImporterNull = importer == null;
            if (isImporterNull == false)
            {
                ApplyCommonSettings(importer);

                importer.spriteImportMode = SpriteImportMode.Multiple;

                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();

                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                bool isTextureValid = texture != null;

                if (isTextureValid == false) continue;

                int cellSize = 16;
                int cols = texture.width / cellSize;
                int rows = texture.height / cellSize;
                int totalSprites = cols * rows;

                // SpriteMetaData 대신 SpriteRect 배열 사용
                SpriteRect[] spriteRects = new SpriteRect[totalSprites];
                int index = 0;

                for (int y = rows - 1; y >= 0; y--)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        SpriteRect rect = new SpriteRect();
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.alignment = SpriteAlignment.Center;
                        rect.name = $"{obj.name}_{index}";
                        rect.rect = new Rect(x * cellSize, y * cellSize, cellSize, cellSize);

                        // [추가됨] 최신 규격에서는 각 스프라이트 조각마다 고유 ID(GUID)가 필수입니다.
                        rect.spriteID = GUID.Generate();

                        spriteRects[index] = rect;
                        index++;
                    }
                }

                // spritesheet 프로퍼티 대신 최신 DataProvider 패턴 적용
                SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
                factory.Init();
                ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
                dataProvider.InitSpriteEditorDataProvider();

                // 슬라이스된 배열을 프로바이더에 덮어씌우기
                dataProvider.SetSpriteRects(spriteRects);
                dataProvider.Apply();

                // 슬라이스 정보 최종 적용
                importer.SaveAndReimport();

                Debug.Log($"{obj.name} 16x16 Multiple 픽셀 설정 완료");
            }
        }
    }
}