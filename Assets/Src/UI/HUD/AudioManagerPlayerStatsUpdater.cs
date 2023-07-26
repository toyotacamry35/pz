using System.ComponentModel;
using NLog;
using Uins;
using UnityEngine;

public class AudioManagerPlayerStatsUpdater : MonoBehaviour
{
    private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

    public PlayerMainStatsViewModel ViewModel;
    private bool _hasPawn;

    void Awake()
    {
        ViewModel.AssertIfNull(nameof(ViewModel));
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        ViewModel.HasPawn += OnHasPawnChanged;
    }

    private void OnHasPawnChanged(bool hasPawn)
    {
        _hasPawn = hasPawn;
        if (_hasPawn)
        {
            //появился перс
        }
        else
        {
            Uins.Sound.SoundControl.Instance.OnTeleport();
            //пропал перс
        }
    }

    void OnDestroy()
    {
        ViewModel.AssertIfNull(nameof(ViewModel));
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    //#Old:
    //#Danger: it's concise code, but, 'cos of reflection using it harder in terms of allocations
    // private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
    // {
    //     if ((sender != null) && (args != null) && !string.IsNullOrEmpty(args.PropertyName))
    //     {
    //         var property = sender.GetType().GetProperty(args.PropertyName);
    //         if (property != null)
    //         {
    //             var value = property.GetValue(sender);
    //             if ((value != null) && (value is float))
    //             {
    //                 Uins.Sound.SoundControl.UpdateStat(args.PropertyName, (float)(value));
    //             }
    //         }
    //     }
    // }
    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
    {

        switch (args.PropertyName)
        {
            case nameof(PlayerMainStatsViewModel.HealthCurrentRatioClamped):
                Uins.Sound.SoundControl.UpdateStat(Uins.Sound.SoundControl.HealthCurrentStatID, ViewModel.HealthCurrentRatioClamped);
                break;
            case nameof(PlayerMainStatsViewModel.StaminaCurrentRatioClamped):
                Uins.Sound.SoundControl.UpdateStat((Uins.Sound.SoundControl.StaminaCurrentStatID), ViewModel.StaminaCurrentRatioClamped);
                break;
            case nameof(PlayerMainStatsViewModel.SatietyRatioClamped):
                Uins.Sound.SoundControl.UpdateStat(Uins.Sound.SoundControl.SatietyCurrentStatID, ViewModel.SatietyRatioClamped);
                break;
            case nameof(PlayerMainStatsViewModel.WaterBalanceRatioClamped):
                Uins.Sound.SoundControl.UpdateStat(Uins.Sound.SoundControl.WaterBalanceCurrentStatID, ViewModel.WaterBalanceRatioClamped);
                break;
            case nameof(PlayerMainStatsViewModel.IntoxicationRatio):
                Uins.Sound.SoundControl.UpdateStat(Uins.Sound.SoundControl.IntoxicationStatID, ViewModel.IntoxicationRatio);
                break;
            case nameof(PlayerMainStatsViewModel.OverheatRatio):
                Uins.Sound.SoundControl.UpdateStat(Uins.Sound.SoundControl.OverheatStatID, ViewModel.OverheatRatio);
                break;
            case nameof(PlayerMainStatsViewModel.HypothermiaRatio):
                Uins.Sound.SoundControl.UpdateStat(Uins.Sound.SoundControl.HypothermiaStatID, ViewModel.HypothermiaRatio);
                break;
            case nameof(PlayerMainStatsViewModel.EnvTemperature):
                Uins.Sound.SoundControl.UpdateStat(Uins.Sound.SoundControl.EnvTemperatureStatID, ViewModel.EnvTemperature);
                break;
            case nameof(PlayerMainStatsViewModel.EnvToxicLevel):
                Uins.Sound.SoundControl.UpdateStat(Uins.Sound.SoundControl.EnvToxicStatID, ViewModel.EnvToxicLevel);
                break;
            // no default - it's intended
        }
    }
}