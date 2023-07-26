using ResourcesSystem.Loader;
using SharedCode.Aspects.Building;
using System;
using System.Runtime.CompilerServices;
using Core.Environment.Logging.Extension;

namespace SharedCode.Utils
{
    public static class BuildUtils
    {
        public class MessageClass { }
        public class ErrorClass { }
        public class DebugClass { }

        private static string CreateReport(string prefix, string callerClass, string callerMember, string callerFilePath, int callerLineNumber, string line)
        {
            var caller = (!string.IsNullOrEmpty(callerClass) || !string.IsNullOrEmpty(callerMember)) ? $"\t{callerClass}.{callerMember}()" : string.Empty;
            var path = (!string.IsNullOrEmpty(callerFilePath) || (callerLineNumber > 0)) ? $"\t{callerFilePath}:{callerLineNumber}" : string.Empty;
            var message = (!string.IsNullOrEmpty(line)) ? $"\t{line}" : string.Empty;
            return $"{prefix}{caller}{path}{message}";
        }

        public static void Report(this MessageClass message, string line, string callerClass, [CallerMemberName] string callerMember = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.IfInfo()?.Message(CreateReport("[BuildSystemMessage]", callerClass, callerMember, callerFilePath, callerLineNumber, line))
                .Write();
        }

        public static void Report(this ErrorClass error, string line, string callerClass, [CallerMemberName] string callerMember = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            logger.Error(CreateReport("[BuildSystemError]", callerClass, callerMember, callerFilePath, callerLineNumber, line));
        }

        public static void Report(this DebugClass debug, bool essential, string line, string callerClass, [CallerMemberName] string callerMember = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            if (essential || CheatDebugVerboose)
            {
                logger.IfDebug()?.Message(CreateReport("[BuildSystemDebug]", callerClass, callerMember, callerFilePath, callerLineNumber, line)).Write();
            }
        }

        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger("BuildSystem");

        private static float fencePlaceGranularity = 50.0f;
        private static string buildParamsDefPath = "/Building/Params/BuildParams";
        private static BuildParamsDef buildParamsDef = null;
        private static BuildParamsDef defaultBuildParamsDef = new BuildParamsDef();

        public static bool CheatDebugEnable { get; private set; } = true;
        public static bool CheatDebugVerboose { get; private set; } = true;

        public static bool CheatDamageEnable { get; private set; }  = false;
        public static float CheatDamageValue { get; private set; } = 0.0f;

        public static bool CheatClaimResourcesEnable { get; private set; } = false;
        public static bool CheatClaimResourcesValue { get; private set; } = true;

        public static MessageClass Message { get; } = new MessageClass();
        public static ErrorClass Error { get; } = new ErrorClass();
        public static DebugClass Debug { get; private set; } = new DebugClass();

        private static void Init()
        {
            if (buildParamsDef == null)
            {
                buildParamsDef = GameResourcesHolder.Instance?.LoadResource<BuildParamsDef>(buildParamsDefPath) ?? null;
            }
        }

        public static BuildParamsDef BuildParamsDef { get { Init(); return (buildParamsDef != null) ? buildParamsDef : defaultBuildParamsDef; } }

        public static BuildingPlaceDef DefaultBuildingPlaceDef { get { return BuildParamsDef.DefaultBuildingPlaceDef.Target; } }

        public static FencePlaceDef DefaultFencePlaceDef { get { return BuildParamsDef.DefaultFencePlaceDef.Target; } }

        public static Vector2Int GetFencePlaceKey(Vector3 position)
        {
            return new Vector2Int((int)Math.Floor(position.x / fencePlaceGranularity), (int)Math.Floor(position.z / fencePlaceGranularity));
        }

        public static Vector3 GetFencePlacePosition(Vector2Int key, float y)
        {
            return new Vector3((key.x + 0.5f) * BuildUtils.fencePlaceGranularity, y, (key.y + 0.5f) * BuildUtils.fencePlaceGranularity);
        }

        public static void SetCheatDebug(bool enable, bool verboose)
        {
            if (CheatDebugEnable != enable)
            {
                CheatDebugEnable = enable;
                Debug = CheatDebugEnable ? new DebugClass() : null;
            }
            CheatDebugVerboose = verboose;
        }

        public static void SetCheatDamage(bool enable, float value)
        {
            CheatDamageEnable = enable;
            CheatDamageValue = value;
        }

        public static void SetCheatClaimResources(bool enable, bool value)
        {
            CheatClaimResourcesEnable = enable;
            CheatClaimResourcesValue = value;
        }
    }
}
