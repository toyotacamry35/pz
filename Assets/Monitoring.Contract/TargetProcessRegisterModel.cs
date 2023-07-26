namespace Monitoring.Contract
{
    public class TargetProcessRegisterModel
    {
        public TargetProcessRegisterModel()
        {
        }

        public TargetProcessRegisterModel(int pid, string instanceName, MonitoringType[] monitoringTypes, int defaultCountersRefreshSeconds)
        {
            Pid = pid;
            InstanceName = instanceName;
            MonitoringTypes = monitoringTypes;
            DefaultCountersRefreshSeconds = defaultCountersRefreshSeconds;
        }
        
        public int Pid { get; set; }

        public string InstanceName { get; set; }

        public MonitoringType[] MonitoringTypes { get; set; }
        
        public int DefaultCountersRefreshSeconds { get; set; }

        public enum MonitoringType
        {
            Gc,
            Thread,
        }
        
        public class Result
        {
            public Result()
            {
            }

            public Result(ResultCode code)
            {
                Code = code;
            }
            
            public ResultCode Code { get; set; }
            
            public enum ResultCode
            {
                Ok,
                ProcessNotFound,
                ProcessAlreadyExists
            }
        }
    }
}