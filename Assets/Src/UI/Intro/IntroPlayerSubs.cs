using JetBrains.Annotations;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins.Intro
{
    [Binding]
    public class IntroPlayerSubs : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private IntroPlayer _introPlayer;

        [Binding]
        public bool IsVisible { get; private set; }

        [Binding]
        public bool CanSkipVideo { get; private set; }

        private void Awake()
        {
            if (_introPlayer.AssertIfNull(nameof(_introPlayer)))
                return;

            Bind(_introPlayer.IsIntroPlayingRp, () => IsVisible);
            Bind(_introPlayer.CanSkipVideoRp, () => CanSkipVideo);
        }
    }
}