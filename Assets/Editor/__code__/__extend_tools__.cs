using UnityEditor;
using Wing.Tools.Edtior;

namespace Wing.Tools.Editor
{
    public static partial class __tools__
    {
        [MenuItem("Tools/note pad")]
        static void _643527753()
        {
            "C:/Windows/SysWOW64/notepad.exe".Execute("__extend_tools__.cs", "D:/wing/projects/unity/wing_plugins/UnityToolExtender/Assets/Editor/__code__", true);
        }
        
        [MenuItem("Tools/cmd")]
        static void _1215264510()
        {
            "".Execute("", "", true);
        }
        
    }
}