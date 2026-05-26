using UnityEditor;

namespace DataSO
{
   /// <summary>
   /// SceneFinderRestore 属于窗口管理逻辑（负责重启后恢复窗口）
   /// SceneFinderEditor 属于 场景查找器 编辑器主逻辑
   /// </summary>
    [InitializeOnLoad]
    public static class SceneFinderRestore
    {
        static SceneFinderRestore()
        {
            EditorApplication.update += RestoreWindowIfOpen;
        }

        // 如果上次记录窗口处于打开状态
        private static void RestoreWindowIfOpen()
        {
            EditorApplication.update -= RestoreWindowIfOpen;

            if (SessionState.GetBool("SceneFinder_Open", false))
            {
                SceneFinderEditor.ShowWindow();
            }
        }
    }
}