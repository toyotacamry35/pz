using SharedCode.Aspects.Regions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.Regions
{
    public class RegionSound : MonoBehaviour
    {
        public AK.Wwise.Switch[] _wwiseSwitches;
        public AK.Wwise.State[] _wwiseStates;

        public RegionSoundStateDef[] GetSwitchIDs()
        {
            List<RegionSoundStateDef> wS = new List<RegionSoundStateDef>();
            if (_wwiseSwitches == null)
                return wS.ToArray();

            foreach (var @switch in _wwiseSwitches)
                wS.Add(new RegionSoundStateDef { _groupID = @switch.GroupId, _valueID = @switch.Id });
            return wS.ToArray();
        }

        public RegionSoundStateDef[] GetStateIDs()
        {
            List<RegionSoundStateDef> wS = new List<RegionSoundStateDef>();
            if (_wwiseStates == null)
                return wS.ToArray();

            foreach (var state in _wwiseStates)
                wS.Add(new RegionSoundStateDef { _groupID = state.GroupId, _valueID = state.Id});
            return wS.ToArray();
        }
    }
}
