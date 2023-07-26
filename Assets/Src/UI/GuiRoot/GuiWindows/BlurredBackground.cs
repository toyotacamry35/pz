using System.Collections.Generic;
using System.Linq;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Uins.GuiWindows
{
    public class BlurredBackground : MonoBehaviour
    {
        public static BlurredBackground Instance;

        private readonly List<FullRequest> _fullBlurRequests = new List<FullRequest>();

        private readonly List<PartialRequest> _partialBlurRequests = new List<PartialRequest>();


        //=== Unity ===========================================================

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }


        //=== Public ==========================================================

        public void SwitchCameraFullBlur(MonoBehaviour requestOwner, bool isOn)
        {
            //UI.CallerLog($"isOn{isOn.AsSign()} owner={requestOwner}"); //2del
            if (requestOwner.AssertIfNull(nameof(requestOwner)))
                return;

            var request = _fullBlurRequests.FirstOrDefault(r => r.Owner == requestOwner);

            if (isOn)
            {
                if (request == null)
                    _fullBlurRequests.Add(new FullRequest(requestOwner));
                else
                    UI.Logger.IfError()?.Message($"{nameof(SwitchCameraFullBlur)}({requestOwner}, isOn+) {nameof(requestOwner)} is already in list").Write();
            }
            else
            {
                if (request != null)
                    _fullBlurRequests.Remove(request);
                else
                    UI.Logger.IfError()?.Message($"{nameof(SwitchCameraFullBlur)}({requestOwner}, isOn-) {nameof(requestOwner)} isn't in list").Write();
            }

            SwitchBlurWork();
        }

        public void SwitchCameraPartialBlur(MonoBehaviour requestOwner, bool isOn, Rect partialRect)
        {
            //UI.CallerLog($"isOn{isOn.AsSign()} owner={requestOwner}, rect={partialRect}"); //2del
            if (requestOwner.AssertIfNull(nameof(requestOwner)))
                return;

            var request = _partialBlurRequests.FirstOrDefault(r => r.Owner == requestOwner);

            if (isOn)
            {
                if (request == null)
                    _partialBlurRequests.Add(new PartialRequest(requestOwner, partialRect));
                else
                    UI.Logger.Error(
                        $"{nameof(SwitchCameraPartialBlur)}({requestOwner}, isOn+, {partialRect}) {nameof(requestOwner)} is already in list");
            }
            else
            {
                if (request != null)
                    _partialBlurRequests.Remove(request);
                else
                    UI.Logger.IfError()
                        ?.Message($"{nameof(SwitchCameraPartialBlur)}({requestOwner}, isOn-, {partialRect}) {nameof(requestOwner)} isn't in list")
                        .Write();
            }

            SwitchBlurWork();
        }


        //=== Private =========================================================

        private void SwitchBlurWork()
        {
            if (_fullBlurRequests.Count > 0)
            {
                PostProcessGlobalVolumeController.Instance.ActivateDof(true);
                PostProcessGlobalVolumeController.Instance.Recalculate();
            }
            else if (_partialBlurRequests.Count > 0)
            {
                PostProcessGlobalVolumeController.Instance.ActivateDof(true);
                PostProcessGlobalVolumeController.Instance.Recalculate(
                    _partialBlurRequests.Select(pr => pr.PartialRect).ToArray());
            }
            else
            {
                PostProcessGlobalVolumeController.Instance.ActivateDof(false);
            }
        }

        private class FullRequest
        {
            public MonoBehaviour Owner;

            public FullRequest(MonoBehaviour owner)
            {
                Owner = owner;
            }
        }

        private class PartialRequest : FullRequest
        {
            public Rect PartialRect;

            public PartialRequest(MonoBehaviour owner, Rect partialRect) : base(owner)
            {
                PartialRect = partialRect;
            }
        }
    }
}