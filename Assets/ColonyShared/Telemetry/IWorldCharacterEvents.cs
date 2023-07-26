using GeneratedCode.Custom.Config;
using GeneratorAnnotations;
using SharedCode.Utils;
using System;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.Telemetry
{
    public static class WorldCharacterEvents
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger(nameof(WorldCharacterEvents));

        public static void EnteredWorld(string name, in Guid characterId, in Guid sessionId, in Vector3 position, MapDef map)
        {
            Logger.IfInfo()?.Message("Player entered world: {user_name}:{user_id}, session id {session_id} map {map} at pos {user_position}", name, characterId, sessionId, map, position).Write();
        }
        public static void LeftWorld(string name, in Guid characterId, in Guid sessionId, in Vector3 position, MapDef map)
        {
            Logger.IfInfo()?.Message("Player left world: {user_name}:{user_id}, session id {session_id} on map {map} at pos {user_position}", name, characterId, sessionId, map, position).Write();
        }

        public static void Dead(string name, in Guid characterId, in Guid sessionId, in Vector3 position, MapDef map)
        {
            Logger.IfInfo()?.Message("Player died: {user_name}:{user_id}, session id {session_id} on map {map} at pos {user_position}", name, characterId, sessionId, map, position).Write();
        }
    }
}
