using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Debug = UnityEngine.Debug;

namespace Wing.Tools.Edtior
{
    public static class SystemExt
    {
        public static string Execute(this string command, string argument, string workingDir = null, bool noWindow = true, bool needWait = false)
        {
            if (string.IsNullOrEmpty(command))
            {
                Debug.LogError($"command can not be empty");
                return "";
            }
            
            ProcessStartInfo start = new ProcessStartInfo(command);

            start.Arguments = argument;
            start.CreateNoWindow = noWindow;
            start.ErrorDialog = true;
            start.UseShellExecute = false;

            if (!string.IsNullOrEmpty(workingDir))
            {
                start.WorkingDirectory = workingDir;
            }
            else
            {
                start.WorkingDirectory = typeof(SystemExt).Assembly.Location + "/../../../";
            }

            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = UTF8Encoding.UTF8;
            start.StandardErrorEncoding = UTF8Encoding.UTF8;

            Process p = Process.Start(start);
            p.Exited += (sender, e) => { UnityEngine.Debug.Log("Execute " + command + " done!"); };
            p.ErrorDataReceived += (sender, e) => { UnityEngine.Debug.LogError(e.Data); };

            if (needWait)
            {
                var ret = p.StandardOutput.ReadToEnd();
                UnityEngine.Debug.Log(ret);
                return ret;
            }

            return "";
        }
    }
}