using UnityEditor;
using UnityEngine;

namespace Wing.Tools.Edtior
{
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T s_Instance;

        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    string[] findAssets = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
                    if (findAssets == null || findAssets.Length == 0)
                    {
                        Debug.LogError($"Please create ScriptableObject typeof {typeof(T)} first...");
                    }
                    else if (findAssets.Length > 1)
                    {
                        Debug.LogError($"ScriptableObject typeof {typeof(T)} exist multiple，please check they...");
                    }
                    else
                    {
                        s_Instance = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(findAssets[0]));
                    }
                }

                return s_Instance;
            }
        }
    }
}