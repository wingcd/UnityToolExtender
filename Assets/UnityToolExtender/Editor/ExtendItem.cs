using System;
using UnityEditor;

namespace Wing.Tools.Editor
{
    [Serializable]
    public class ExtendItem
    {
        public string title = "Tools/";
        public string command = "";
        public string paramaters = "";
        public string workspace = "";
        public bool silence = false;
        public bool alert = false;
        public bool waitexit = true;
        public bool showInMenu = true;

        public void Run(string extParams = null, bool? _silence = null, bool? _alert = null, bool? waitexit = null)
        {
            var parms = paramaters;
            if (!string.IsNullOrEmpty(extParams))
            {
                parms += " " + extParams;
            }

            bool sil = silence;
            if (_silence != null)
            {
                sil = _silence.Value;
            }

            if (_alert != null && _alert.Value || _alert == null && alert)
            {
                if (!EditorUtility.DisplayDialog("提示", $"确认执行命令：{title}", "Yes", "No"))
                {
                    return;
                }
            }

            var wait = this.waitexit;
            if (waitexit != null)
            {
                wait = waitexit.Value;
            }
            command.Execute(parms, workspace, sil, wait);
        }
    }
}