using System;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Src.Lib.ProfileTools
{
	public class Profile
	{
        //public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        //private static Stopwatch debugTimer = new Stopwatch();

        public static UnityEngine.Object[] FindObjectsOfTypeAll(Type type)
        {
            //debugTimer.Restart();

            var result = Resources.FindObjectsOfTypeAll(type);

            //var seconds = debugTimer.ElapsedMilliseconds / 1000.0f;
            //UnityEngine.Debug.LogError($"Profile, time: {seconds} sec, FindObjectsOfTypeAll {type.Name}");

            return result;
        }

        public static T[] FindObjectsOfTypeAll<T>() where T : UnityEngine.Object
        {
            //debugTimer.Restart();

            var result = Resources.FindObjectsOfTypeAll<T>();

            //var seconds = debugTimer.ElapsedMilliseconds / 1000.0f;
            //UnityEngine.Debug.LogError($"Profile, time: {seconds} sec, FindObjectsOfTypeAll {typeof(T).Name}");

            return result;
        }

        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            //debugTimer.Restart();

            var result = Resources.Load<T>(path);

            //var seconds = debugTimer.ElapsedMilliseconds / 1000.0f;
            //UnityEngine.Debug.LogError($"Profile, time: {seconds} sec, Load {((result != null) ? result.GetType().Name : "null")}: {path}");

            return result;
        }

        public static T[] LoadAll<T>(string path) where T : UnityEngine.Object
        {
            //debugTimer.Restart();

            var result = Resources.LoadAll<T>(path);
            //var seconds = debugTimer.ElapsedMilliseconds / 1000.0f;
            //UnityEngine.Debug.LogError($"Profile, time: {seconds} sec, LoadAll {((result != null) ? result.GetType().Name : "null")}: {path}");

            return result;
        }
    }
}