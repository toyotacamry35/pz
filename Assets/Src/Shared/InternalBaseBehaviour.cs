using System;
using System.Reflection;
using Core.Environment.Logging.Extension;
using NLog;
using NLog.Fluent;

namespace Assets.Src.Shared
{
    public class InternalBaseBehaviour : UnityEngine.MonoBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [NonSerialized]
        public bool Enabled = true;

        private bool IsOverriden(string name)
        {
            var method = this.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
            var baseDefinition = method.GetBaseDefinition();
            return method.DeclaringType != baseDefinition.DeclaringType;
        }

        void OnInternalUnityUpdate()
        {
            if(!Enabled)
                return;
            try
            {
                OnUnityUpdate();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, e.ToString()).UnityObj(this).Write();
            }
        }

        void OnInternalFixedUpdate()
        {
            if (!enabled)
                return;
            try
            {
                OnUnityFixedUpdate();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, e.ToString()).UnityObj(this).Write();
            }
        }
        protected virtual void OnUnityUpdate()
        {

        }
        protected virtual void OnUnityFixedUpdate()
        {

        }
    }
}
