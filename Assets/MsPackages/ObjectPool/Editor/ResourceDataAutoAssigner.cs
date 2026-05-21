using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(PoolableResourcesData))]
public class PoolableDataAutoAssigner : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Auto Assign Poolables (Match Name)"))
        {
            AssignPoolables((PoolableResourcesData)target);
        }
    }

    private void AssignPoolables(PoolableResourcesData data)
    {
        string[] searchPaths = { "Assets/_Prefabs/Pool" };
        string[] guids = AssetDatabase.FindAssets("t:GameObject", searchPaths);

        data.poolableResourceList.Clear();

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            // 파일명과 일치하는 Enum이 있는지 확인
            if (Enum.TryParse(go.name, out EPoolableType type))
            {
                PoolableResource resource = new PoolableResource
                {
                    poolableType = type,
                    poolableObject = go
                };

                data.poolableResourceList.Add(resource);
            }
        }

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        Debug.Log($"[Poolable] {data.poolableResourceList.Count}개 항목 할당 완료!");
    }
}