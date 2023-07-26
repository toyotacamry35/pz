
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
 using Core.Environment.Logging.Extension;
 using NLog;

namespace ResourcesSystem.Loader
{
    public class LoadingContext
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public LoadingContext()
        {
            Trace = Logger.IsTraceEnabled;
        }

        public class LoadingFrame
        {
            public readonly bool FromProto = false;
            public int ProtoIndex = 1;
            public readonly ProtoFrame RootTemplateFrame = new ProtoFrame(false, default(ResourceIDFull), -1);
            public readonly string Adress;
            public Dictionary<string, IResource> Resources { get; } = new Dictionary<string, IResource>();
            public int PushedSubObjects = 0;
            public ResourceIDFull? ProtoStart;
            public Dictionary<string, object> CustomFields { get; } = new Dictionary<string, object>();

            public LoadingFrame(string adress, bool fromProto)
            {
                Adress = adress;
                FromProto = fromProto;
            }
            public override string ToString()
            {
                return $"{FromProto} {ProtoIndex} {Adress} {PushedSubObjects}";
            }
        }
        public class ProtoFrame
        {
            public object ProtoObject;
            public bool FinishedLoadingDirectProtos = false;
            public bool IsCurrentlyParsingVars;
            public bool Embedded = false;
            public readonly Dictionary<string, TemplateVariable> Variables = new Dictionary<string, TemplateVariable>();
            public readonly bool IsProtoLoading;
            public ResourceIDFull Id;
            //public bool CanRegisterDefInstance = true;
            public readonly int StackIdx;

            public ProtoFrame(bool isProtoLoading, ResourceIDFull id, int stackIdx)
            {
                Id = id;
                IsProtoLoading = isProtoLoading;
                StackIdx = stackIdx;
            }
            public override string ToString()
            {
                return $"{Embedded} {IsProtoLoading} {IsCurrentlyParsingVars} {ProtoObject?.GetType().Name} {((IResource)ProtoObject)?.Address} {Id} {StackIdx}";
            }
        }
        public void PushSubObject()
        {
            LoadingFrames.Peek().PushedSubObjects++;
        }

        public void PopSubObject()
        {
            LoadingFrames.Peek().PushedSubObjects--;
        }
        public int ProtoIndex { get { return LoadingFrames.First(x => !x.FromProto).ProtoIndex; } set { LoadingFrames.First(x => !x.FromProto).ProtoIndex = value; } }
        public bool IsRootObject => !IsProto && LoadingFrames.Peek().PushedSubObjects == 0;
        public bool IsProto => ProtoStack.Count > 0 && ProtoStack.Peek().IsProtoLoading;
        public string StartRootAddress => LoadingFrames.First().Adress;
        public string RootAddress => LoadingFrames.Peek().Adress;
        public string ProtoRootAdress => LoadingFrames.First(x => !x.FromProto).Adress;
        public bool IsProtoChildFile => IsProto && RootAddress == ProtoStack.Peek().Id.Root;
        public Stack<LoadingFrame> LoadingFrames { get; } = new Stack<LoadingFrame>();
        public readonly Stack<ProtoFrame> ProtoStack = new Stack<ProtoFrame>();

        public void DumpToLogger(BaseResource resource, ResourceIDFull id)
        {
            Logger.IfError()?.Message($"REGISTER OBJECT {resource.GetType().Name} {id} {resource.____GetDebugAddress()} \n PROTO STACK \n {string.Join("\n", ProtoStack.Select(x => x.ToString()))} \n FRAMES \n {string.Join("\n", LoadingFrames.Select(x => x.ToString()))}").Write();
        }


        #region Debug Trace
        public bool Trace;
        public int Depth;
        public string Indent => new string(' ', Depth * 2);
        #endregion

        public void PushLoading(string address)
        {
            if (Trace)
                 Logger.IfTrace()?.Message("#{0}{1}*> PushLoading [{4}] {2} {3}",  Depth, Indent, address, IsProto, LoadingFrames.Count).Write();
            LoadingFrames.Push(new LoadingFrame(address, IsProto));
        }

        public void PopLoading()
        {
            var frame = LoadingFrames.Pop();
            if (Trace)
                 Logger.IfTrace()?.Message("#{0}{1}<* PopLoading [{3}] {2}",  Depth, Indent, frame.Adress, LoadingFrames.Count).Write();
        }

        public void SetInternalRes(string id, IResource value)
        {
            LoadingFrames.Peek().Resources.Add(id, value);
        }

        public IResource GetInternalRes(string id)
        {
            if (!LoadingFrames.Peek().Resources.ContainsKey(id))
                return null;
            return LoadingFrames.Peek().Resources[id];
        }
        public void PushProto(bool isProtoLoading, ResourceIDFull id = default(ResourceIDFull))
        {
            if (Trace)
                 Logger.IfTrace()?.Message("#{0}{1}$> PushProto [{4}] {2} {3}",  Depth, Indent, id, isProtoLoading, ProtoStack.Count).Write();
            ProtoStack.Push(new ProtoFrame(isProtoLoading, id, ProtoStack.Count));
        }

        public void PopProto()
        {
            var frame = ProtoStack.Pop();
            if (Trace)
                 Logger.IfTrace()?.Message("#{0}{1}<$ PopProto [{3}] {2}",  Depth, Indent, frame.Id, ProtoStack.Count).Write();
        }
        public void SetVar(string var, TemplateVariable obj)
        {
            if (!IsProto)
            {
                LoadingFrames.Peek().RootTemplateFrame.Variables.Add(var, obj);
                return;
            }
            var lastProto = ProtoStack.First(x => x.IsProtoLoading);
            if (lastProto.Variables.ContainsKey(var))
            {
                var temVar = lastProto.Variables[var];
                if (Trace)
                    Logger.Warn("#{5}{6} Variable {0} already defined. Existing value: '{1}' New value: '{2}' ProtoFrameIdx: {8} ProtoFrame: {7} Root: {3} ProtoRoot: {4}",
                                var, temVar.Value, obj.Value, RootAddress, ProtoRootAdress, Depth, Indent, lastProto.Id, lastProto.StackIdx);
                //do nothing, except check the type
                var checkedAgainstType = temVar.Type;
                if (checkedAgainstType == null)
                    return;
                if (checkedAgainstType.IsGenericType && checkedAgainstType.GetGenericTypeDefinition() == typeof(ResourceRef<>))
                    return;//do nothing, we can't yet check this stuff in a meaningfull manner
                if (checkedAgainstType.IsGenericType && checkedAgainstType.GetGenericTypeDefinition() == typeof(UnityRef<>))
                    return;
                if (!obj.Type.IsAssignableFrom(checkedAgainstType) && !PrimitiveTypesConverter.CanConvert(checkedAgainstType, obj.Type))
                    GameResources.ThrowError($"Type mismatch in template variables {ProtoStack.Peek().Variables[var].VariableId} {checkedAgainstType.Name} {obj.Type.Name} {obj.VariableId} {obj.Type?.Name}");
            }
            else
            {
                if (Trace)
                     Logger.IfTrace()?.Message("#{4}{5}Set {0} = '{1}' ProtoFrameIdx: {7} ProtoFrame: {6} Root: {2} ProtoRoot: {3}",  var, obj.Value, RootAddress, ProtoRootAdress, Depth, Indent, lastProto.Id, lastProto.StackIdx).Write();
                lastProto.Variables.Add(var, obj);
            }
        }

        public object GetVar(string var, out Type type)
        {
            TemplateVariable v;
            if (!IsProto)
            {
                if (!LoadingFrames.Peek().RootTemplateFrame.Variables.TryGetValue(var, out v))
                {
                    GameResources.ThrowError($"Has no variable {var} in root {RootAddress}");
                }
            }
            else
            {
                if (!ProtoStack.First(x => x.IsProtoLoading).Variables.TryGetValue(var, out v))
                {
                    GameResources.ThrowError($"Has no variable {var} in proto stack last {ProtoStack.First(x => x.IsProtoLoading).Id}");
                }
            }
            type = v.Type;
            return v.Value;
        }
    }

}