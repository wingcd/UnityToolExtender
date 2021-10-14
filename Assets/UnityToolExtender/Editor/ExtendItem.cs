using System;

namespace Wing.Tools.Editor
{
    [Serializable]
    public class ExtendItem
    {
        public string title = "Tools/";
        public string command = "";
        public string paramaters = "";
        public string workspace = "";
        public bool silence = true;

        public void Run()
        {
            command.Execute(paramaters, workspace, silence);
        }
    }
}