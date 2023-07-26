using Assets.Src.Aspects.Doings;
using GeneratedCode.Custom.Config;
using System;
using System.Collections.Generic;

public struct ConnectionParams : IEquatable<ConnectionParams>
{
    public string ConnectionAddress;
    public readonly int ConnectionPort;
    public readonly string Version;
    public readonly BotActionDef BotDef;
    public readonly string SpawnPointType;

    public readonly string Code;
    public readonly string UserId;

    public ConnectionParams With(string addr, int port) => new ConnectionParams(addr, port, Version, BotDef, SpawnPointType, Code, UserId);
    public ConnectionParams With(string devUser) => new ConnectionParams(ConnectionAddress, ConnectionPort, Version, BotDef, SpawnPointType, Code, devUser);
    public ConnectionParams(string connectionAddress, int connectionPort, string version, 
        BotActionDef botDef, string spawnPointType, string code, string userId)
    {
        ConnectionAddress = connectionAddress?.Trim();
        ConnectionPort = connectionPort;
        Version = version?.Trim();
        Code = code ?? "";
        UserId = userId ?? "";
        BotDef = botDef;
        SpawnPointType = spawnPointType;
    }

    public override bool Equals(object obj)
    {
        return obj is ConnectionParams && Equals((ConnectionParams)obj);
    }

    public bool Equals(ConnectionParams other)
    {
        return ConnectionAddress == other.ConnectionAddress &&
               ConnectionPort == other.ConnectionPort &&
               Version == other.Version &&
               Code == other.Code &&
               UserId == other.UserId;
    }

    public override int GetHashCode()
    {
        var hashCode = -996213682;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ConnectionAddress);
        hashCode = hashCode * -1521134295 + ConnectionPort.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Version);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Code);
        return hashCode;
    }

    public static bool operator ==(ConnectionParams params1, ConnectionParams params2)
    {
        return params1.Equals(params2);
    }

    public static bool operator !=(ConnectionParams params1, ConnectionParams params2)
    {
        return !(params1 == params2);
    }
}

public enum ConnectionMode
{
    player,
    bot,
    spectator
}

public struct UnityConnectionParams : IEquatable<UnityConnectionParams>
{
    public readonly string ConnectionAddress;
    public readonly int ConnectionPort;
    public readonly MapDef Map;

    public UnityConnectionParams(string connectionAddress, int connectionPort, MapDef map)
    {
        ConnectionAddress = connectionAddress.Trim();
        ConnectionPort = connectionPort;
        Map = map;
    }

    public override bool Equals(object obj)
    {
        return obj is UnityConnectionParams && Equals((UnityConnectionParams)obj);
    }

    public bool Equals(UnityConnectionParams other)
    {
        return ConnectionAddress == other.ConnectionAddress &&
               ConnectionPort == other.ConnectionPort &&
               EqualityComparer<MapDef>.Default.Equals(Map, other.Map);
    }

    public override int GetHashCode()
    {
        var hashCode = -797277182;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ConnectionAddress);
        hashCode = hashCode * -1521134295 + ConnectionPort.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<MapDef>.Default.GetHashCode(Map);
        return hashCode;
    }

    public static bool operator ==(UnityConnectionParams params1, UnityConnectionParams params2)
    {
        return params1.Equals(params2);
    }

    public static bool operator !=(UnityConnectionParams params1, UnityConnectionParams params2)
    {
        return !(params1 == params2);
    }
}