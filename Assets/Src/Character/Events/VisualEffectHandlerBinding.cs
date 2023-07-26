using System;
using ColonyShared.GeneratedCode;
using JetBrains.Annotations;
using SharedCode.Utils;

namespace Assets.Src.Character.Events
{
    public interface IVisualEffectHandlerBinding {}
    
    public interface IVisualEffectHandlerBinding<TDef> : IVisualEffectHandlerBinding where TDef : VisualEffectHandlerCasterTargetDef
    {
        void OnEventUpdate(TDef def, IVisualEvent evt);
    }
    
    public abstract class VisualEffectHandlerBindingHolder : BindingHolder<IVisualEffectHandlerBinding, VisualEffectHandlerCasterTargetDef>
    {
        public abstract void OnEventUpdate(VisualEffectHandlerCasterTargetDef def, IVisualEvent evt);
    }
    
    public class VisualEffectHandlerBindingHolder<TDef> : VisualEffectHandlerBindingHolder where TDef: VisualEffectHandlerCasterTargetDef
    {
        public override void OnEventUpdate(VisualEffectHandlerCasterTargetDef def, IVisualEvent evt)
        {
            var indef = (TDef)def;
            ((IVisualEffectHandlerBinding<TDef>)Instance).OnEventUpdate(indef, evt);
        }
    }

    public class VisualEffectHandlerCollection
    {
        [CollectTypes, UsedImplicitly] private static Collector _collector;

        public static VisualEffectHandlerBindingHolder Get(VisualEffectHandlerCasterTargetDef def)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));
            var defType = def.GetType();
            if (_collector.Collection.TryGetValue(defType, out var holder))
                return holder;
            throw new NotImplementedException($"{nameof(IVisualEffectHandlerBinding)} is not implemented for {defType}");
        }
        
        public static VisualEffectHandlerBindingHolder TryGet(VisualEffectHandlerCasterTargetDef def)
        {
            if (def != null && _collector.Collection.TryGetValue(def.GetType(), out var holder))
                return holder;
            return default;
        }
        
        public class Collector : BindingCollector<IVisualEffectHandlerBinding, VisualEffectHandlerBindingHolder, VisualEffectHandlerCasterTargetDef>
        {
            public Collector() : base(typeof(IVisualEffectHandlerBinding<>), typeof(VisualEffectHandlerBindingHolder<>)) {}
        }
    }
}
