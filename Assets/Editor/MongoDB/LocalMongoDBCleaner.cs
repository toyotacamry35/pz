using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor;

public class LocalMongoDBCleaner : EditorWindow
{
    [MenuItem("Tools/Clean local mongo db", false, 100)]
    public static void Do()
    {
        for (int i = 0; i < 10; ++i)
        {
            var procs = Process.GetProcessesByName("mongod");
            if (procs.Length == 0)
                break;
            foreach (var proc in procs)
                proc.Kill();
            Thread.Sleep(1000);
        }
        var dbPath = Path.GetFullPath("db");
        if(Directory.Exists(dbPath))
            Directory.Delete(dbPath, true);
        UnityEngine.Debug.Log("Mongo DB cleaned");
    }
}
