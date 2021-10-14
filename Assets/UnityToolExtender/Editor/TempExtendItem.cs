using UnityEngine;

namespace Wing.Tools.Editor
{
    public class TempExtendItem: ScriptableObject
    {
        public string title;
        public string command;
        public string paramaters;
        public string workspace;
        public bool silence;

        public void From(ExtendItem item)
        {
            title = item.title;
            command = item.command;
            paramaters = item.paramaters;
            workspace = item.workspace;
            silence = item.silence;
        }

        public void To(ExtendItem item)
        {
            item.title = title;
            item.command = command;
            item.paramaters = paramaters;
            item.workspace = workspace;
            item.silence = silence;
        }

        public ExtendItem ToItem()
        {
            var item = new ExtendItem();
            To(item);
            return item;
        }
    }
}