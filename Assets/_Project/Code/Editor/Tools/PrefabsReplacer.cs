using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class PrefabsReplacer : EditorWindow
    {
        private GameObject _newPrefab;

        [MenuItem("Window/ProjectTools/PrfabsReplacer")]
        public static void ShowWindow()
        {
            GetWindow<PrefabsReplacer>("PrfabsReplacer");
        }

        private void OnGUI()
        {
            GUILayout.Label("PrefabReplacer", EditorStyles.label);
            DrawPrefabField();
            EditorGUILayout.Space();
            DrawReplaceSelectedButton();
        }

        private void DrawPrefabField()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("New prefab", GUILayout.MaxWidth(128));
            _newPrefab = (GameObject)EditorGUILayout.ObjectField(_newPrefab,
                typeof(GameObject), true);
            GUILayout.EndHorizontal();
        }

        private void DrawReplaceSelectedButton()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Replace prefabs"))
            {
                Debug.Log("Replacing");
                ReplaceSelectedObjects();
            }
            GUILayout.EndHorizontal();
        }

        private void ReplaceSelectedObjects()
        {
            foreach (GameObject objectToReplace in Selection.gameObjects)
            {
                ReplaceObject(objectToReplace);
            }
        }

        private void ReplaceObject(GameObject objectToReplace)
        {
            if (_newPrefab == null)
            {
                Debug.Log("Prefab not found");
                return;
            }
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(_newPrefab, objectToReplace.transform);
            newObject.transform.localPosition = Vector3.zero;
            newObject.transform.localScale = Vector3.one;
            newObject.transform.SetParent(objectToReplace.transform.parent);
            newObject.name = objectToReplace.name;
            int hierarchyIndex = objectToReplace.transform.GetSiblingIndex();
            newObject.transform.SetSiblingIndex(hierarchyIndex + 1);
            GameObject.DestroyImmediate(objectToReplace);
        }
    }
}

