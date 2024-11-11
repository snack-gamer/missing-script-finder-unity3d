using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Helpers
{
    public class MissingScriptFinder : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool searchInScene = true;
        private bool searchInPrefabs = true;
        private bool showFullPath = false;
        private readonly List<GameObject> objectsWithMissingScripts = new List<GameObject>();

        [MenuItem("Tools/Missing Script Finder")]
        public static void ShowWindow()
        {
            GetWindow<MissingScriptFinder>("Missing Script Finder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Missing Script Finder", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            searchInScene = EditorGUILayout.Toggle("Search in Scene", searchInScene);
            searchInPrefabs = EditorGUILayout.Toggle("Search in Prefabs", searchInPrefabs);
            showFullPath = EditorGUILayout.Toggle("Show Full Path", showFullPath);

            EditorGUILayout.Space();

            if (GUILayout.Button("Find Missing Scripts"))
            {
                FindMissingScripts();
            }

            if (objectsWithMissingScripts.Count > 0 && GUILayout.Button("Clear Results"))
            {
                objectsWithMissingScripts.Clear();
            }

            EditorGUILayout.Space();

            if (objectsWithMissingScripts.Count > 0)
            {
                GUILayout.Label($"Found {objectsWithMissingScripts.Count} objects with missing scripts:", EditorStyles.boldLabel);
            
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
                foreach (var obj in objectsWithMissingScripts.Where(o => o != null))
                {
                    EditorGUILayout.BeginHorizontal();
                
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                    EditorGUI.EndDisabledGroup();

                    var displayText = showFullPath ? GetFullPath(obj) : obj.name;
                    EditorGUILayout.LabelField(displayText);

                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        Selection.activeObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            
                EditorGUILayout.EndScrollView();
            }
        }

        private void FindMissingScripts()
        {
            objectsWithMissingScripts.Clear();

            if (searchInScene)
            {
                var sceneObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (GameObject obj in sceneObjects)
                {
                    if (PrefabUtility.IsPartOfPrefabAsset(obj))
                        continue;

                    if (HasMissingScripts(obj))
                        objectsWithMissingScripts.Add(obj);
                }
            }

            if (searchInPrefabs)
            {
                var prefabPaths = AssetDatabase.GetAllAssetPaths()
                    .Where(path => path.EndsWith(".prefab"))
                    .ToArray();

                foreach (var path in prefabPaths)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null && HasMissingScripts(prefab))
                        objectsWithMissingScripts.Add(prefab);
                }
            }
        }

        private bool HasMissingScripts(GameObject obj)
        {
            var components = obj.GetComponents<Component>();
        
            if (components.Any(component => component == null))
                return true;

            foreach (Transform child in obj.transform)
            {
                if (HasMissingScripts(child.gameObject))
                    return true;
            }

            return false;
        }

        private string GetFullPath(GameObject obj)
        {
            var path = obj.name;
            var parent = obj.transform.parent;
        
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            if (PrefabUtility.IsPartOfPrefabAsset(obj))
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                path = "Prefab: " + assetPath;
            }

            return path;
        }
    }
}