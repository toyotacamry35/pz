using System;
using System.Linq;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.GameObjectAssembler;
using Assets.Src.SpawnSystem;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using SharedCode.Repositories;
using SharedCode.Wizardry;
using Src.Debugging;
using Uins;
using UnityEngine;
using UnityWeld.Binding;

[Binding]
public class DebugView : BindingViewModel
{
    public static DebugView Instance;


    //=== Props ===============================================================

    [Binding]
    public bool IsVisible
    {
        get => DebugExtension.Draw;
        set
        {
            if (DebugExtension.Draw != value)
            {
                DebugExtension.Draw = value;
                PlayerPrefs.Save();
                NotifyPropertyChanged();
                NeedForInputTimeWindow = GetNeedForInputTimeWindow();
            }
        }
    }

    private const string IsVisibleLinearStatsKey = "DebugIsVisibleLinearStats";

    [Binding]
    public bool IsVisibleLinearStats
    {
        get => UniquePlayerPrefs.GetBool(IsVisibleLinearStatsKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsVisibleLinearStatsKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsVisibleLinearStatsKey, value);
                NotifyPropertyChanged();
            }
        }
    }

    private bool _isMoveActionsVisible;

    [Binding]
    public bool IsMoveActionsVisible
    {
        get => _isMoveActionsVisible;
        set
        {
            if (_isMoveActionsVisible != value)
            {
                _isMoveActionsVisible = value;
                NotifyPropertyChanged();
            }
        }
    }

    private bool _isPathfindingOwnershipVisible;
    
    [Binding]
    public bool IsPathfindingOwnershipVisible
    {
        get => _isPathfindingOwnershipVisible;
        set
        {
            if (_isPathfindingOwnershipVisible != value)
            {
                _isPathfindingOwnershipVisible = value;
                NotifyPropertyChanged();
            }
        }
    }

    private string _moveActionsInfo;

    [Binding]
    public string MoveActionsInfo
    {
        get => _moveActionsInfo;
        set
        {
            if (_moveActionsInfo != value)
            {
                _moveActionsInfo = value;
                NotifyPropertyChanged();
            }
        }
    }

    private const string IsVisibleAccumStatsKey = "DebugIsVisibleAccumStats";

    [Binding]
    public bool IsVisibleAccumStats
    {
        get => UniquePlayerPrefs.GetBool(IsVisibleAccumStatsKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsVisibleAccumStatsKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsVisibleAccumStatsKey, value);
                NotifyPropertyChanged();
            }
        }
    }

    private const string IsVisibleProcStatsKey = "DebugIsVisibleProcStats";

    [Binding]
    public bool IsVisibleProcStats
    {
        get => UniquePlayerPrefs.GetBool(IsVisibleProcStatsKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsVisibleProcStatsKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsVisibleProcStatsKey, value);
                NotifyPropertyChanged();
            }
        }
    }

    private const string IsVisibleSpellsKey = "DebugIsVisibleSpells";

    [Binding]
    public bool IsVisibleSpells
    {
        get => UniquePlayerPrefs.GetBool(IsVisibleSpellsKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsVisibleSpellsKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsVisibleSpellsKey, value);
                if (value)
                    SpellsText = "";
                NotifyPropertyChanged();
            }
        }
    }

    private string _spellsText;

    [Binding]
    public string SpellsText
    {
        get => _spellsText;
        set
        {
            if (_spellsText != value)
            {
                _spellsText = value;
                NotifyPropertyChanged();
            }
        }
    }

    private const string IsVisibleTraumasKey = "DebugIsVisibleTraumas";

    [Binding]
    public bool IsVisibleTraumas
    {
        get => UniquePlayerPrefs.GetBool(IsVisibleTraumasKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsVisibleTraumasKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsVisibleTraumasKey, value);
                NotifyPropertyChanged();
            }
        }
    }

    private string _traumasText;

    [Binding]
    public string TraumasText
    {
        get => _traumasText;
        set
        {
            if (_traumasText != value)
            {
                _traumasText = value;
                NotifyPropertyChanged();
            }
        }
    }

    private const string IsVisiblePlayerMoveKey = "DebugIsVisiblePlayerMove";

    [Binding]
    public bool IsVisiblePlayerMove
    {
        get => UniquePlayerPrefs.GetBool(IsVisiblePlayerMoveKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsVisiblePlayerMoveKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsVisiblePlayerMoveKey, value);
                if (value)
                    MoveText = "";
                NotifyPropertyChanged();
            }
        }
    }

    private string _moveText;

    [Binding]
    public string MoveText
    {
        get => _moveText;
        set
        {
            if (_moveText != value)
            {
                _moveText = value;
                NotifyPropertyChanged();
            }
        }
    }

    private const string IsVisiblePlayerPosKey = "DebugIsVisiblePlayerPos";

    [Binding]
    public bool IsVisiblePlayerPos
    {
        get => UniquePlayerPrefs.GetBool(IsVisiblePlayerPosKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsVisiblePlayerPosKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsVisiblePlayerPosKey, value);
                if (value)
                    PlayerPosText = "-";
                NotifyPropertyChanged();
            }
        }
    }

    private string _playerPosText;

    [Binding]
    public string PlayerPosText
    {
        get => _playerPosText;
        set
        {
            if (_playerPosText != value)
            {
                _playerPosText = value;
                NotifyPropertyChanged();
            }
        }
    }

    private const string IsVisibleDebugMsgKey = "DebugIsVisibleDebugMsg";

    [Binding]
    public bool IsVisibleDebugMsg
    {
        get => UniquePlayerPrefs.GetBool(IsVisibleDebugMsgKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsVisibleDebugMsgKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsVisibleDebugMsgKey, value);
                NotifyPropertyChanged();
            }
        }
    }

    private string _debugMessage;

    [Binding]
    public string DebugMessage
    {
        get => _debugMessage;
        set
        {
            if (_debugMessage != value)
            {
                _debugMessage = value;
                NotifyPropertyChanged();
            }
        }
    }

    private const string IsVisibleTimesKey = "DebugIsVisibleTimes";

    [Binding]
    public bool IsVisibleTimes
    {
        get => UniquePlayerPrefs.GetBool(IsVisibleTimesKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsVisibleTimesKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsVisibleTimesKey, value);
                if (value)
                    SyncTimeString = RegionTimeString = "";
                NotifyPropertyChanged();
            }
        }
    }

    private string _syncTimeString;

    [Binding]
    public string SyncTimeString
    {
        get => _syncTimeString;
        set
        {
            if (_syncTimeString != value)
            {
                _syncTimeString = value;
                NotifyPropertyChanged();
            }
        }
    }

    private string _regionTimeString;

    [Binding]
    public string RegionTimeString
    {
        get => _regionTimeString;
        set
        {
            if (_regionTimeString != value)
            {
                _regionTimeString = value;
                NotifyPropertyChanged();
            }
        }
    }

    private const string IsVisibleInteractiveObjectKey = "DebugIsVisibleInteractiveObject";

    [Binding]
    public bool IsVisibleInteractiveObject
    {
        get => UniquePlayerPrefs.GetBool(IsVisibleInteractiveObjectKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsVisibleInteractiveObjectKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsVisibleInteractiveObjectKey, value);
                if (value)
                    TargetGo = null;
                NotifyPropertyChanged();
            }
        }
    }

    private GameObject _targetGo;

    public GameObject TargetGo
    {
        get => _targetGo;
        set
        {
            if (_targetGo != value)
            {
                _targetGo = value;
                _interactive = _targetGo?.GetComponent<Interactive>();
                _jsonRefHolder = _targetGo?.GetComponent<JsonRefHolder>();
                _knowledgeHolder = _targetGo?.GetComponent<KnowledgeHolder>();
                NotifyPropertyChanged(nameof(TargetGoName));
                NotifyPropertyChanged(nameof(HasInteractiveComponent));
                NotifyPropertyChanged(nameof(JsonRefHolderRef));
                NotifyPropertyChanged(nameof(KnowledgeHolderRef));
                NotifyPropertyChanged(nameof(LocalObjectDef));
                NotifyPropertyChanged(nameof(EntityObjectDef));
            }
        }
    }

    private SpellDef _lastInteractionSpell;

    public SpellDef LastInteractionSpell
    {
        get => _lastInteractionSpell;
        set
        {
            if (_lastInteractionSpell != value)
            {
                _lastInteractionSpell = value;
                NotifyPropertyChanged(nameof(LastInteractionSpellName));
            }
        }
    }

    [Binding]
    public string LastInteractionSpellName => LastInteractionSpell?.ToString() ?? "";

    [Binding]
    public string TargetGoName => TargetGo?.name;

    private Interactive _interactive;

    [Binding]
    public bool HasInteractiveComponent => _interactive != null;

    private KnowledgeHolder _knowledgeHolder;

    [Binding]
    public string KnowledgeHolderRef => _knowledgeHolder?.KnowledgeAdress;

    private JsonRefHolder _jsonRefHolder;

    [Binding]
    public string JsonRefHolderRef => null;//_jsonRefHolder?.Ref;

    [Binding]
    public string LocalObjectDef => _interactive?.LocalObjectDef?.ToString() ?? "";

    [Binding]
    public string EntityObjectDef => _interactive?.EntityObjectDef?.ToString() ?? "";

    private const string IsTransparentScrollsKey = "DebugIsTransparentScrolls";

    [Binding, UsedImplicitly]
    public bool IsTransparentScrolls
    {
        get => UniquePlayerPrefs.GetBool(IsTransparentScrollsKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsTransparentScrollsKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsTransparentScrollsKey, value);
                NotifyPropertyChanged();
            }
        }
    }

    [Binding]
    public bool IsVisibleFps
    {
        get => DebugState.Instance.IsVisibleFps > 0;
        set
        {
            if (IsVisibleFps != value)
            {
                DebugState.Instance.IsVisibleFps = value ? 2 : 0;
                NotifyPropertyChanged();
            }
        }
    }
    
    private const string IsVisibleTimelineKey = "DebugIsVisibleTimeline";

    [Binding]
    public bool IsVisibleTimeline
    {
        get => UniquePlayerPrefs.GetBool(IsVisibleTimelineKey);
        set
        {
            if (UniquePlayerPrefs.GetBool(IsVisibleTimelineKey) != value)
            {
                UniquePlayerPrefs.SetBool(IsVisibleTimelineKey, value);
                NotifyPropertyChanged();
            }
        }
    }

    private bool _isVisibleToggles;

    [Binding]
    public bool IsVisibleToggles
    {
        get => _isVisibleToggles;
        set
        {
            if (_isVisibleToggles != value)
            {
                _isVisibleToggles = value;
                NotifyPropertyChanged();
            }
        }
    }

    private string _timeStatsFilter = "";

    [Binding]
    public string TimeStatsFilter
    {
        get => _timeStatsFilter;
        set
        {
            if (_timeStatsFilter != value)
            {
                _timeStatsFilter = value;
                NotifyPropertyChanged();
            }
        }
    }

    private string _accumStatsFilter = "";

    [Binding]
    public string AccumStatsFilter
    {
        get => _accumStatsFilter;
        set
        {
            if (_accumStatsFilter != value)
            {
                _accumStatsFilter = value;
                NotifyPropertyChanged();
            }
        }
    }

    private string _procStatsFilter = "";

    [Binding]
    public string ProcStatsFilter
    {
        get => _procStatsFilter;
        set
        {
            if (_procStatsFilter != value)
            {
                _procStatsFilter = value;
                NotifyPropertyChanged();
            }
        }
    }

    private string _spellsFilter = "";

    [Binding]
    public string SpellsFilter
    {
        get => _spellsFilter;
        set
        {
            if (_spellsFilter != value)
            {
                _spellsFilter = value;
                NotifyPropertyChanged();
            }
        }
    }

    private bool _isVisibleMutation;

    [Binding]
    public bool IsVisibleMutation
    {
        get => _isVisibleMutation;
        set
        {
            if (_isVisibleMutation != value)
            {
                _isVisibleMutation = value;
                if (value)
                    MutationText = "";
                NotifyPropertyChanged();
            }
        }
    }

    private string _mutationText = "";

    [Binding]
    public string MutationText
    {
        get => _mutationText;
        set
        {
            if (_mutationText != value)
            {
                _mutationText = value;
                NotifyPropertyChanged();
            }
        }
    }

    private bool _inTextInputMode;

    public bool InTextInputMode
    {
        get => _inTextInputMode;
        set
        {
            if (_inTextInputMode != value)
            {
                _inTextInputMode = value;
                NeedForInputTimeWindow = GetNeedForInputTimeWindow();
            }
        }
    }

    private bool _needForInputTimeWindow;

    [Binding]
    public bool NeedForInputTimeWindow
    {
        get => _needForInputTimeWindow;
        set
        {
            if (_needForInputTimeWindow != value)
            {
                _needForInputTimeWindow = value;
                NotifyPropertyChanged();
            }
        }
    }

    private void OnIsVisibleFpsChanged(int obj)
    {
        NotifyPropertyChanged(nameof(IsVisibleFps));
    }

    //=== Unity ===============================================================

    private void Awake()
    {
        Instance = SingletonOps.TrySetInstance(this, Instance);
#if !UNITY_EDITOR
        IsVisible = false;
#endif
        DebugMessage = "";
        NotifyPropertyChanged(nameof(IsVisibleFps));
        DebugState.Instance.OnIsVisibleFpsChanged += OnIsVisibleFpsChanged;
    }


    protected override void OnDestroy()
    {
        DebugState.Instance.OnIsVisibleFpsChanged -= OnIsVisibleFpsChanged;
        base.OnDestroy();
        if (Instance == this)
            Instance = null;
    }


    //=== Private =============================================================

    private bool GetNeedForInputTimeWindow()
    {
        return IsVisible && InTextInputMode;
    }
}