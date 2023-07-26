#if UNITY_EDITOR
namespace UnityAsyncAwaitUtil.Editor
{
    [UnityEditor.InitializeOnLoad]
    internal static class EditorSyncContextReplacer
    {
        static EditorSyncContextReplacer()
        {
            UnitySynchronizationContext2.Replace();
        }
    }
}
#endif
