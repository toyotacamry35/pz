using Harmony;

namespace Assets.ColonyShared.SharedCode.Scribe
{
    public interface IScribeWriter
    {
        void ConnectToCode(HarmonyInstance harmony);
    }
}