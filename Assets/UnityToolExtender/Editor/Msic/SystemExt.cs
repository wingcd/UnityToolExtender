using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Wing.Tools.Editor
{
    public static class SystemExt
    {
        public static string Execute(this string command, string argument, string workingDir = null, bool noWindow = true, bool needWait = false)
        {
            command = command.Replace("{DATA_PATH}", Application.dataPath);

            var cmdext = Path.GetExtension(command);
            if (string.IsNullOrEmpty(cmdext))
            {
                var exts = new string[0];
#if UNITY_EDITOR_WIN
                exts = new[] {".exe", ".bat"};
#else
                exts = new[] {"", ".sh"};
#endif
                foreach (var ext in exts)
                {
                    var f = command + ext;
                    if (File.Exists(f))
                    {
                        command = f;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(command))
            {
                Debug.LogError($"command can not be empty");
                return "";
            }
            
            ProcessStartInfo start = new ProcessStartInfo(command);

            start.Arguments = argument;
            start.CreateNoWindow = noWindow;
            start.ErrorDialog = true;
            start.UseShellExecute = !noWindow;

            if (!string.IsNullOrEmpty(workingDir))
            {
                workingDir = workingDir.Replace("{DATA_PATH}", Application.dataPath);
                
                start.WorkingDirectory = workingDir;
            }
            else
            {
                start.WorkingDirectory = Application.dataPath; // typeof(SystemExt).Assembly.Location + "/../../../";
            }

            start.RedirectStandardOutput = noWindow;
            start.RedirectStandardError = noWindow;
            start.RedirectStandardInput = noWindow;
            if (noWindow)
            {
                start.StandardOutputEncoding = UTF8Encoding.UTF8;
                start.StandardErrorEncoding = UTF8Encoding.UTF8;
            }

            Process p = Process.Start(start);
            p.Exited += (sender, e) => { UnityEngine.Debug.Log("Execute " + command + " done!"); };
            p.ErrorDataReceived += (sender, e) =>
            {
                UnityEngine.Debug.LogError(e.Data);
            };
            p.OutputDataReceived += (sender, args) =>
            {
                UnityEngine.Debug.Log(args.Data);
            };

            if (needWait)
            {
                p.WaitForExit();
            }

            if (noWindow)
            {
                var ret = p.StandardOutput.ReadToEnd();
                UnityEngine.Debug.Log(ret);
                return "";
            }

            return "";
        }
    }
}