namespace GeneratedCode.VersionHelper
{
    public static class GeneratedCodeVersion
    {
        public static string GitCommitId() => ThisAssembly.GitCommitId;
    }
}

#if UNITY_5_3_OR_NEWER
internal static partial class ThisAssembly 
{
    internal const string AssemblyInformationalVersion = "";
    internal const string GitCommitId = "0000000000000000000000000000000000000000";
}    
#endif