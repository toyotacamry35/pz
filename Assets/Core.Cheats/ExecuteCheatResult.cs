using ProtoBuf;

namespace Core.Cheats
{
    public static class ExecuteCheatResultExts
    {
        public static bool IsSuccess(this ExecuteCheatResult ecr) => ecr.Status == ExecuteCheatResult.StatusCode.Ok;
    }

    [ProtoContract]
    public struct ExecuteCheatResult
    {
        public enum StatusCode : byte
        {
            Ok,
            EntityNotFound,
            CheatNotFound,
            NotAuthorized,
            ArgumentsCountError,
            CheatParseError,
            ExecutionError
        }

        public static ExecuteCheatResult Success(object executeInfo)
        {
            return new ExecuteCheatResult(StatusCode.Ok, executeInfo?.ToString() ?? string.Empty);
        }

        public static ExecuteCheatResult Fail(StatusCode resultCode, object executeInfo = default)
        {
            return new ExecuteCheatResult(resultCode, executeInfo?.ToString() ?? string.Empty);
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Result))
                return $"[ExecuteCheatResult: {Status}]";
            else
                return $"[ExecuteCheatResult: {Status}, Msg: {Result}]";
        }

        public ExecuteCheatResult(StatusCode resultCode, string executeInfo)
        {
            Result = executeInfo;
            Status = resultCode;
        }


        [ProtoMember(1)] public StatusCode Status { get; set; }

        [ProtoMember(2)]
        public string Result { get; set; }
    }
}
