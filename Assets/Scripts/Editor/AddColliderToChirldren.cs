using UnityEngine;
using UnityEditor;

public class AddMeshColliderToChildren
{
    [MenuItem("Tools/Add MeshCollider To Selected With Children")]
    static void AddColliders()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("请先选中一个物体");
            return;
        }

        int count = 0;
        foreach (Transform child in selected.GetComponentsInChildren<Transform>(true))
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf != null && child.GetComponent<MeshCollider>() == null)
            {
                child.gameObject.AddComponent<MeshCollider>();
                count++;
            }
        }

        Debug.Log($"已为 {count} 个子物体添加 MeshCollider。");
    }
}
