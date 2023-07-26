﻿using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace SharedCode.Utils
{
    public static partial class EventHelper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static async Task InvokeAsync(this Func<Task> ev)
        {
            if (ev == null)
                return;
            var invocationList = ev.GetInvocationList();
            for (int i = 0; i < invocationList.Length; ++i)
            {
                var subscriber = (Func<Task>)invocationList[i];

                try
                {
                    await subscriber();
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }

        public static async Task InvokeAsync<T>(this Func<T, Task> ev, T arg)
        {
            if (ev == null)
                return;
            var invocationList = ev.GetInvocationList();
            for (int i = 0; i < invocationList.Length; ++i)
            {
                var subscriber = (Func<T, Task>)invocationList[i];

                try
                {
                    await subscriber(arg);
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }
        public static async Task InvokeAsync<T1, T2>(this Func<T1, T2, Task> ev, T1 arg1, T2 arg2)
        {
            if (ev == null)
                return;
            var invocationList = ev.GetInvocationList();
            for (int i = 0; i < invocationList.Length; ++i)
            {
                var subscriber = (Func<T1, T2, Task>)invocationList[i];

                try
                {
                    await subscriber(arg1, arg2);
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }

        public static async Task InvokeAsync<T1, T2, T3>(this Func<T1, T2, T3, Task> ev, T1 arg1, T2 arg2, T3 arg3)
        {
            if (ev == null)
                return;
            var invocationList = ev.GetInvocationList();
            for (int i = 0; i < invocationList.Length; ++i)
            {
                var subscriber = (Func<T1, T2, T3, Task>)invocationList[i];

                try
                {
                    await subscriber(arg1, arg2, arg3);
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }

        public static async Task InvokeAsync<T1, T2, T3, T4>(this Func<T1, T2, T3, T4, Task> ev, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (ev == null)
                return;
            var invocationList = ev.GetInvocationList();
            for (int i = 0; i < invocationList.Length; ++i)
            {
                var subscriber = (Func<T1, T2, T3, T4, Task>)invocationList[i];

                try
                {
                    await subscriber(arg1, arg2, arg3, arg4);
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5, Task> ev, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (ev == null)
                return;
            var invocationList = ev.GetInvocationList();
            for (int i = 0; i < invocationList.Length; ++i)
            {
                var subscriber = (Func<T1, T2, T3, T4, T5, Task>)invocationList[i];

                try
                {
                    await subscriber(arg1, arg2, arg3, arg4, arg5);
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }
    }
}
