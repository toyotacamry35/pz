using System;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.Lib.Cheats;
using Assets.Src.Aspects.Impl.Traumas.Template;
using Assets.Src.Camera;
using Assets.Src.ContainerApis;
using Assets.Src.NetworkedMovement;
using ColonyDI;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using UnityEngine;
using ColonyShared.SharedCode.Aspects.Locomotion;
using EnumerableExtensions;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using OutlineEffect;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Src.Locomotion;
using Src.Locomotion.Unity;
using Utilities;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.Src.RubiconAI;
using Assets.Tools;
using L10n;
using NLog;
using SharedHelpers = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;
using Assets.Src.DebugUI;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.SpawnSystem;
using ColonyShared.GeneratedCode.Shared.Aspects;
using SharedCode.Utils.Extensions;
using Src.Aspects.Impl;
using ColonyShared.SharedCode;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using ReactivePropsNs;
using ResourceSystem.Utils;
using SharedCode.EntitySystem.Delta;
using SharedCode.Utils;
using static Src.Locomotion.DebugTag;
using Src.Debugging;
using Vector3 = UnityEngine.Vector3;
using SharedCode.Serializers;

namespace Uins
{
    public class DebugGui : DependencyEndNode
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [SerializeField, UsedImplicitly]
        private DebugView _debugView;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _statsUpdateInterval;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _movesUpdateInterval;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _timesUpdateInterval;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _interactiveObjectUpdateInterval;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _mutationUpdateInterval;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _moveActionsUpdateInterval;

        [SerializeField, UsedImplicitly]
        private UpdateInterval _pathfindingOwnershipUpdateInterval;

        [UsedImplicitly, SerializeField]
        private DebugTimelineChart _characterPositionChart;

        [UsedImplicitly, SerializeField]
        private DebugTimelineChart _characterRotationChart;

        [UsedImplicitly, SerializeField]
        private DebugTimelineChart _characterCustomValueChart;

        [UsedImplicitly, SerializeField]
        private GameObject _markerPrefab;

        [UsedImplicitly, SerializeField]
        private GameObject _pathfindingOwnershipMarkerPrefab;

        [UsedImplicitly, SerializeField]
        private StatDebugViewModel _statDebugViewModelPrefab;

        [UsedImplicitly, SerializeField]
        private Transform _timeStatsRoot;

        [UsedImplicitly, SerializeField]
        private Transform _accumStatsRoot;

        [UsedImplicitly, SerializeField]
        private Transform _procStatsRoot;

        [UsedImplicitly, SerializeField]
        private List<DebugTag> _showLocomotionKeys;

        private readonly DebugTag[] _defaultShowLocomotionKeys =
        {
            RelevanceLevel,
            NavMeshPosition,
            //"NavMeshOffLink",
            VarsVelocity,
            //"Shift",
            //"FallHeight",
            NetworkSentPosition,
            //"NetworkSentReason",
            NetworkSentFrameId,
            //"Trail"
        };

        [SerializeField, UsedImplicitly]
        private WindowId _inputTimeWindowId;
        [SerializeField, UsedImplicitly] 
        private float SpellObsolescenceTime = 15;

        private bool _isInited;
        private ICharacterPawn _playerPawn;
        private PlayerInteractionViewModel _playerInteractionVm;
        private List<ICharacterPawn> _characters = new List<ICharacterPawn>();
        private DebugTag _currentLocomotionValueId;
        private LocomotionDebugTrail.TrailModes _currentLocomotionTrailMode = LocomotionDebugTrail.TrailModes.BodySpeed;
        private LocomotionDebugProviders _locomotionDebug;
        private List<Outline> _outlines = new List<Outline>();

        private Dictionary<StatKind, StatKindSubscribers> _statKindSubscribers;

        private string _asyncMutationText;
        private IGuiWindow _inputTimeWindow;

        private EntityApiWrapper<HasTraumasFullApi> _hasTraumasFullApiWrapper;
        private EntityApiWrapper<HasStatsBroadcastApi> _hasStatsBroadcastApiWrapper;
        private EntityApiWrapper<HasStatsFullApi> _hasStatsFullApiWrapper;
        private List<SpellInfo> _spells = new List<SpellInfo>();
        private readonly object _spellsLock = new object();
        private OuterRef<IHasWizardEntityClientFull> _spellsEntityRef;

        private Dictionary<GameObject, GameObject> _pathfindingOwnershipVisualizedPaws = new Dictionary<GameObject, GameObject>();


        //=== Props ===============================================================

        [Dependency]
        private HudGuiNode HudGui { get; set; }

        private void OnChangeEntityInDebugFocus((IEntityPawn oldPawn, IEntityPawn newPawn) pawns)
        {
            if (_locomotionDebug.Trail)
            {
                _locomotionDebug.Trail.Drawing = false;
                _locomotionDebug.Trail.Recording = false;
                _locomotionDebug.Trail.Clear();
            }
            
            switch (pawns.oldPawn)
            {
                case CharacterPawn characterPawn:
                    break;
                case IMobPawn mobPawn:
                    FinishObservationMob(mobPawn);
                    break;
            }

            switch (pawns.newPawn)
            {
                case ICharacterPawn pawn:
                {
                    _locomotionDebug = new LocomotionDebugProviders
                    {
                        InfoProvider = () => pawn.LocomotionDebugInfo,
                        TrailProvider = () => pawn.LocomotionDebugTrail,
                        EntityId = pawn.EntityRef.Guid,
                        EntityName = pawn == _playerPawn ? "local player" : "remote character",
                        Transform = pawn.Ego.transform
                    };
                    SetSelectionMark(pawn);
                    SetSelectedEntityId(pawn.EntityRef.Guid);
                }
                    break;

                case IMobPawn pawn:
                {
                    StartObservationMob(pawn);
                    _locomotionDebug = new LocomotionDebugProviders
                    {
                        InfoProvider = () => pawn.LocomotionDebugInfo,
                        TrailProvider = () => pawn.LocomotionDebugTrail,
                        EntityId = pawn.EntityRef.Guid,
                        EntityName = pawn.Ego.gameObject.name.Replace(pawn.EntityRef.Guid.ToString(), ""),
                        Transform = pawn.Ego.transform
                    };
                    SetSelectionMark(pawn);
                    SetSelectedEntityId(pawn.EntityRef.Guid);
                }
                    break;

                case null:
                    _locomotionDebug = new LocomotionDebugProviders();
                    SetSelectionMark(default);
                    SetSelectedEntityId(default);
                    break;
            }
        }

        //=== Unity ===========================================================

        private void Awake()
        {
            _debugView.AssertIfNull(nameof(_debugView));
            _timeStatsRoot.AssertIfNull(nameof(_timeStatsRoot));
            _procStatsRoot.AssertIfNull(nameof(_procStatsRoot));
            _accumStatsRoot.AssertIfNull(nameof(_accumStatsRoot));
            _inputTimeWindowId.AssertIfNull(nameof(_inputTimeWindowId));
            _statDebugViewModelPrefab.AssertIfNull(nameof(_statDebugViewModelPrefab));

            foreach (var entry in _defaultShowLocomotionKeys)
            {
                if (!_showLocomotionKeys.Contains(entry))
                    _showLocomotionKeys.Add(entry);
            }

            _debugView.PropertyChanged += OnDebugViewPropertyChanged;
            CreateStatKindSubscribers();
            EntityInDebugFocus.Changed += OnChangeEntityInDebugFocus;
        }


        //=== Private =========================================================

        private void OnMarkerActivityChanged(bool newVal)
        {
            // Manage marker:
            if (_markerGo != null)
            {
                if (newVal == true)
                {
                    if (_markerGo.transform.parent != null)
                        _markerGo.SetActive(true);
                }
                else
                    _markerGo.SetActive(false);
            }

            // Manage AiLegionaryHost gather debug data:
            if (EntityInDebugFocus.Mob != null)
            {
                if (newVal)
                    StartObservationMob(EntityInDebugFocus.Mob);
                else
                    FinishObservationMob(EntityInDebugFocus.Mob);
            }
        }

        private int IsGatherDebugDataEnabledCounter_Dbg;

        // cached, 'cos expensive to take:
        private AIWorldLegionaryHost _cashedLegHostOfMobInFocus;

        private void StartObservationMob(ISubjectPawn pawn)
        {
            if (_debugView.IsMoveActionsVisible)
            {
                _cashedLegHostOfMobInFocus = AIWorld.GetWorld(pawn.Ego.WorldSpaceId)?.GetHost(pawn.EntityRef.To<IEntity>());
                if (_cashedLegHostOfMobInFocus != null)
                {
                    _cashedLegHostOfMobInFocus.IsGatherDebugDataEnabled = true;

                    //#Dbg-of-Dbg:
                    {
                        ++IsGatherDebugDataEnabledCounter_Dbg;
                        Logger.IfDebug()?.Message /*Warn*/($"#Dbg (Not Warn): ++ IsGatherDebugDataEnabledCounter_Dbg (=={IsGatherDebugDataEnabledCounter_Dbg})")
                            .Write();

                        if (IsGatherDebugDataEnabledCounter_Dbg > 1)
                            Logger.IfError()?.Message($"IsGatherDebugDataEnabledCounter_Dbg > 1! (=={IsGatherDebugDataEnabledCounter_Dbg})").Write();
                    }
                }
            }
        }

        private void FinishObservationMob(ISubjectPawn pawn)
        {
            if (_debugView.IsMoveActionsVisible)
            {
                if (_cashedLegHostOfMobInFocus != AIWorld.GetWorld(pawn.Ego.WorldSpaceId)?.GetHost(pawn.EntityRef.To<IEntity>()))
                    Logger.Error($"Unexpected `{nameof(_cashedLegHostOfMobInFocus)}` != re-got one: ({_cashedLegHostOfMobInFocus}, " +
                                 $"{AIWorld.GetWorld(pawn.Ego.WorldSpaceId)?.GetHost(pawn.EntityRef.To<IEntity>())}(pawn:{pawn}).");

                if (_cashedLegHostOfMobInFocus != null && _cashedLegHostOfMobInFocus.IsGatherDebugDataEnabled)
                {
                    _cashedLegHostOfMobInFocus.IsGatherDebugDataEnabled = false;

                    //#Dbg-of-Dbg:
                    {
                        --IsGatherDebugDataEnabledCounter_Dbg;
                        Logger.IfDebug()?.Message /*Warn*/($"#Dbg (Not Warn): --- IsGatherDebugDataEnabledCounter_Dbg (=={IsGatherDebugDataEnabledCounter_Dbg})")
                            .Write();
                    }
                }
            }
        }

        private void OnDebugViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;

            switch (propertyName)
            {
                case nameof(DebugView.IsVisibleLinearStats):
                    if (_playerPawn != null)
                        OnTimeStatsSubscribersActivityChanged();
                    break;

                case nameof(DebugView.IsVisibleAccumStats):
                    if (_playerPawn != null)
                        OnAccumStatsSubscribersActivityChanged();

                    break;

                case nameof(DebugView.IsVisibleProcStats):
                    if (_playerPawn != null)
                        OnProcStatsSubscribersActivityChanged();

                    break;

                case nameof(DebugView.IsVisible):
                    OnMarkerActivityChanged(_debugView.IsVisible);
                    if (_playerPawn != null)
                    {
                        OnTimeStatsSubscribersActivityChanged();
                        OnAccumStatsSubscribersActivityChanged();
                        OnProcStatsSubscribersActivityChanged();
                    }

                    break;

                case nameof(DebugView.TimeStatsFilter):
                    _statKindSubscribers[StatKind.Time].Filter = _debugView.TimeStatsFilter;
                    break;

                case nameof(DebugView.AccumStatsFilter):
                    _statKindSubscribers[StatKind.Accumulated].Filter = _debugView.AccumStatsFilter;
                    break;

                case nameof(DebugView.ProcStatsFilter):
                    _statKindSubscribers[StatKind.Procedural].Filter = _debugView.ProcStatsFilter;
                    break;

                case nameof(DebugView.NeedForInputTimeWindow):
                    if (_inputTimeWindow == null)
                        return;

                    if (_debugView.NeedForInputTimeWindow)
                    {
                        if (_inputTimeWindow.State.Value == GuiWindowState.Closed)
                            WindowsManager.Open(_inputTimeWindow);
                    }
                    else
                    {
                        if (_inputTimeWindow.State.Value != GuiWindowState.Closed)
                            WindowsManager.Close(_inputTimeWindow);
                    }

                    break;
                
                case nameof(DebugView.IsPathfindingOwnershipVisible):
                    UpdatePathfindingOwnershipVisualization();
                        break;
            }
        }

        private void UpdatePathfindingOwnershipVisualization()
        {
            if (_debugView.IsPathfindingOwnershipVisible)
            {
                foreach (var pawn in FindObjectsOfType<Pawn>())
                {
                    if (!_pathfindingOwnershipVisualizedPaws.TryGetValue(pawn.gameObject, out var existedMarker))
                    {
                        if (pawn.Ego.IsServerRepo)
                        {
                            var marker = Instantiate(_pathfindingOwnershipMarkerPrefab, pawn.transform);
                            _pathfindingOwnershipVisualizedPaws.Add(pawn.gameObject, marker);
                        }
                    }
                    else if (!pawn.Ego.IsServerRepo)
                    {
                        Destroy(existedMarker);
                        _pathfindingOwnershipVisualizedPaws.Remove(pawn.gameObject);
                    }
                }
            }
            else
            {
                foreach (var pawn in _pathfindingOwnershipVisualizedPaws)
                {
                    Destroy(pawn.Value);
                }
                
                _pathfindingOwnershipVisualizedPaws.Clear();
            }
        }

        private void OnTimeStatsSubscribersActivityChanged(bool hasPawn = true)
        {
            _statKindSubscribers[StatKind.Time].OnActivityChanged(
                hasPawn && _debugView.IsVisible && _debugView.IsVisibleLinearStats,
                _hasStatsBroadcastApiWrapper.EntityApi,
                _hasStatsFullApiWrapper.EntityApi);
        }

        private void OnAccumStatsSubscribersActivityChanged(bool hasPawn = true)
        {
            _statKindSubscribers[StatKind.Accumulated].OnActivityChanged(
                hasPawn && _debugView.IsVisible && _debugView.IsVisibleAccumStats,
                _hasStatsBroadcastApiWrapper.EntityApi,
                _hasStatsFullApiWrapper.EntityApi);
        }

        private void OnProcStatsSubscribersActivityChanged(bool hasPawn = true)
        {
            _statKindSubscribers[StatKind.Procedural].OnActivityChanged(
                hasPawn && _debugView.IsVisible && _debugView.IsVisibleProcStats,
                _hasStatsBroadcastApiWrapper.EntityApi,
                _hasStatsFullApiWrapper.EntityApi);
        }

        private void SetSelectedEntityId(Guid entityId)
        {
            //     NLog.LogManager.Configuration.Variables["selectedentityid"] = entityId.ToString();
            //   NLog.LogManager.ReconfigExistingLoggers();
        }

        private void LateUpdate()
        {
            if (!ClientCheatsState.DebugInfo)
                return;

            if (!_isInited)
                return;
            VisibilityGridsDebugger.UpdateInputs();
            if (_playerPawn != null && ClientCheatsState.Fly)
                if (Input.GetKeyDown(KeyCode.T) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
                    (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
                {
                    var culture = new CultureInfo("en-US");
                    var pos = _playerPawn.Ego.transform.position;
                    GUIUtility.systemCopyBuffer =
                        $"{GameState.Instance.CurrentMap.____GetDebugRootName()} ({pos.x.ToString("F2", culture)} {pos.y.ToString("F2", culture)} {pos.z.ToString("F2", culture)})";
                    Debug.LogFormat("Coordinates to clipboard: {0}", GUIUtility.systemCopyBuffer);
                }

            var shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftAlt) ||
                       Input.GetKey(KeyCode.RightAlt);

            if (shift && ctrl)
            {
                if (Input.GetKeyDown(KeyCode.Equals))
                    _debugView.IsVisible = !_debugView.IsVisible;
                
                if (Input.GetKeyDown(KeyCode.E))
                    GUIUtility.systemCopyBuffer = EntityInDebugFocus.EntityRef.Guid.ToString();
            }

            if (_debugView.IsVisible)
            {
                _debugView.IsVisibleToggles = GetIsVisibleToggles();

                if (_locomotionDebug.Trail)
                {
                    _locomotionDebug.Trail.Drawing = _debugView.IsVisiblePlayerMove;
                    _locomotionDebug.Trail.TrailMode = _currentLocomotionTrailMode;

                    if (Input.GetKeyDown(KeyCode.PageUp) && ctrl && !shift)
                        _locomotionDebug.Trail.Recording = !_locomotionDebug.Trail.Recording;

                    if (Input.GetKeyDown(KeyCode.PageUp) && ctrl && shift)
                        _locomotionDebug.Trail.Clear();

                    bool tick = (Time.frameCount % 15) == 0;
                    if (Input.GetKey(KeyCode.KeypadPlus) && ctrl && (tick || Input.GetKeyDown(KeyCode.KeypadPlus)))
                        _locomotionDebug.Trail.SkipFrames--;
                    if (Input.GetKey(KeyCode.KeypadMinus) && ctrl && (tick || Input.GetKeyDown(KeyCode.KeypadMinus)))
                        _locomotionDebug.Trail.SkipFrames++;
                }

                if (Input.GetKeyDown(KeyCode.PageDown) && ctrl)
                    _currentLocomotionTrailMode = (LocomotionDebugTrail.TrailModes)
                        ((int) (_currentLocomotionTrailMode + (shift ? -1 : 1) + LocomotionDebugTrail.ModesCount) %
                         LocomotionDebugTrail.ModesCount);

                if (Input.GetKeyDown(KeyCode.Home) && ctrl && _characters.Count > 0)
                {
                    if (_locomotionDebug.Info != null)
                        _locomotionDebug.Info.IsDemanded = false;
                    EntityInDebugFocus.Character = shift
                        ? _characters[(_characters.IndexOf(EntityInDebugFocus.Character) - 1 + _characters.Count) % _characters.Count]
                        : _characters[(_characters.IndexOf(EntityInDebugFocus.Character) + 1) % _characters.Count];
                }

                if (Input.GetKeyDown(KeyCode.Insert) && ctrl && GameCamera.Camera)
                {
                    var mobs = FindObjectsOfType<Pawn>().Where(pawn =>
                    {
                        
                        var vp = GameCamera.Camera.WorldToViewportPoint(pawn.transform.position);
                        return vp.z > 0 && Mathf.Abs(vp.x) <= 1 && Mathf.Abs(vp.y) <= 1;
                    }).Cast<IMobPawn>().ToArray();
                    if (mobs.Length > 0)
                    {
                        EntityInDebugFocus.Mob = shift
                            ? mobs[(mobs.IndexOf(EntityInDebugFocus.Mob) - 1 + mobs.Length) % mobs.Length]
                            : mobs[(mobs.IndexOf(EntityInDebugFocus.Mob) + 1) % mobs.Length];
                    }
                }

                if (Input.GetKeyDown(KeyCode.End) && ctrl)
                {
                    var debugInfo = _locomotionDebug.Info;
                    if (debugInfo != null)
                        _currentLocomotionValueId = shift
                            ? debugInfo.PrevId(_currentLocomotionValueId)
                            : debugInfo.NextId(_currentLocomotionValueId);
                }

                if ((!_debugView.IsVisible || !_debugView.IsVisiblePlayerMove) && _locomotionDebug.Info != null)
                    _locomotionDebug.Info.IsDemanded = false;

                UpdateView();
            }
            else
            {
                if (_locomotionDebug.Trail && _locomotionDebug.Trail.Drawing)
                    _locomotionDebug.Trail.Drawing = false;
            }
        }

        private void OnValidate()
        {
            var xform = _locomotionDebug.Transform;
            _characterPositionChart.SetValueProvider(x => xform ? xform.position : Vector3.zero);
            _characterRotationChart.SetValueProvider(x => xform ? xform.rotation.eulerAngles.y : 0);
            _characterCustomValueChart.SetValueProvider(x => GetChartValue(x));
        }


        //=== Protected ===========================================================

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            if (!SurvivalGuiNode.Instance.AssertIfNull($"{nameof(SurvivalGuiNode)}.{nameof(SurvivalGuiNode.Instance)}"))
            {
                SurvivalGuiNode.Instance.PawnChangesStream.Action(D, OnOurPawnChanged);
                SurvivalGuiNode.Instance.PawnSpawned += OnPawnSpawned;
                SurvivalGuiNode.Instance.PawnDestroyed += OnPawnDestroyed;
            }

            _playerInteractionVm = HudGui?.PlayerInteractionViewModel;
            _playerInteractionVm.AssertIfNull(nameof(_playerInteractionVm));
            _isInited = _debugView != null && _playerInteractionVm != null;
            _debugView.TraumasText = "";
            _inputTimeWindow = WindowsManager.GetWindow(_inputTimeWindowId);
            _inputTimeWindow.AssertIfNull(nameof(_inputTimeWindow));
        }


        //=== Private =============================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                _hasTraumasFullApiWrapper.EntityApi.UnsubscribeFromActivityChanged(OnTraumasChanged);
                _debugView.TraumasText = "";

                OnTimeStatsSubscribersActivityChanged(false);
                OnAccumStatsSubscribersActivityChanged(false);
                OnProcStatsSubscribersActivityChanged(false);
                _playerPawn = null;

                _hasTraumasFullApiWrapper.Dispose();
                _hasTraumasFullApiWrapper = null;

                _hasStatsBroadcastApiWrapper.Dispose();
                _hasStatsBroadcastApiWrapper = null;

                _hasStatsFullApiWrapper.Dispose();
                _hasStatsFullApiWrapper = null;
            }

            if (newEgo != null)
            {
                _playerPawn = newEgo.GetComponent<ICharacterPawn>();
                if (_playerPawn.AssertIfNull(nameof(_playerPawn)))
                    return;

                _hasTraumasFullApiWrapper = EntityApi.GetWrapper<HasTraumasFullApi>(newEgo.OuterRef);
                _hasStatsBroadcastApiWrapper = EntityApi.GetWrapper<HasStatsBroadcastApi>(newEgo.OuterRef);
                _hasStatsFullApiWrapper = EntityApi.GetWrapper<HasStatsFullApi>(newEgo.OuterRef);

                _hasTraumasFullApiWrapper.EntityApi.SubscribeToTraumasChanged(OnTraumasChanged);

                OnTimeStatsSubscribersActivityChanged();
                OnAccumStatsSubscribersActivityChanged();
                OnProcStatsSubscribersActivityChanged();

                if (EntityInDebugFocus.Entity == null)
                    EntityInDebugFocus.Character = _playerPawn;
            }

            UpdateSubscriptionOnSpells();
        }

        private void OnPawnSpawned(ISubjectPawn characterClientView)
        {
            var characterPawn = characterClientView as ICharacterPawn;
            _characters.Add(characterPawn);
        }

        private void OnPawnDestroyed(ISubjectPawn characterClientView)
        {
            _characters.Remove(characterClientView as ICharacterPawn);
        }

        private string GetFilteredStatsToString(IEnumerable<string> statsToString, string filterToLower)
        {
            return "\n" +
                   (string.IsNullOrEmpty(filterToLower)
                       ? statsToString.OrderBy(sts => sts)
                       : statsToString.Where(sts => sts.ToLower().Contains(filterToLower)).OrderBy(sts => sts))
                   .ItemsToStringByLines(true);
        }

        private string GetPositionInfo()
        {
            ILocomotionDebugInfoProvider nfo = _locomotionDebug.Info;
            Guid entityId = _locomotionDebug.EntityId;
            string entityName = _locomotionDebug.EntityName;

            var sb = new StringBuilder();
            if (nfo != null)
            {
                nfo.IsDemanded = true;

                // Print base info (manually formatted here):
                {
                    var timestamp = DateTime.FromBinary(nfo[TimeStamp].Long);
                    var pos = nfo[BodyPosition].LocomotionVector();
                    var posErr = pos - nfo[VarsPosition].LocomotionVector();
                    var orientation = nfo[BodyOrientation].Float;
                    var velocity = nfo[BodyVelocity].LocomotionVector();
                    var realVelocity = nfo[RealVelocity].LocomotionVector();
                    var speed = new LocomotionVector(LocomotionHelpers.InverseTransformVector(velocity.Horizontal, orientation),
                        velocity.Vertical);
                    var speedErr =
                        new LocomotionVector(
                            LocomotionHelpers.InverseTransformVector(velocity.Horizontal - realVelocity.Horizontal, orientation),
                            velocity.Vertical - realVelocity.Vertical);
                    var moveAxis = nfo[MoveAxes].Vector2;
                    var guide = nfo[GuideAxes].Vector2;
                    sb.AppendLine($"Subject: {entityId}{(entityName != null ? $" ({entityName})" : "")}")
                        .AppendLine($"Last updated: {SharedHelpers.TimeStamp(timestamp)}")
                        .Append($"Position: {pos.ToStringVerbose()}").AppendLine(posErr.ApproximatelyZero() ? "" : $" Err:{posErr}")
                        .Append($"Speed: {velocity.Magnitude:F2} {speed}")
                        .AppendLine(speedErr.ApproximatelyZero() ? "" : $" Err:{speedErr}")
                        .AppendLine($"Orientation: {orientation * Mathf.Rad2Deg:F0} " +
                                    $"({Mathf.DeltaAngle(orientation * Mathf.Rad2Deg, nfo[VarsOrientation].Float * Mathf.Rad2Deg):F2})")
                        .AppendLine($"Angular Speed: {nfo[VarsAngularVelocity].Float * Mathf.Rad2Deg:F0}")
                        .AppendLine($"MoveAxes:{moveAxis} GuideAxes:{guide}")
                        .AppendLine($"Ground: {nfo[DistanceToGround].Float:F2} {(nfo[Airborne].Bool ? "AIR" : "GRN")} ")
                        .AppendLine($"Slope: {Mathf.Asin(nfo[SlopeFactor].Float) * Mathf.Rad2Deg:F2} " +
                                    $"{Mathf.Asin(nfo[SlopeFactorAlongVelocity].Float) * Mathf.Rad2Deg:F2}")
                        .AppendLine($"Flags: {(LocomotionFlags) nfo[MovementFlags].Int}");
                    if (nfo.Contains(StateMachineStateName))
                        sb.AppendLine($"State: {nfo[StateMachineStateName]}");
                    if (nfo.Contains(_currentLocomotionValueId))
                        sb.AppendLine($"{_currentLocomotionValueId}: {nfo[_currentLocomotionValueId]}");
                }
                // Print additional info (w/o manual formatting):
                {
                    sb.AppendLine("<--------------------------------->");
                    ///#PZ-11448: 
                    // foreach (var id in nfo.Ids)
                    //     if (!_ignoreKeysInForeach.Contains(id))
                    //         sb.AppendLine($"{id}: {nfo[id]}");
                    foreach (var id in _showLocomotionKeys)
                    {
                        if (nfo.Contains(id))
                            sb.AppendLine($"{id}: {nfo[id]}");
                    }
                }

                //            if (Time.frameCount%2!=0)
                //            {
                //                var p0 = _locomotionDebug.Transform.position;
                //                Debug.DrawLine(p0, p0 + (LocomotionHelpers.LocomotionToWorldVector(moveAxis).ToUnity() * 2), Color.red, 5f, false);
                //                Debug.DrawLine(p0, p0 + (LocomotionHelpers.LocomotionToWorldVector(guide).ToUnity() * 3), Color.black, 5f, false);
                //            }
            }

            LocomotionDebugTrail trail = _locomotionDebug.Trail;
            if (trail)
            {
                sb.AppendLine($"Trail: {(trail.Recording ? ((Time.frameCount % 40) > 20 ? "REC" : "___") : "")} {trail.TrailMode}");
                if (trail.SkipFrames > 0)
                    sb.AppendLine(
                        $"Frame[#{trail.SkipFrames}|{trail.GetLastFrameTimestamp().ToLocalTime():h:mm:ss.fff}] {trail.TrailMode}:{trail.GetLastFrameString(trail.TrailMode)}");
            }
                
            return sb.ToString();
        }

        ///#PZ-11448: #Tmp? restored:
        // // Add here keys, you handle (format) manually at `.GetPositionInfo()`:
        // private readonly HashSet<string> _ignoreKeysInForeach = new HashSet<string>()
        // {
        //     "TimeStamp", "Position", "VarsPosition", "Orientation", "Velocity", "RealVelocity", "MoveAxes", "GuideAxes", "VarsOrientation",
        //     "VarsAngularVelocity", "DistanceToGround", "Airborne", "GroundContact", "SlopeFactor", "SlopeFactorAlongVelocity",
        //     "MovementFlags",  "StateMachineStateName", "IsDirectControl", "DepenetrationCorrected"
        // };
        private void UpdateView()
        {
            if (_debugView.IsVisibleTimes && _timesUpdateInterval.IsItTime())
            {
                _debugView.SyncTimeString = GetSyncTime();
                _debugView.RegionTimeString = GetRegionTime();
            }

            ProcessSpells();
            
            if (_playerPawn != null)
            {
                var entityRef = _playerPawn.EntityRef;

                if (_debugView.IsVisibleMutation && _mutationUpdateInterval.IsItTime())
                {
                    _debugView.MutationText = _asyncMutationText;

                    var repository = ClusterCommands.ClientRepository;
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        using (var wrapper = await repository.Get<IWorldCharacterClientFull>(entityRef.Guid))
                        {
                            var worldCharacter = wrapper.Get<IWorldCharacterClientFull>(entityRef.Guid);
                            _asyncMutationText =
                                $"Faction: {worldCharacter.MutationMechanics.Mutation} ({worldCharacter.MutationMechanics.Stage.NameLs.GetText()})";
                        }
                    });
                }

                if (_debugView.IsVisibleInteractiveObject && _interactiveObjectUpdateInterval.IsItTime())
                {
                    _debugView.TargetGo = _playerInteractionVm.SelectedTarget;
                    _debugView.LastInteractionSpell = _playerInteractionVm.LastInteractionSpell;
                }

                if (_debugView.IsMoveActionsVisible)
                {
                    if (_selectedPawn != null && _moveActionsUpdateInterval.IsItTime())
                    {
                        if (!_debugViewIsMoveActionsVisiblePrevVal && EntityInDebugFocus.Mob != null)
                            StartObservationMob(EntityInDebugFocus.Mob);

                        if (_dbgInfoProviders == null)
                        {
                            // 1) Get from GOComponents:
                            _dbgInfoProviders = _selectedPawn.Ego.GetComponents<IDebugInfoProvider>().ToList();
                            // 2) Add from others i-face implementations:
                            if (_cashedLegHostOfMobInFocus != null)
                                _dbgInfoProviders.Add(_cashedLegHostOfMobInFocus);
                        }

                        if (_dbgInfoProviders.Count == 0)
                            return;

                        var sb = new StringBuilder();
                        //foreach (var provider in _dbgInfoProviders)
                        foreach (var provider in _dbgInfoProviders)
                        {
                            sb.AppendLine(provider.GetDebugInfo());
                            sb.AppendLine("<--------------------------->");
                        }

                        _debugView.MoveActionsInfo = sb.ToString();
                    }
                }
                else
                {
                    if (_debugViewIsMoveActionsVisiblePrevVal && EntityInDebugFocus.Mob != null)
                        FinishObservationMob(EntityInDebugFocus.Mob);
                }

                _debugViewIsMoveActionsVisiblePrevVal = _debugView.IsMoveActionsVisible;
            }

            if (_movesUpdateInterval.IsItTime())
            {
                if (_debugView.IsVisiblePlayerMove)
                    _debugView.MoveText = GetPositionInfo();

                if (_debugView.IsVisiblePlayerPos && _playerPawn != null)
                    _debugView.PlayerPosText = _playerPawn.Ego.transform.position.ToString();
            }

            if (_pathfindingOwnershipUpdateInterval.IsItTime())
            {
                UpdatePathfindingOwnershipVisualization();
            }
        }

        private void UpdateSubscriptionOnSpells()
        {
            var newSpellsEntityRef = _debugView.IsVisibleSpells && _playerPawn != null ? _playerPawn.EntityRef.To<IHasWizardEntityClientFull>() : OuterRef<IHasWizardEntityClientFull>.Invalid;
            var oldSpellsEntityRef = _spellsEntityRef;

            var newBuffsEntityRef = newSpellsEntityRef.To<IHasBuffsClientFull>();
            var oldBuffsEntityRef = oldSpellsEntityRef.To<IHasBuffsClientFull>();

            if (newSpellsEntityRef != oldSpellsEntityRef)
            {
                _spellsEntityRef = newSpellsEntityRef;

                AsyncUtils.RunAsyncTask(async () =>
                {
                    if (oldSpellsEntityRef.IsValid && GameState.Instance.ClientClusterNode != null)
                        await ClusterHelpers.UseWizard(oldSpellsEntityRef, GameState.Instance.ClientClusterNode, wizard =>
                        {
                            wizard.Spells.OnItemAddedOrUpdated -= OnSpellAdded;
                            wizard.Spells.OnItemRemoved -= OnSpellRemoved;
                            Logger.IfDebug()?.Message(oldSpellsEntityRef.Guid, "Unsubscribed from spells")
                                .Write();
                            return Task.CompletedTask;
                        });
    
                    if (oldBuffsEntityRef.IsValid && GameState.Instance.ClientClusterNode != null)
                        await ClusterHelpers.Use(oldBuffsEntityRef, GameState.Instance.ClientClusterNode, ReplicationLevel.ClientFull, hasBuffs =>
                        {
                            hasBuffs.Buffs.All.OnItemAddedOrUpdated -= OnBuffAdded;
                            hasBuffs.Buffs.All.OnItemRemoved -= OnBuffRemoved;
                            Logger.IfDebug()?.Message(oldBuffsEntityRef.Guid, "Unsubscribed from buffs").Write();
                            return Task.CompletedTask;
                        });

                    lock (_spellsLock)
                        _spells.Clear();

                    if (newSpellsEntityRef.IsValid && GameState.Instance.ClientClusterNode != null)
                        await ClusterHelpers.UseWizard(newSpellsEntityRef, GameState.Instance.ClientClusterNode, async wizard =>
                        {
                            foreach(var spell in wizard.Spells.Values)
                                await OnSpellAdded(wizard, spell);
                            wizard.Spells.OnItemAddedOrUpdated += OnSpellAdded;
                            wizard.Spells.OnItemRemoved += OnSpellRemoved;
                            Logger.IfDebug()?.Message(newSpellsEntityRef.Guid, "Subscribed to spells")
                                .Write();
                        });
                
                    if (newBuffsEntityRef.IsValid && GameState.Instance.ClientClusterNode != null)
                        await ClusterHelpers.Use(newBuffsEntityRef, GameState.Instance.ClientClusterNode, ReplicationLevel.ClientFull, async hasBuffs =>
                        {
                            foreach(var spell in hasBuffs.Buffs.All.Values)
                                await OnBuffAdded(hasBuffs, spell);
                            hasBuffs.Buffs.All.OnItemAddedOrUpdated += OnBuffAdded;
                            hasBuffs.Buffs.All.OnItemRemoved += OnBuffRemoved;
                            Logger.IfDebug()?.Message(newBuffsEntityRef.Guid, "Subscribed to buffs").Write();
                        });
                });
            }
        }

        private void ProcessSpells()
        {
            UpdateSubscriptionOnSpells();
            
            if (_debugView.IsVisibleSpells && _statsUpdateInterval.IsItTime())
            {
                SpellInfo[] runningSpells, finishedSpells;
                lock (_spellsLock)
                {
                    _spells.RemoveAll(x => x.FinishedAt > 0 && (SyncTime.Now - x.FinishedAt) > SyncTime.FromSeconds(SpellObsolescenceTime));
                    runningSpells = _spells.Where(x => x.FinishedAt <= 0).ToArray();
                    finishedSpells = _spells.Except(runningSpells).ToArray();
                }
                var text = GetFilteredStatsToString(runningSpells.Select(s => s.ToString()), _debugView.SpellsFilter.ToLower());
                if (finishedSpells.Length > 0)
                    text += "\n\n-- FINISHED --------------------------"
                            + GetFilteredStatsToString(finishedSpells.Select(s => s.ToString()), _debugView.SpellsFilter.ToLower());
                _debugView.SpellsText = text;
            }
        }

        private async Task OnSpellAdded(DeltaDictionaryChangedEventArgs<SpellId, ISpellClientFull> args)
        {
            var wizardRef = new OuterRef<IWizardEntityClientFull>(args.Sender.ParentEntityId, args.Sender.ParentTypeId);
            await ClusterHelpers.Use(wizardRef, GameState.Instance.ClientClusterNode, ReplicationLevel.ClientFull, w => OnSpellAdded(w,args.Value));
        }

        private async Task OnSpellRemoved(DeltaDictionaryChangedEventArgs<SpellId, ISpellClientFull> args)
        {
            var wizardRef = new OuterRef<IWizardEntityClientFull>(args.Sender.ParentEntityId, args.Sender.ParentTypeId);
            await ClusterHelpers.Use(wizardRef, GameState.Instance.ClientClusterNode, ReplicationLevel.ClientFull, w => OnSpellRemoved(w,args.Value));
        }
        
        private Task OnSpellAdded(IWizardEntityClientFull wizard, ISpellClientFull spell)
        {
            Logger.IfDebug()?.Message(wizard.Owner.Guid, $"Spell added: {spell.Id} {spell.CastData}").Write();
            AddSpellInfo(new SpellInfo(spell.Id, spell.CastData.Def, spell.Started, spell.Finished));
            return Task.CompletedTask;
        }
        
        private Task OnSpellRemoved(IWizardEntityClientFull wizard, ISpellClientFull spell)
        {
            Logger.IfDebug()?.Message(wizard.Owner.Guid, $"Spell removed: {spell.Id} {spell.CastData}").Write();
            AddSpellInfo(new SpellInfo(spell.Id, spell.CastData.Def, spell.Started, spell.Finished > 0 ? spell.Finished : SyncTime.Now, spell.FinishReason));
            return Task.CompletedTask;
        }
        
        

        private async Task OnBuffAdded(DeltaDictionaryChangedEventArgs<SpellId, IBuffClientFull> args)
        {
            var wizardRef = new OuterRef<IHasBuffsClientFull>(args.Sender.ParentEntityId, args.Sender.ParentTypeId);
            await ClusterHelpers.Use(wizardRef, GameState.Instance.ClientClusterNode, ReplicationLevel.ClientFull, w => OnBuffAdded(w,args.Value));
        }

        private async Task OnBuffRemoved(DeltaDictionaryChangedEventArgs<SpellId, IBuffClientFull> args)
        {
            var wizardRef = new OuterRef<IHasBuffsClientFull>(args.Sender.ParentEntityId, args.Sender.ParentTypeId);
            await ClusterHelpers.Use(wizardRef, GameState.Instance.ClientClusterNode, ReplicationLevel.ClientFull, w => OnBuffRemoved(w,args.Value));
        }
        
        private Task OnBuffAdded(IHasBuffsClientFull wizard, IBuffClientFull buff)
        {
            Logger.IfDebug()?.Message(wizard.ParentEntityId, $"Buff added: {buff.Id} {buff.Def.____GetDebugAddress()}").Write();
            AddSpellInfo(new SpellInfo(buff.Id, buff.Def, buff.StartTime, buff.Finished ? buff.EndTime : 0));
            return Task.CompletedTask;
        }
        
        private Task OnBuffRemoved(IHasBuffsClientFull wizard, IBuffClientFull buff)
        {
            Logger.IfDebug()?.Message(wizard.ParentEntityId, $"Buff removed: {buff.Id} {buff.Def.____GetDebugAddress()}").Write();
            AddSpellInfo(new SpellInfo(buff.Id, buff.Def, buff.StartTime, buff.Finished ? buff.EndTime : SyncTime.Now));
            return Task.CompletedTask;
        }

        private void AddSpellInfo(SpellInfo nfo)
        {
            lock (_spellsLock)
            {
                var idx = _spells.FindIndex(x => x.Def == nfo.Def && x.Id == nfo.Id);
                if (idx != -1)
                    _spells[idx] = nfo;
                else
                    _spells.Add(nfo);
                _spells.Sort((x,y) => (int)(y.StartedAt - x.StartedAt));
            }
        }
        
        
        private async Task OnSpellsChanged(DeltaDictionaryChangedEventArgs args)
        {
            var wizardRef = new OuterRef<IWizardEntityClientFull>(args.Sender.ParentEntityId, args.Sender.ParentTypeId);
            await ClusterHelpers.Use(wizardRef, GameState.Instance.ClientClusterNode, ReplicationLevel.ClientFull, OnSpellsChanged);
        }

        private Task OnSpellsChanged(IWizardEntityClientFull wizard)
        {
            var spells = wizard.Spells;
            Logger.IfDebug()?.Message(spells.ParentEntityId, "Spells changed")
                .Write();
            var nfo = spells.Select(x => new SpellInfo(x.Value.Id, x.Value.CastData.Def, x.Value.Started, x.Value.Finished)).ToArray();
            lock (_spellsLock)
            {
                var oldSpellsCount = _spells.Count;
                for (int i = 0; i < nfo.Length; ++i)
                {
                    for (int j = 0; j < oldSpellsCount; ++j)
                        if (nfo[i].Id == _spells[j].Id)
                        {
                            --oldSpellsCount;
                            (_spells[j], _spells[oldSpellsCount]) = (_spells[oldSpellsCount], nfo[i]);
                            goto next;
                        }
                    _spells.Add(nfo[i]);
                    next: ;
                }
                for (int i = 0; i < oldSpellsCount; ++i)
                {
                    var sp = _spells[i];
                    if (sp.FinishedAt <= 0)
                        _spells[i] = new SpellInfo(sp.Id, sp.Def, sp.StartedAt, SyncTime.Now);
                }
                _spells.Sort((x,y) => (int)(y.StartedAt - x.StartedAt));
                Logger.IfDebug()?.Message(spells.ParentEntityId, $"Spells: {string.Join("\n", _spells)}")
                    .Write();
            }
            return Task.CompletedTask;
        }

        private bool _debugViewIsMoveActionsVisiblePrevVal;

        ///#PZ-9704: ?toMe?: is array[] ok?
        private List<IDebugInfoProvider> /*IDebugInfoProvider[]*/
            _dbgInfoProviders;

        private void OnTraumasChanged(IReadOnlyDictionary<string, TraumaDef> newTraumas)
        {
            _debugView.TraumasText = newTraumas.Select(v => $"{v.Key}: {v.Value.____GetDebugShortName()}").ItemsToStringByLines(true);
        }

        private string GetSyncTime()
        {
            return $"{SyncTime.Now} ({SyncTime.DeltaToServer})";
        }

        private string GetRegionTime()
        {
            var time = RegionTime.IngameTime;
            return $"Day {time.Day} Time: {time.Hour}:{time.Minute:#}:{time.Second:#}";
        }

        private DebugTimelineChart.Value GetChartValue(float dt)
        {
            var debugInfo = _locomotionDebug.Info;
            if (debugInfo != null)
            {
                var v = debugInfo[_currentLocomotionValueId];
                switch (v.ValueType)
                {
                    case Value.Type.Float:
                        return new DebugTimelineChart.Value(v.Float);
                    case Value.Type.Vector3:
                        return new DebugTimelineChart.Value(new Vector3(v.Vector3.x, v.Vector3.y, v.Vector3.z));
                    case Value.Type.Vector2:
                        return new DebugTimelineChart.Value(new Vector3(v.Vector2.x, v.Vector2.y, 0));
                    case Value.Type.Int:
                        return new DebugTimelineChart.Value(v.Int);
                    case Value.Type.Bool:
                        return new DebugTimelineChart.Value(v.Bool);
                }
            }

            return default(DebugTimelineChart.Value);
        }

        private GameObject _markerGo;
        private IEntityPawn _selectedPawn;

        private void SetSelectionMark(IEntityPawn obj)
        {
            _selectedPawn = obj;
            _dbgInfoProviders = null;
            foreach (var outline in _outlines)
                DestroyImmediate(outline);
            if (obj == null)
                return;

            // Marker g.o.:
            AttachMarker(obj);

            if (obj != _playerPawn)
            {
                // Outline:
                var renderers = obj.Ego.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    var outline = renderer.gameObject.GetOrAddComponent<Outline>();
                    outline.ColorDelegate = () => OutlineColor.Red;
                    _outlines.Add(outline);
                }
            }
        }

        private void AttachMarker(IEntityPawn obj)
        {
            if (obj == _playerPawn || _playerPawn == null)
            {
                SwitchMarkOffIfShould();
                return;
            }

            if (obj != null)
                SwitchMarkOnIfShould(obj.Ego.gameObject);
        }

        private void SwitchMarkOnIfShould(GameObject obj)
        {
            if (_markerGo == null)
            {
                if (_markerPrefab != null)
                    _markerGo = Instantiate(_markerPrefab, obj.transform);
            }
            else
            {
                _markerGo.transform.SetPositionAndRotation(obj.transform.position, obj.transform.rotation);
                _markerGo.transform.SetParent(obj.transform);
                _markerGo.SetActive(true);
            }
        }

        private void SwitchMarkOffIfShould()
        {
            if (_markerGo == null)
                return;

            _markerGo.SetActive(false);
            _markerGo.transform.SetParent(null);
        }

        private bool GetIsVisibleToggles()
        {
            return Input.mousePosition.y < 30;
        }

        private void CreateStatKindSubscribers()
        {
            _statKindSubscribers = new Dictionary<StatKind, StatKindSubscribers>()
            {
                {StatKind.Time, new StatKindSubscribers(StatKind.Time, _statDebugViewModelPrefab, _timeStatsRoot)},
                {StatKind.Accumulated, new StatKindSubscribers(StatKind.Accumulated, _statDebugViewModelPrefab, _accumStatsRoot)},
                {StatKind.Procedural, new StatKindSubscribers(StatKind.Procedural, _statDebugViewModelPrefab, _procStatsRoot)}
            };
        }


        //=== Classes, structs ============================================================================================

        private struct LocomotionDebugProviders
        {
            public ILocomotionDebugInfoProvider Info => InfoProvider?.Invoke();
            public LocomotionDebugTrail Trail => TrailProvider?.Invoke();
            public Guid EntityId;
            public string EntityName;
            public Transform Transform;
            public Func<ILocomotionDebugInfoProvider> InfoProvider;
            public Func<LocomotionDebugTrail> TrailProvider;
        }

        private readonly struct SpellInfo
        {
            public readonly SpellId Id;
            public readonly BaseResource Def;
            public readonly long StartedAt;
            public readonly long FinishedAt;
            private readonly SpellFinishReason _reason;

            public SpellInfo(SpellId id, BaseResource def, long startedAt, long finishedAt, SpellFinishReason reason = SpellFinishReason.None)
            {
                Id = id;
                Def = def;
                StartedAt = startedAt;
                FinishedAt = finishedAt;
                _reason = reason;
            }

            public override string ToString()
            {
                var sb = StringBuildersPool.Get;
                sb.Append("[").Append(Id).Append("] ")
                    .Append(Def?.____GetDebugShortName())
                    .Append(" | ")
                    .Append(SyncTime.ToSeconds((FinishedAt > 0 ? FinishedAt : SyncTime.Now) - StartedAt).ToString("F2"));
                if (FinishedAt > 0)
                    sb.Append(" | ")
                        .Append(_reason.ToString());
                return sb.ToStringAndReturn();
            }
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            //VisibilityGridsDebugger.DrawVisibilityGrids();
        }
#endif        
    }
}