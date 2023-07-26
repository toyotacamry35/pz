using System;
using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;

namespace ColonyShared.SharedCode.InputActions
{
    public interface IInputActionLayersStack
    {
        /// <summary>
        /// Note: initializer не должен захатывать entity или deltaobj'ы!
        /// </summary>
        void PushLayer(object layerOwner, InputActionHandlersLayerDef def, Action<IInputActionLayer> initializer = null);
        
        /// <summary>
        /// Note: modifier не должен захатывать entity или deltaobj'ы!
        /// </summary>
        void ModifyLayer(object layerOwner, InputActionHandlersLayerDef def, Action<IInputActionLayer> modifier);
        
        void DeleteLayer(object layerOwner, InputActionHandlersLayerDef def);

    }

    public static class InputActionHandlersStackMethods
    {
        public static void PushLayer(this IInputActionLayersStack self, object layerOwner, InputActionHandlersLayerListDef handlersList)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            self.PushLayer(layerOwner, handlersList.Layer, handlersList.Handlers);
        }

        public static void PushLayer(this IInputActionLayersStack self, object layerOwner, InputActionHandlersLayerDef layerDef, 
            InputActionHandlersListDef handlersList)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            self.PushLayer(layerOwner, layerDef, handlersList.Handlers);
        }

        public static void PushLayer(this IInputActionLayersStack self, object layerOwner, InputActionHandlersLayerDef layerDef, 
            IEnumerable<KeyValuePair<InputActionDef, IInputActionHandlerDescriptor>> handlers)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            self.PushLayer(layerOwner, layerDef, layer =>
            {
                if (handlers != null)
                    foreach (var kv in handlers)
                        layer.AddBinding(layerOwner, kv.Key, kv.Value, default(InputActionHandlerContext));
            });
        }

        public static void PushLayer(this IInputActionLayersStack self, object layerOwner, InputActionHandlersLayerDef layerDef, 
            IEnumerable<KeyValuePair<ResourceRef<InputActionDef>, ResourceRef<InputActionHandlerDef>>> handlers)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            self.PushLayer(layerOwner, layerDef, layer =>
            {
                if (handlers != null)
                    foreach (var kv in handlers)
                        layer.AddBinding(layerOwner, kv.Key, kv.Value.Target, default(InputActionHandlerContext));
            });
        }
        
        public static void AddHandlers(this IInputActionLayer self, object handlerOwner, 
            IEnumerable<KeyValuePair<InputActionDef, IInputActionHandlerDescriptor>> handlers)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (handlers != null)
                foreach (var kv in handlers)
                    self.AddBinding(handlerOwner, kv.Key, kv.Value, default(InputActionHandlerContext));
        }
        
        public static void AddHandlers(this IInputActionLayer self, object handlerOwner, 
            IEnumerable<KeyValuePair<ResourceRef<InputActionDef>, ResourceRef<InputActionHandlerDef>>> handlers)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (handlers != null)
                foreach (var kv in handlers)
                    self.AddBinding(handlerOwner, kv.Key, kv.Value.Target, default(InputActionHandlerContext));
        }       
    }
}