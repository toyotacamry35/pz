using Mono.Cecil;
using Core.Reflection;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Mono.Cecil.Cil;

namespace Assets.Tools.Editor.GitInjector
{
    class GitInjectionWeaver
    {
        [InitializeOnLoadMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members")]
        private static void Init()
        {
            var gitRev = GetCurrentRevision();
            if(!string.IsNullOrWhiteSpace(gitRev))
                CompilationPipeline.assemblyCompilationFinished += (path, messages) => OnCompilationFinished(gitRev, path, messages);
        }

        private static string GetCurrentRevision()
        {
            try
            {
                var psi = new ProcessStartInfo()
                {
                    FileName = "git",
                    WorkingDirectory = Application.dataPath,
                    Arguments = "rev-parse HEAD",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var proc = Process.Start(psi))
                {
                    var data = proc.StandardOutput.ReadLine();

                    proc.WaitForExit();
                    if (proc.ExitCode != 0)
                        return null;

                    return data;
                }
            }
            catch(Exception)
            {
                return null;
            }
        }


        private static void OnCompilationFinished(string gitRev, string targetPath, CompilerMessage[] messages)
        {
            if (messages.Any(v => v.type == CompilerMessageType.Error))
                return;

            if (string.Compare(Path.GetFileNameWithoutExtension(targetPath), "GeneratedCode", true) != 0)
                return;

            try
            {

                using (var stream = File.Open(targetPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    var resolver = new DefaultAssemblyResolver();
                    foreach (var assemblyLocation in AppDomain.CurrentDomain.GetAssembliesSafe().Where(v => !v.IsDynamic && !string.IsNullOrWhiteSpace(v.Location)).Select(v => Path.GetDirectoryName(v.Location)).Distinct())
                        resolver.AddSearchDirectory(assemblyLocation);

                    var readerParams = new ReaderParameters()
                    {
                        AssemblyResolver = resolver
                    };
                    var assembly = AssemblyDefinition.ReadAssembly(stream, readerParams);
                    var changed = false;
                    changed |= ReplaceThisAssemblyConstant(assembly, gitRev);
                    changed |= ReplaceGitCommitIdMethod(assembly, gitRev);

                    if (!changed)
                        return;

                    stream.Seek(0, SeekOrigin.Begin);
                    assembly.Write(stream);
                    stream.SetLength(stream.Position);
                }

            }
            catch(Exception e)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(e, $"TargetPath:{targetPath}");
            }
        }

        private static bool ReplaceThisAssemblyConstant(AssemblyDefinition assembly, string gitRev)
        {
            var type = assembly.MainModule.Types.SingleOrDefault(v => v.FullName == "ThisAssembly");
            if (type == null)
                return false;

            var field = type.Fields.Single(v => v.Name == "GitCommitId");
            field.Constant = gitRev;
            return true;
        }

        private static bool ReplaceGitCommitIdMethod(AssemblyDefinition assembly, string gitRev)
        {
            var type = assembly.MainModule.Types.SingleOrDefault(v => v.FullName == "GeneratedCode.VersionHelper.GeneratedCodeVersion");
            if (type == null)
                return false;

            var method = type.Methods.Single(v => v.Name == "GitCommitId");
            var existing = method.Body.Instructions[0];
            var proc = method.Body.GetILProcessor();
            var instr = proc.Create(OpCodes.Ldstr, gitRev);
            proc.Replace(existing, instr);

            return true;
        }
    }
}
