using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.Hints;
using Assets.Src.ResourceSystem;
using DG.Tweening;
using L10n;
using SharedCode.Utils;
using Uins;
using UnityEngine;
using UnityWeld.Binding;

[Binding]
public class LoadingScreenView : BindingViewModel
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public Transform LoadingPanel;
    public RandomImageSetter RandomImageSetter;
    public JdbMetadata HintsJdb;

    private HintsDef _hints => HintsJdb?.Get<HintsDef>();
    private int _currentHintIndex = 0;
    private List<int> _indexes;
    private Tweener _rotationTweener;
    private DateTime _lastSwitchDateTime;

    private Coroutine _coroutine;

    private LocalizedString _hint;

    [Binding]
    public LocalizedString Hint
    {
        get { return _hint; }
        set
        {
            if (!_hint.Equals(value))
            {
                _hint = value;
                NotifyPropertyChanged();
            }
        }
    }


    //=== Unity ===============================================================

    private void Awake()
    {
        LoadingPanel.AssertIfNull(nameof(LoadingPanel));
        RandomImageSetter.AssertIfNull(nameof(RandomImageSetter));

        UnityEngine.Random.InitState((int)DateTime.UtcNow.ToUnix());
        _indexes = Enumerable.Range(0, _hints.Hints.Length).OrderBy(x => UnityEngine.Random.value).ToList();
        _lastSwitchDateTime = DateTime.Now;
    }

    private IEnumerator RunHints()
    {
        if (_indexes.Count == 0)
            yield return null;

        while (true)
        {
            int hintIndex = ++_currentHintIndex >= _indexes.Count ? 0 : _currentHintIndex;
            var hint = _hints.Hints[hintIndex];
            Hint = hint.TextLs;

            yield return new WaitForSeconds(hint.Time);
        }
    }

    //=== Public ==============================================================

    public void ShowLoadingScreen()
    {
        RandomImageSetter.SetRandomImage();
        _coroutine = this.StartInstrumentedCoroutine(RunHints());
        LoadingPanel.gameObject.SetActive(true);
        TimeDebugInfo(true); //DEBUG
    }

    private void TimeDebugInfo(bool isOn)
    {
        UI.CallerLogInfo($"<{typeof(LoadingScreenView)}> {(isOn ? "SHOW" : "HIDE")} {((DateTime.Now - _lastSwitchDateTime).TotalSeconds):f1}s");
        _lastSwitchDateTime = DateTime.Now;
    }

    public void HideLoadingScreen()
    {
        if (_rotationTweener != null)
        {
            _rotationTweener.Kill();
            _rotationTweener = null;
        }

        if(_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = null;

        LoadingPanel.gameObject.SetActive(false);
        TimeDebugInfo(false); //DEBUG
    }
}