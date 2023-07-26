using System;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using ColonyShared.SharedCode;
using UnityEngine;

namespace  Assets.Src.Audio
{
    public static class SoundHelper
    {
        public static void SetParameter(string name, Value value, GameObject obj)
        {
            switch (value.ValueType)
            {
                case Value.Type.Bool:
                    AkSoundEngine.SetSwitch(name, value.Bool ? "true" : "false", obj);
                    break;
                case Value.Type.String:
                    if (value.String != null)
                        AkSoundEngine.SetSwitch(name, value.String, obj);
                    break;
                case Value.Type.Resource:
                    if (value.Resource != null)
                    {
                        if (value.Resource is IResourceWithSoundSwitch ss)
                        {
                            if (ss.SoundSwitch != null)
                                AkSoundEngine.SetSwitch(name, ss.SoundSwitch, obj);
                        }
                        else
                            throw new Exception($"{value.Resource.GetType()} is not a {nameof(IResourceWithSoundSwitch)}");
                    }
                    break;
                default:
                    AkSoundEngine.SetRTPCValue(name, value.AsFloat(), obj);
                    break;
            }
        }
    }
}