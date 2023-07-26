using System;
using System.Runtime.CompilerServices;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Input;
using Core.Environment.Logging.Extension;
using NLog;

namespace Uins
{
    public static class UI
    {
        public static readonly Logger LoggerDefault = LogManager.GetLogger("Default");
        public static readonly Logger Logger = LogManager.GetLogger("UI");

        public static readonly ResourceRef<InputBindingsDef> BlockedActions =
            new ResourceRef<InputBindingsDef>("/UtilPrefabs/Input/Bindings/UIBlockActions");

        public static readonly ResourceRef<InputBindingsDef> BlockedCamera =
            new ResourceRef<InputBindingsDef>("/UtilPrefabs/Input/Bindings/UIBlockCamera");

        public static readonly ResourceRef<InputBindingsDef> BlockedActionsAndMovement =
            new ResourceRef<InputBindingsDef>("/UtilPrefabs/Input/Bindings/UIBlockActionsAndMovement");

        public static readonly ResourceRef<InputBindingsDef> BlockedActionsAndCamera =
            new ResourceRef<InputBindingsDef>("/UtilPrefabs/Input/Bindings/UIBlockActionsAndCamera");

        public static readonly ResourceRef<InputBindingsDef> BlockedActionsMovementAndCamera =
            new ResourceRef<InputBindingsDef>("/UtilPrefabs/Input/Bindings/UIBlockActionsMovementAndCamera");

        public static void InfoOrDebug(Logger logger, bool isInfoNorDebug, string formatMessage, params object[] args)
        {
            if (logger == null)
                logger = Logger;

            if (isInfoNorDebug)
                logger.IfInfo()?.Message(formatMessage, args).Write();
            else
                logger.IfDebug()?.Message(formatMessage, args).Write();
        }

        public static void ErrorOrWarn(Logger logger, bool isErrorNorWarn, string formatMessage, params object[] args)
        {
            if (logger == null)
                logger = Logger;

            if (isErrorNorWarn)
                logger.IfError()?.Message(formatMessage, args).Write();
            else
                logger.IfWarn()?.Message(formatMessage, args).Write();
        }

        public static void CallerLog(string message = "", object caller = null, Logger logger = null,
            [CallerMemberName] string callerMethodName = "")
        {
            CallerLogWork(false, message, caller, logger, callerMethodName);
        }

        public static void CallerLogDefault(string message = "", [CallerMemberName] string callerMethodName = "")
        {
            CallerLogWork(false, message, null, LoggerDefault, callerMethodName);
        }

        public static void CallerLogInfo(string message = "", object caller = null, Logger logger = null,
            [CallerMemberName] string callerMethodName = "")
        {
            CallerLogWork(true, message, caller, logger, callerMethodName);
        }

        public static void CallerLogInfoDefault(string message = "", [CallerMemberName] string callerMethodName = "")
        {
            CallerLogWork(true, message, null, LoggerDefault, callerMethodName);
        }

        private static void CallerLogWork(bool isInfoNorDebug, string message, object caller, Logger logger, string callerMethodName)
        {
            var someLogger = logger ?? Logger;
            if (isInfoNorDebug)
                someLogger.IfInfo()?.Message(GetLogMessage(message, caller, callerMethodName)).Write();
            else
                someLogger.IfDebug()?.Message(GetLogMessage(message, caller, callerMethodName)).Write();
        }

        private static string GetLogMessage(string message, object caller, string callerMethodName)
        {
            return $"[{DateTime.Now:HH:mm:ss.ff}] {(caller != null ? $"<{caller.GetType()}> " : "")} {callerMethodName}(): {message}";
        }
    }
}