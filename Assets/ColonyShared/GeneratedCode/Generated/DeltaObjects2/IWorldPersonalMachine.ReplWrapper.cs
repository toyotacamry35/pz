// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using Newtonsoft.Json;
using ProtoBuf;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldPersonalMachine
    {
        private static readonly NLog.Logger SystemLogger = NLog.LogManager.GetLogger("EntitySystem");
        public override IDeltaObject GetReplicationLevel(SharedCode.EntitySystem.ReplicationLevel replicationLevel)
        {
            switch (replicationLevel)
            {
                case SharedCode.EntitySystem.ReplicationLevel.Master:
                    return this;
                case SharedCode.EntitySystem.ReplicationLevel.Always:
                    return Always_Wrap;
                case SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast:
                    return ClientBroadcast_Wrap;
                case SharedCode.EntitySystem.ReplicationLevel.ClientFullApi:
                    return ClientFullApi_Wrap;
                case SharedCode.EntitySystem.ReplicationLevel.ClientFull:
                    return ClientFull_Wrap;
                case SharedCode.EntitySystem.ReplicationLevel.ServerApi:
                    return ServerApi_Wrap;
                case SharedCode.EntitySystem.ReplicationLevel.Server:
                    return Server_Wrap;
            }

            Core.Environment.Logging.Extension.LoggerExtensions.IfError(SystemLogger)?.Message("GetReplicationLevel unknown ReplicationLevel {0} for obj of type {1}", replicationLevel, GetType()).Write();
            return null;
        }

        private WorldPersonalMachineAlways _Always_Wrap;
        [ProtoIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public WorldPersonalMachineAlways Always_Wrap
        {
            get
            {
                if (_Always_Wrap == null)
                    _Always_Wrap = new WorldPersonalMachineAlways(this);
                return _Always_Wrap;
            }
        }

        private WorldPersonalMachineClientBroadcast _ClientBroadcast_Wrap;
        [ProtoIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public WorldPersonalMachineClientBroadcast ClientBroadcast_Wrap
        {
            get
            {
                if (_ClientBroadcast_Wrap == null)
                    _ClientBroadcast_Wrap = new WorldPersonalMachineClientBroadcast(this);
                return _ClientBroadcast_Wrap;
            }
        }

        private WorldPersonalMachineClientFullApi _ClientFullApi_Wrap;
        [ProtoIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public WorldPersonalMachineClientFullApi ClientFullApi_Wrap
        {
            get
            {
                if (_ClientFullApi_Wrap == null)
                    _ClientFullApi_Wrap = new WorldPersonalMachineClientFullApi(this);
                return _ClientFullApi_Wrap;
            }
        }

        private WorldPersonalMachineClientFull _ClientFull_Wrap;
        [ProtoIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public WorldPersonalMachineClientFull ClientFull_Wrap
        {
            get
            {
                if (_ClientFull_Wrap == null)
                    _ClientFull_Wrap = new WorldPersonalMachineClientFull(this);
                return _ClientFull_Wrap;
            }
        }

        private WorldPersonalMachineServerApi _ServerApi_Wrap;
        [ProtoIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public WorldPersonalMachineServerApi ServerApi_Wrap
        {
            get
            {
                if (_ServerApi_Wrap == null)
                    _ServerApi_Wrap = new WorldPersonalMachineServerApi(this);
                return _ServerApi_Wrap;
            }
        }

        private WorldPersonalMachineServer _Server_Wrap;
        [ProtoIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public WorldPersonalMachineServer Server_Wrap
        {
            get
            {
                if (_Server_Wrap == null)
                    _Server_Wrap = new WorldPersonalMachineServer(this);
                return _Server_Wrap;
            }
        }
    }
}