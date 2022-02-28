using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Wing.Tools.Editor
{
    public class ToolExtends : ScriptableObjectSingleton<ToolExtends>
    {
        public TextAsset classTemp;
        public TextAsset methodTemp;
        public List<ExtendItem> items;

        public static string GetMethodName(string title)
        {
            return "_" + title.ToLower().GetHashCode().ToString().Replace("-", "_");
        }

        static string FixString(string value)
        {
            return value.Replace("\"", "\\\"").Replace("\\", "/");
        }
        
        public void Save()
        {
            var result = "";

            if (items.Count > 0)
            {
                List<string> list = new List<string>();
                var func = methodTemp.text;
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var funcStr = func
                        .Replace("{SHOW_MENU}", item.showInMenu ? "" : "//")
                        .Replace("{TITLE}", item.title)
                        .Replace("{METHOD_NAME}", GetMethodName(item.title));
                    list.Add(funcStr);
                }

                result = classTemp.text.Replace("{ALL_METHODS}", string.Join("\n", list));
            }

            var path = AssetDatabase.GetAssetPath(Instance) + "/../../__code__/";
            Directory.CreateDirectory(path);
            var filename = Path.Combine(path, "__extend_tools__.cs");
            File.WriteAllText(filename, result);

            EditorUtility.SetDirty(this);
            AssetDatabase.Refresh();
        }

        public static void Run(string title, string extParms = null, bool? _silence = null, bool? _alert = null, bool? _waitexit = null)
        {
            for (var i = 0; i < Instance.items.Count; i++)
            {
                var item = Instance.items[i];
                if (item != null)
                {
                    var method = GetMethodName(item.title);
                    if (method == GetMethodName(title))
                    {
                        item.Run(extParms, _silence, _alert, _waitexit);
                        return;
                    }
                }
            }
        }
    }

    public static partial class __tools__
    {
        
    }
}