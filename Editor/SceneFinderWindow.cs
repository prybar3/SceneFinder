using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataSO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;
using Object = UnityEngine.Object;

/// <summary>
/// 该死的场景全部滚出来
/// </summary>
[UnityEditor.InitializeOnLoad]
public class SceneFinderEditor : EditorWindow
{
    private string searchText = "";
    private Vector2 scrollPos;
    private List<string> allScenes;
    private double lastClickTime;
    private string lastClickedScene;

    private SceneFavoritesDataSO favoriteData;
    private const string FAVORITE_PATH = "Assets/Settings/SceneFinderFavorites.asset";

    [MenuItem("Tools/Scene Finder")] 
    public static void ShowWindow()
    {
        var window = GetWindow<SceneFinderEditor>(false, "Scene Finder", true);
        window.RefreshSceneList();
        window.LoadFavorites();
    }

    private void RefreshSceneList()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene");
        allScenes = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToList();
    }

    private void LoadFavorites()
    {
        favoriteData = AssetDatabase.LoadAssetAtPath<SceneFavoritesDataSO>(FAVORITE_PATH);

        if (favoriteData == null)
        {
            AssetDatabase.Refresh();
            favoriteData = AssetDatabase.LoadAssetAtPath<SceneFavoritesDataSO>(FAVORITE_PATH);
        }
        
        // 如果仍然加载失败，检查文件是否存在
        if (favoriteData == null)
        {
            // 先检查文件是否实际存在
            bool fileExists = File.Exists(FAVORITE_PATH);
            
            if (!fileExists)
            {
                // 文件不存在，创建新的 ScriptableObject
                Debug.LogWarning("SceneFinderFavorites.asset 不存在，自动创建一个新的");
                favoriteData = ScriptableObject.CreateInstance<SceneFavoritesDataSO>();
                favoriteData.favorites = new List<string>();
                
                string dir = Path.GetDirectoryName(FAVORITE_PATH);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                    
                AssetDatabase.CreateAsset(favoriteData, FAVORITE_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                // 再次验证创建是否成功
                favoriteData = AssetDatabase.LoadAssetAtPath<SceneFavoritesDataSO>(FAVORITE_PATH);
                if (favoriteData == null)
                {
                    Debug.LogError("创建 SceneFinderFavorites.asset 失败！请检查路径权限：" + FAVORITE_PATH);
                }
            }
            else
            {
                // 文件存在但加载失败，说明文件损坏
                Debug.LogError("SceneFinderFavorites.asset 文件存在但无法加载，可能格式损坏！\n路径：" + FAVORITE_PATH + "\n请手动删除该文件后重新打开窗口。");
            }
        }
        
        // 防止字段未初始化
        if (favoriteData != null && favoriteData.favorites == null)
            favoriteData.favorites = new List<string>();
    }

    private void SaveFavorites()
    {
        EditorUtility.SetDirty(favoriteData);
        AssetDatabase.SaveAssetIfDirty(favoriteData);
        AssetDatabase.Refresh();
        // if (favoriteData != null)
        // {
        //     EditorUtility.SetDirty(favoriteData);
        //     AssetDatabase.SaveAssetIfDirty(favoriteData);
        //     AssetDatabase.Refresh();
        // }
    }

    private void OnEnable()
    {
        SessionState.SetBool("SceneFinder_Open", true);
    }

    private void OnDisable()
    {
        SessionState.SetBool("SceneFinder_Open", false);
    }

    private void OnGUI()
    {
        if (GUI.changed)
            EditorUtility.SetDirty(this);


        if (favoriteData == null)
        {
            LoadFavorites();
            if (favoriteData == null || favoriteData.favorites == null)
            {
                EditorGUILayout.HelpBox("无法加载收藏数据。请重启 Unity 或删除损坏的 SceneFinderFavorites.asset。", MessageType.Error);
                return;
            }
        }

        if (allScenes == null)
        {
            RefreshSceneList();
        }

        if (GUILayout.Button("刷新列表"))
        {
            RefreshSceneList();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Search Scenes", EditorStyles.boldLabel);
        string newSearch = EditorGUILayout.TextField(searchText);
        if (newSearch != searchText)
        {
            searchText = newSearch;
        }

        EditorGUILayout.Space();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        IEnumerable<string> filteredScenes = allScenes
            .Where(path => string.IsNullOrEmpty(searchText) || path.ToLower().Contains(searchText.ToLower()));

        var favoriteList = filteredScenes.Where(favoriteData.favorites.Contains);
        var nonFavoriteList = filteredScenes.Where(p => !favoriteData.favorites.Contains(p));

        DrawSceneList(favoriteList, true);
        DrawSceneList(nonFavoriteList, false);

        EditorGUILayout.EndScrollView();
    }

    private void DrawSceneList(IEnumerable<string> scenePaths, bool isFavorite)
    {
        Event evt = Event.current;

        foreach (string scenePath in scenePaths)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Rect rect = EditorGUILayout.GetControlRect();
            Rect starRect = new Rect(rect.x, rect.y, 20, rect.height);
            Rect labelRect = new Rect(rect.x + 20, rect.y, rect.width - 20, rect.height);

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            EventType type = evt.GetTypeForControl(controlID);

            if (type == EventType.MouseDown && labelRect.Contains(evt.mousePosition))
            {
                if (evt.clickCount == 2)
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(scenePath);
                    }
                }
                GUIUtility.hotControl = controlID;
                evt.Use();
            }
            else if (type == EventType.MouseDrag && labelRect.Contains(evt.mousePosition))
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.paths = new string[] { scenePath };
                DragAndDrop.objectReferences = new Object[] { AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) };
                DragAndDrop.StartDrag(sceneName);
                evt.Use();
            }
            else if (type == EventType.Repaint)
            {
                GUI.Label(labelRect, sceneName, EditorStyles.label);
            }

            GUIContent starIcon = new GUIContent(isFavorite ? "★" : "☆");
            if (GUI.Button(starRect, starIcon, EditorStyles.label))
            {
                if (favoriteData.favorites.Contains(scenePath))
                    favoriteData.favorites.Remove(scenePath);
                else
                    favoriteData.favorites.Add(scenePath);
                SaveFavorites();
                Repaint();
            }
        }
    }
}
