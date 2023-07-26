using System;
using ReactivePropsNs;
using UnityEngine;

public class VideoSettings : IDisposable
{
    private const string IsFpsLimitedKey = "IsFpsLimited";
    private DisposableComposite _d = new DisposableComposite();
    private bool noDevice = false;

    private bool isFixedFPS = false;
    //=== Props ===============================================================

    public static VideoSettings Instance { get; private set; }

    public ReactiveProperty<bool> IsFpsLimitedRp { get; } = new ReactiveProperty<bool>();

    private bool SavedIsFpsLimited
    {
        get => UniquePlayerPrefs.GetBool(IsFpsLimitedKey, false);
        set => UniquePlayerPrefs.SetBool(IsFpsLimitedKey, value);
    }


    //=== Ctor ================================================================

    public VideoSettings()
    {
        if (Instance == null)
            Instance = this;

        noDevice = (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        isFixedFPS = (Array.IndexOf(Environment.GetCommandLineArgs(), "--fixed-fps") >= 0);

        //Единственный раз читаем сохраненные значения
        IsFpsLimitedRp.Value = SavedIsFpsLimited;
        //Связываем изменения значений Rp с 1) изменением реальных параметров, 2) сохранением в UniquePlayerPrefs (реестре)
        IsFpsLimitedRp.Action(_d, b =>
        {
            SavedIsFpsLimited = b;
            OnFpsLimitedChanged(b);
        });
    }


    //=== Public ==============================================================

    public void Dispose()
    {
        if (Instance == this)
            Instance = null;
    }


    //=== Private =============================================================

    private void OnFpsLimitedChanged(bool isFpsLimited)
    {
        if (isFixedFPS || noDevice)
        {
            Application.targetFrameRate = (int) (1.0f / Mathf.Max(Time.fixedDeltaTime, 0.02f));
        }
        else
        {
            Application.targetFrameRate = isFpsLimited ? 60 : -1;
        }
    }
}