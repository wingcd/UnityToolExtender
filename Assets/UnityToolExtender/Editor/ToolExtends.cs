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
            return title.ToLower().GetHashCode().ToString().Replace("-", "_");
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
                    var funcStr = func.Replace("{TITLE}", item.title)
                        .Replace("{METHOD_NAME}", GetMethodName(item.title))
                        .Replace("{COMMAND}", FixString(item.command))
                        .Replace("{PARAMATERS}", FixString(item.paramaters))
                        .Replace("{WORKSPACE}", FixString(item.workspace))
                        .Replace("{SILENCE}", item.silence.ToString().ToLower());
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

        public static void Run(string title)
        {
            var type = typeof(__tools__);
            var name = "_" + GetMethodName(title);
           var methods =  type.GetMethods();
            var method = type.GetMethod(name, BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(
                    null,
                    BindingFlags.Static | BindingFlags.InvokeMethod, 
                    null, 
                    null, 
                    null
                    );
            }
        }
    }

    public static partial class __tools__
    {
        
    }
}