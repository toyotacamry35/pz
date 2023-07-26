using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.Aspects.Impl.Stats;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using SharedCode.Serializers;
using SharedCode.Wizardry;
using System.Collections.Generic;
using ColonyShared.SharedCode.Utils;
using UnityEditor;
using UnityEngine;
using Assets.Src.ResourceSystem;
using SharedCode.Repositories;
using SharedCode.EntitySystem;

namespace Assets.Src.DebugInterfaces.Editor
{
    class WizardDebugInterface : EditorWindow
    {
        [MenuItem("Debug/WizardDebug")]
        static void WizardDebug()
        {
            GetWindow<WizardDebugInterface>();
        }

        private void OnEnable()
        {
        }

        class SpellDebugDescription
        {
            public string DefName;
            public long StopAt;
            public long StartAt;
            public bool IsActive;
        }
        Dictionary<StatResource, float> _stats = new Dictionary<StatResource, float>();
        List<SpellTimeLineData> _spellStatuses = new List<SpellTimeLineData>();
        List<string> _spellStatusesServer = new List<string>();
        private void InternalUpdate()
        {
            var sGo = Selection.activeGameObject;
            if (sGo != null)
            {
                var @ref = ColonyHelpers.Helpers.GetObjectEntityRef(sGo);

                var type = ReplicaTypeRegistry.GetTypeById(@ref.TypeId);
                if (!typeof(IHasWizardEntity).IsAssignableFrom(type))
                    return;
                var rid = EntitiesRepository.GetReplicationTypeId(@ref.TypeId, SharedCode.EntitySystem.ReplicationLevel.ClientBroadcast);
                /*AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var ent = await NodeAccessor.Repository.Get(rid, guid))
                    {
                        var e = ent.Get<IHasWizardEntityClientBroadcast>(rid, guid);
                        using (var wEnt = await NodeAccessor.Repository.Get<IWizardEntityClientBroadcast>(e.Wizard.Id))
                        {
                            _stats.Clear();
                            _spellStatuses.Clear();
                            var wiz = wEnt.Get<IWizardEntityClientBroadcast>(e.Wizard.Id);
                            foreach (var spell in wiz.Spells)
                            {
                                _spellStatuses.Add(spell.TimeLineData);
                            }
                            if (e is IStatEntity)
                                foreach (var stat in (e as IStatEntityClientBroadcast).SimpleStupidStats)
                                {
                                    _stats.Add(stat.Key, stat.Value);
                                }
                        }
                    }
                }, NodeAccessor.Repository).Wait(); 
                AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var ent = await ServerProvider.Server.UnityEntitiesRepository.Get(rid, guid))
                    {
                        var e = ent.Get<IHasWizardEntityClientBroadcast>(rid, guid);
                        using (var wEnt = await ServerProvider.Server.UnityEntitiesRepository.Get<IWizardEntityClientBroadcast>(e.Wizard.Id))
                        {
                            _spellStatusesServer.Clear();
                            var wiz = wEnt.Get<IWizardEntityClientBroadcast>(e.Wizard.Id);
                            foreach (var spell in wiz.Spells)
                            {
                                _spellStatusesServer.Add($"{ spell.TimeLineData.SpellDef.____GetDebugShortName()} IsActive { spell.TimeLineData.IsActive} " +
                        $"{string.Join("\n", spell.TimeLineData.Updates.SelectMany(x => x.Actions.Select(a => a.ToString())))}");
                            }
                        }
                    }
                }, ServerProvider.Server.UnityEntitiesRepository).Wait();*/
                if (_castSpell)
                {

                    _castSpell = false;
                    var spellDef = spell.Get<SpellDef>();
                    AsyncUtils.RunAsyncTask(async () =>
                                        {
                                            using (var ent = await NodeAccessor.Repository.Get(rid, @ref.Guid))
                                            {
                                                var e = ent.Get<IHasWizardEntityClientBroadcast>(rid, @ref.Guid);
                                                using (var wEnt = await NodeAccessor.Repository.Get<IWizardEntityClientBroadcast>(e.Wizard.Id))
                                                {
                                                    var wiz = wEnt.Get<IWizardEntityClientBroadcast>(e.Wizard.Id);
                                                    await wiz.CastSpell(new SpellCast() { StartAt = SyncTime.Now, Def = spellDef });
                                                }
                                            }
                                        }, NodeAccessor.Repository);
                }
            }
        }
        private void OnDisable()
        {
        }
        SpellId _castedSpellId = default(SpellId);
        bool _castSpell = false;
        JdbMetadata spell;
        private void OnGUI()
        {
            GUILayout.BeginVertical();
            spell = (JdbMetadata)EditorGUILayout.ObjectField("Spell: ", spell, typeof(JdbMetadata), allowSceneObjects: false);
            GUILayout.Label("Stats:");
            foreach (var stat in _stats)
            {
                GUILayout.Label($"{stat.Key.____GetDebugShortName()} {stat.Value}");
            }
            GUILayout.Label("Spells:");
            foreach (var spell in _spellStatusesServer)
            {
                GUILayout.TextArea(spell);
            }
            if (GUILayout.Button("Cast test spell"))
            {
                _castSpell = true;
            }
            GUILayout.EndVertical();
            if (Application.isPlaying && UnityEngine.Event.current.type == EventType.Repaint)
                InternalUpdate();
            Repaint();
        }
    }
}
