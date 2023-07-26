using System;
using System.Collections.Generic;
using UnityEngine;

public class UniquePlayerPrefs : IDisposable
{
    private static UniquePlayerPrefs _instance;
    private Dictionary<string, string> _keys;
    private string _uniquePart;
    private string _delimiter;


    //=== Props ==============================================================

    protected static UniquePlayerPrefs Instance
    {
        get
        {
            if (_instance == null)
                _instance = new UniquePlayerPrefs();

            return _instance;
        }
    }


    //=== Ctor ================================================================

    public UniquePlayerPrefs(string delimiter = "_")
    {
        _delimiter = delimiter;
        _keys = new Dictionary<string, string>();
        _uniquePart = GetUniquePart();
    }


    //=== Public ==============================================================

    public void Dispose()
    {
        _keys = null;
        _instance = null;
    }


    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void DeleteKey(string key)
    {
        Instance.DeleteKeyWork(key);
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    public static float GetFloat(string key, float defaultValue = 0)
    {
        return Instance.GetFloatWork(key, defaultValue);
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return Instance.GetIntWork(key, defaultValue);
    }

    public static bool GetBool(string key, bool defaultValue = false)
    {
        return Instance.GetIntWork(key, defaultValue ? 1 : 0) == 1;
    }

    public static string GetString(string key, string defaultValue = "")
    {
        return Instance.GetStringWork(key, defaultValue);
    }

    public static long GetLong(string key, long defaultValue = 0)
    {
        return Instance.GetLongWork(key, defaultValue);
    }

    public static double GetDouble(string key, double defaultValue = 0)
    {
        return Instance.GetDoubleWork(key, defaultValue);
    }

    public static bool HasKey(string key)
    {
        return Instance.HasKeyWork(key);
    }

    public static void SetFloat(string key, float val)
    {
        Instance.SetFloatWork(key, val);
    }

    public static void SetInt(string key, int val)
    {
        Instance.SetIntWork(key, val);
    }

    public static void SetBool(string key, bool val)
    {
        Instance.SetIntWork(key, val ? 1 : 0);
    }

    public static void SetString(string key, string val)
    {
        Instance.SetStringWork(key, val);
    }

    public static void SetLong(string key, long val)
    {
        Instance.SetLongWork(key, val);
    }

    public static void SetDouble(string key, double val)
    {
        Instance.SetDoubleWork(key, val);
    }


    //=== Private =============================================================

    private string GetUniquePart()
    {
        return Application.dataPath.GetHashCode() + _delimiter;
    }

    private string GetUniqueKey(string baseKey)
    {
        string uniqueKey;
        if (!_keys.TryGetValue(baseKey, out uniqueKey))
        {
            uniqueKey = _uniquePart + baseKey;
            _keys.Add(baseKey, uniqueKey);
        }

        return uniqueKey;
    }

    private void DeleteKeyWork(string key)
    {
        PlayerPrefs.DeleteKey(GetUniqueKey(key));
    }

    private float GetFloatWork(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(GetUniqueKey(key), defaultValue);
    }

    private int GetIntWork(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(GetUniqueKey(key), defaultValue);
    }

    private string GetStringWork(string key, string defaultValue)
    {
        return PlayerPrefs.GetString(GetUniqueKey(key), defaultValue);
    }

    private long GetLongWork(string key, long defaultValue)
    {
        if (!PlayerPrefs.HasKey(GetUniqueKey(key)))
            return defaultValue;

        var asString = PlayerPrefs.GetString(GetUniqueKey(key));
        long asLong;
        if (!long.TryParse(asString, out asLong))
            return defaultValue;

        return asLong;
    }

    private double GetDoubleWork(string key, double defaultValue)
    {
        if (!PlayerPrefs.HasKey(GetUniqueKey(key)))
            return defaultValue;

        var asString = PlayerPrefs.GetString(GetUniqueKey(key));
        double asDouble;
        if (!double.TryParse(asString, out asDouble))
            return defaultValue;

        return asDouble;
    }

    private bool HasKeyWork(string key)
    {
        return PlayerPrefs.HasKey(GetUniqueKey(key));
    }

    private void SetFloatWork(string key, float val)
    {
        PlayerPrefs.SetFloat(GetUniqueKey(key), val);
    }

    private void SetIntWork(string key, int val)
    {
        PlayerPrefs.SetInt(GetUniqueKey(key), val);
    }

    private void SetStringWork(string key, string val)
    {
        PlayerPrefs.SetString(GetUniqueKey(key), val);
    }

    private void SetLongWork(string key, long val)
    {
        PlayerPrefs.SetString(GetUniqueKey(key), val.ToString());
    }

    private void SetDoubleWork(string key, double val)
    {
        PlayerPrefs.SetString(GetUniqueKey(key), val.ToString());
    }
}
