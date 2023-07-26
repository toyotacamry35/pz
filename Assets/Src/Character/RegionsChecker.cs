using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using GeneratedCode.DeltaObjects;
using SharedCode.Aspects.Regions;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System;
using System.Collections;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Assets.Src.Character
{
    public class RegionsChecker : EntityGameObjectComponent, IDeathResurrectListenerComponent
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private GameObject _akGameObject;
        private Coroutine _updateCoroutine;

        protected override void GotClientAuthority()
        {
            _activatedSwitches = new List<RegionSoundStateDef>();
            _activatedStates = new List<RegionSoundStateDef>();
            _updateCoroutine = this.StartInstrumentedCoroutine(UpdateRegions());
        }

        private IEnumerator UpdateRegions()
        {
            IRegion rootRegion;
            while (true)
            {
                rootRegion = RegionsHolder.GetRegionByGuid(ClientRepo.GetSceneForEntity(GetOuterRef<IEntity>()));
                if (rootRegion != null)
                    break;

                yield return CoroutineAwaiters.GetTick(1);
            }

            var currentRegions = new List<IRegion>();
            while (true)
            {
                yield return CoroutineAwaiters.GetTick(1);
                try
                {
                    currentRegions.Clear();
                    rootRegion.GetAllContainingRegionsNonAlloc(currentRegions, (SharedCode.Utils.Vector3)transform.position);
                    ProcessRegions(currentRegions);
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }

        private readonly List<RegionWithLevel> _regionsWithData = new List<RegionWithLevel>();
        private void ProcessRegions(ICollection<IRegion> regions)
        {
            _regionsWithData.Clear();
            foreach (var region in regions)
            {
                DebugDrawCurrentRegion(region);

                if (region.RegionDef != default(ARegionDef))
                    _regionsWithData.Add(new RegionWithLevel(region, region.Level));
            }
            ProcessSoundRegions(_regionsWithData);
        }

        public void DebugDrawCurrentRegion(IRegion region)
        {
            var boxReg = region as GeoBox;
            if (boxReg != default(GeoBox))
            {
                Matrix4x4 matrix4X4 = Matrix4x4.TRS((Vector3)boxReg.Center, Quaternion.Inverse((Quaternion)boxReg.InverseRotation), Vector3.one);
                DebugExtension.DebugLocalCube(matrix4X4, ((Vector3)boxReg.Extents) * 2, default(Vector3), 5f);
            }
            var sphereReg = region as GeoSphere;
            if (sphereReg != default(GeoSphere))
                DebugExtension.DebugWireSphere((Vector3)sphereReg.Center, sphereReg.Radius, 5f);
        }

        public void OnResurrect(UnityEnvironmentMark.ServerOrClient ctx, PositionRotation at) => ResetSoundRegionsCaches();

        public void OnDeath(UnityEnvironmentMark.ServerOrClient ctx) { }

        protected override void LostClientAuthority()
        {
            if(_updateCoroutine != null)
                this.StopCoroutine(_updateCoroutine);
        }

        #region SoundRegionProcessing

        private List<RegionSoundStateDef> _activatedSwitches;
        private List<RegionSoundStateDef> _activatedStates;

        private struct SoundStateWithLevel
        {
            public RegionSoundStateDef _soundState;
            public int _level;
        }

        private List<SoundStateWithLevel> _switchesWithLevels = new List<SoundStateWithLevel>();
        private List<SoundStateWithLevel> _statesWithLevels = new List<SoundStateWithLevel>();
        private List<RegionSoundStateDef> _switchesToActivate = new List<RegionSoundStateDef>();
        private List<RegionSoundStateDef> _statesToActivate = new List<RegionSoundStateDef>();
        private void ProcessSoundRegions(List<RegionWithLevel> regionsWithLevels)
        {
            foreach (var regionWithLevel in regionsWithLevels)
            {
                var regionData = regionWithLevel._region.RegionDef.Data;
                var regionLevel = regionWithLevel._regionLevel;
                foreach (var d in regionData)
                {
                    var regionSoundData = d.Target as SoundRegionDataDef;
                    if (regionSoundData != null)
                    {
                        var switches = regionSoundData.Switches;
                        if (switches != null)
                            foreach (var @switch in switches)
                                _switchesWithLevels.Add(new SoundStateWithLevel { _soundState = @switch, _level = regionLevel });

                        var states = regionSoundData.States;
                        if (states != null)
                            foreach (var state in states)
                            {
                                _statesWithLevels.Add(new SoundStateWithLevel { _soundState = state, _level = regionLevel });
                            }
                                
                    }
                }
            }

            SelectStatesToActivateAndRemoveCurrent(_switchesWithLevels, _activatedSwitches, _switchesToActivate);
            SelectStatesToActivateAndRemoveCurrent(_statesWithLevels, _activatedStates, _statesToActivate);

            foreach (var switchToActivate in _switchesToActivate)
            {
                if (_akGameObject == null)
                    _akGameObject = GetComponentInChildren<AkGameObj>().gameObject;
                if (_akGameObject != null)
                {
                    _activatedSwitches.RemoveAll(x => x._groupID == switchToActivate._groupID);
                    _activatedSwitches.Add(switchToActivate);
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Set Sound Switch | {@}", new { GroupId = switchToActivate._groupID, ValueID = switchToActivate._valueID, Object = _akGameObject.name }).Write();
                    AkSoundEngine.SetSwitch((uint)switchToActivate._groupID, (uint)switchToActivate._valueID, _akGameObject);
                }
            }

            foreach (var stateToActivate in _statesToActivate)
            {
                _activatedStates.RemoveAll(x => x._groupID == stateToActivate._groupID);
                _activatedStates.Add(stateToActivate);
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Set Sound State | {@}", new { GroupId = stateToActivate._groupID, ValueID = stateToActivate._valueID }).Write();
                AkSoundEngine.SetState((uint)stateToActivate._groupID, (uint)stateToActivate._valueID);
            }
        }

        private void SelectStatesToActivateAndRemoveCurrent(List<SoundStateWithLevel> currentSoundStateWithLevels, List<RegionSoundStateDef> activeStates, List<RegionSoundStateDef> regionStatesToActivate)
        {
            foreach (var activeState in activeStates)
                for (var i = 0; i < currentSoundStateWithLevels.Count; ++i)
                {
                    var previousSoundState = currentSoundStateWithLevels[i];
                    if (previousSoundState._soundState.Equals(activeState))
                    {
                        for (var j = currentSoundStateWithLevels.Count - 1; j >= 0; --j)
                            if (currentSoundStateWithLevels[j]._soundState._groupID == activeState._groupID &&
                                currentSoundStateWithLevels[j]._level <= previousSoundState._level)
                                currentSoundStateWithLevels.RemoveAt(j);
                        break;
                    }
                }

            regionStatesToActivate.Clear();
            while (currentSoundStateWithLevels.Count > 0)
            {
                var saved = currentSoundStateWithLevels[0];

                foreach (var currentSoundStateWithLevel in currentSoundStateWithLevels)
                    if (currentSoundStateWithLevel._level > saved._level && currentSoundStateWithLevel._soundState._groupID == saved._soundState._groupID)
                        saved = currentSoundStateWithLevel;

                for (var i = currentSoundStateWithLevels.Count - 1; i >= 0; --i)
                    if (currentSoundStateWithLevels[i]._soundState._groupID == saved._soundState._groupID)
                        currentSoundStateWithLevels.RemoveAt(i);

                regionStatesToActivate.Add(saved._soundState);
            }
        }

        public void ResetSoundRegionsCaches()
        {
            _activatedSwitches = new List<RegionSoundStateDef>();
            _activatedStates = new List<RegionSoundStateDef>();
        }

        #endregion SoundRegionProcessing

        internal readonly struct RegionWithLevel
        {
            public readonly IRegion _region;
            public readonly int _regionLevel;

            public RegionWithLevel(IRegion region, int regionLevel)
            {
                _region = region;
                _regionLevel = regionLevel;
            }
        }
    }
}
