using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;

namespace Assets.Test.Src.Editor
{
    public static class GitCacheStatus
    {
        private static string RetrieveCommit()
        {
            return CommandOutput("git rev-parse HEAD", Directory.GetCurrentDirectory());
        }

        public static string CurrentCacheVersion()
        {
            var command = RetrieveCommit().Replace("\r","").Replace("\n","");
            return command.Contains("fatal") || command.Contains("exception") || command.Contains("error") ? "" : command;
        }

        public static string CommandOutput(
            string command,
            string workingDirectory = null)
        {
            try
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command);

                procStartInfo.RedirectStandardError = procStartInfo.RedirectStandardInput = procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                if (null != workingDirectory)
                {
                    procStartInfo.WorkingDirectory = workingDirectory;
                }

                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                StringBuilder sb = new StringBuilder();
                proc.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) { sb.AppendLine(e.Data); };
                proc.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) { sb.AppendLine(e.Data); };

                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                proc.WaitForExit();
                return sb.ToString();
            }
            catch (Exception objException)
            {
                return $"Error in command: {command}, {objException.Message}";
            }
        }
    }
}