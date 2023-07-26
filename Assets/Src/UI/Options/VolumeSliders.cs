using System;
using ReactivePropsNs;
using Uins.Sound;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class VolumeSliders : BindingViewModel
    {
        private TwoWayPropController<bool, VolumeSliders> _isMutedTwoWayPropController;

        [Binding]
        public bool IsMuted
        {
            get => _isMutedTwoWayPropController?.Value ?? false;
            set => _isMutedTwoWayPropController?.OnChange(value);
        }

        private TwoWayPropController<bool, VolumeSliders> _isFpsLimitedTwoWayPropController;

        [Binding]
        public bool IsFpsLimited
        {
            get => _isFpsLimitedTwoWayPropController?.Value ?? false;
            set => _isFpsLimitedTwoWayPropController?.OnChange(value);
        }

        private TwoWayPropController<float, VolumeSliders> _masterVolumeTwoWayPropController;

        [Binding]
        public float MasterVolume
        {
            get => _masterVolumeTwoWayPropController?.Value ?? 1;
            set => _masterVolumeTwoWayPropController?.OnChange(value);
        }

        private TwoWayPropController<float, VolumeSliders> _musicVolumeTwoWayPropController;

        [Binding]
        public float MusicVolume
        {
            get => _musicVolumeTwoWayPropController?.Value ?? 1;
            set => _musicVolumeTwoWayPropController?.OnChange(value);
        }

        private TwoWayPropController<float, VolumeSliders> _sfxVolumeTwoWayPropController;

        [Binding]
        public float SfxVolume
        {
            get => _sfxVolumeTwoWayPropController?.Value ?? 1;
            set => _sfxVolumeTwoWayPropController?.OnChange(value);
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (SoundControl.Instance.AssertIfNull($"{nameof(SoundControl)}.{nameof(SoundControl.Instance)}"))
                return;

            _isMutedTwoWayPropController = new TwoWayPropController<bool, VolumeSliders>(() => IsMuted, this);
            _isMutedTwoWayPropController.Connect(SoundControl.Instance.IsMutedRp, b => SoundControl.Instance.IsMutedRp.Value = b);

            _isFpsLimitedTwoWayPropController = new TwoWayPropController<bool, VolumeSliders>(() => IsFpsLimited, this);
            _isFpsLimitedTwoWayPropController.Connect(VideoSettings.Instance.IsFpsLimitedRp, b => VideoSettings.Instance.IsFpsLimitedRp.Value = b);

            _masterVolumeTwoWayPropController = new TwoWayPropController<float, VolumeSliders>(() => MasterVolume, this);
            _masterVolumeTwoWayPropController.Connect(
                SoundControl.Instance.MasterVolumeRp.Func(D, ToRatio),
                ratio => OnSomeSliderChanged(ratio, SoundControl.Instance.MasterVolumeRp));

            _musicVolumeTwoWayPropController = new TwoWayPropController<float, VolumeSliders>(() => MusicVolume, this);
            _musicVolumeTwoWayPropController.Connect(
                SoundControl.Instance.MusicVolumeRp.Func(D, ToRatio),
                ratio => OnSomeSliderChanged(ratio, SoundControl.Instance.MusicVolumeRp));

            _sfxVolumeTwoWayPropController = new TwoWayPropController<float, VolumeSliders>(() => SfxVolume, this);
            _sfxVolumeTwoWayPropController.Connect(
                SoundControl.Instance.SfxVolumeRp.Func(D, ToRatio),
                ratio => OnSomeSliderChanged(ratio, SoundControl.Instance.SfxVolumeRp));
        }


        //=== Private =========================================================

        private float ToRatio(float volume)
        {
            return volume / SoundControl.MaxVolume;
        }

        private void OnSomeSliderChanged(float ratio, ReactiveProperty<float> reactiveProperty)
        {
            var volume = ratio * SoundControl.MaxVolume;

            if (Math.Abs(reactiveProperty.Value - volume) > float.Epsilon)
                reactiveProperty.Value = volume;
        }
    }
}