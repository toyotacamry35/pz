using System;
using System.Collections;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.ResourceSystem;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;
using Transform = UnityEngine.Transform;

namespace Uins
{
    [Binding]
    public class AccountGuiWindowCtrl : BindingController<AccountGuiWindowVM>
    {        
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        [Binding, UsedImplicitly]  public string AccName                  { get; private set; }
        [Binding, UsedImplicitly]  public int Level                       { get; private set; }
        [Binding, UsedImplicitly]  public int TotalExp                    { get; private set; } 
        [Binding, UsedImplicitly]  public int NextLvlUpExpThreshold       { get; private set; } 
        [Binding, UsedImplicitly]  public int ExpGainedOnCurrLvl          { get; private set; }
        [Binding, UsedImplicitly]  public float CurrLvlExpProgressPercent { get; private set; }
        [Binding, UsedImplicitly]  public LocalizedString Title           { get; private set; }
        [Binding, UsedImplicitly]  public Sprite Stripe                   { get; private set; }
        [Binding, UsedImplicitly]  public int GenderInt0Based             { get; private set; }

        [Binding, UsedImplicitly] public bool IsAnySelected                   { get; private set; }
        [Binding, UsedImplicitly] public bool ShowMsgChangeGenderIsForbidden  { get; private set; }
        [Binding, UsedImplicitly] public bool ShowCharPhotoWithFullAlpha    { get; private set; }


        [Binding, UsedImplicitly] public LocalizedString SelectedAchvmntName  { get; private set; }
        [Binding, UsedImplicitly] public LocalizedString SelectedAchvmntDescr { get; private set; }
        [Binding, UsedImplicitly] public LocalizedString SelectedTitle        { get; private set; }
        [Binding, UsedImplicitly] public Sprite SelectedStripe                { get; private set; }
        [Binding, UsedImplicitly] public Sprite SelectedStripeBig             { get; private set; }
        [Binding, UsedImplicitly] public bool SelectedStripeBigNotNull        { get; private set; }
        [Binding, UsedImplicitly] public bool HasActiveSession                { get; private set; }
        [Binding, UsedImplicitly] public bool HasAchievedPacksToConsume       { get; private set; }


        [SerializeField, UsedImplicitly] private Transform _rewardPacksListParentTransform;
        [SerializeField, UsedImplicitly] private AccountLevelRewardsPackCtrl _rewardPackPrefab;
        [SerializeField, UsedImplicitly] private HorizontalScrollSnap _scrollSnap;
        [SerializeField, UsedImplicitly] private GenderDefRef _genderMale;
        [SerializeField, UsedImplicitly] private GenderDefRef _genderFemale;

        // --- Privates: ------------------------------------------

        [UsedImplicitly]
        private void Awake()
        {
            _rewardPacksListParentTransform.AssertIfNull(nameof(_rewardPacksListParentTransform));
            _rewardPackPrefab.AssertIfNull(nameof(_rewardPackPrefab));
            _scrollSnap      .AssertIfNull(nameof(_scrollSnap));

            var accNameStream  = Vmodel.SubStream(D, vm => vm.AccName);
            var selectedStream = Vmodel.SubStream(D, vm => vm.SelectedRewardsPack);
            var isAnySelectedStream = selectedStream.Func(D, vm => vm != null);

            var selectedAchvmntName      = selectedStream.Func(D, vm => vm?.Def?.Value?.Achievement.Target?.Name ?? LsExtensions.Empty);
            var selectedAchvmntDescr     = selectedStream.Func(D, vm => vm?.Def?.Value?.Achievement.Target?.Description ?? LsExtensions.Empty);
            var selectedTitle            = selectedStream.Func(D, vm => vm?.Def?.Value?.Title.Target?.Name ?? LsExtensions.Empty);
            var selectedStripe           = selectedStream.Func(D, vm => vm?.Def?.Value?.Title.Target?.StripeIcon?.Target);
            var selectedStripeBig        = selectedStream.Func(D, vm => vm?.Def?.Value?.Title.Target?.StripeIconBig?.Target);
            var selectedStripeBigNotNull = selectedStripeBig.Func(D, sprite => sprite != null);

            var levelStream        = Vmodel.SubStream(D, vm => vm.Level);
            var totalExpStream     = Vmodel.SubStream(D, vm => vm.TotalExp);
            var hasAchievedPacksToConsumeStrm = Vmodel.SubStream(D, vm => vm.HasAchievedPacksToConsume);
            var nxtLvlUpExpThrStrm = Vmodel.SubStream(D, vm => vm.DeltaExpNeededToGetNextLevel);
            var expGainedOnCurrLvlStrm = Vmodel.SubStream(D, vm => vm.ExpGainedOnCurrLvl);
            var expProgressClampedStrm = Vmodel.SubStream(D, vm => vm.CurrLvlExpProgressPercent.Func(D, SharedHelpers.Clamp01));
            var titleStream        = Vmodel.SubStream(D, vm => vm.Title);
            var stripeStream       = Vmodel.SubStream(D, vm => vm.Stripe).Func(D, x => x?.Target);
            var genderIntStream    = Vmodel.SubStream(D, vm => vm.GenderIntStream0Based);
            var hasActiveSessionStream = Vmodel.SubStream(D, vm => vm.HasActiveSession);
            var needRecalcScrollStream = Vmodel.SubStream(D, vm => vm.NeedRecalcScroll, false, true);
            needRecalcScrollStream.Action(D, needRecalc =>
                {
                    if (!needRecalc)
                        return;
                    RequestRecalc();
                }
            );

            var showMsgChangeGenderIsForbiddenStream = isAnySelectedStream
                .Zip(D, hasActiveSessionStream)
                .Func(D, (isAnySelected, hasActiveSession) => !isAnySelected && hasActiveSession);
            var showCharPhotoWithFullAlphaStream = isAnySelectedStream
                .Zip(D, hasActiveSessionStream)
                .Func(D, (isAnySelected, hasActiveSession) => !isAnySelected && !hasActiveSession);

            // Просто перекладывалка значений из VM в prop монобеха д/доступности Weld'у.
            Bind(accNameStream,   () => AccName);
            Bind(levelStream,     () => Level);
            Bind(totalExpStream,  () => TotalExp);
            Bind(hasAchievedPacksToConsumeStrm, () => HasAchievedPacksToConsume);
            Bind(nxtLvlUpExpThrStrm, () => NextLvlUpExpThreshold);
            Bind(expGainedOnCurrLvlStrm, () => ExpGainedOnCurrLvl);
            Bind(expProgressClampedStrm,  () => CurrLvlExpProgressPercent);
            Bind(titleStream,     () => Title);
            Bind(stripeStream,    () => Stripe);
            Bind(genderIntStream, () => GenderInt0Based);

            Bind(isAnySelectedStream,  () => IsAnySelected);
            Bind(showMsgChangeGenderIsForbiddenStream, () => ShowMsgChangeGenderIsForbidden);
            Bind(showCharPhotoWithFullAlphaStream,  () => ShowCharPhotoWithFullAlpha);
            Bind(selectedAchvmntName,      () => SelectedAchvmntName);
            Bind(selectedAchvmntDescr,     () => SelectedAchvmntDescr);
            Bind(selectedTitle,            () => SelectedTitle);
            Bind(selectedStripe,           () => SelectedStripe);
            Bind(selectedStripeBig,        () => SelectedStripeBig);
            Bind(selectedStripeBigNotNull, () => SelectedStripeBigNotNull);

            Bind(hasActiveSessionStream, () => HasActiveSession);

            _rewardPacksListParentTransform.DestroyAllChildren(); //Clean dbg content   
            var pool = new BindingControllersPool<AccountLevelRewardsPackVM>(_rewardPacksListParentTransform, _rewardPackPrefab);
            var subListStream = Vmodel.SubListStream(D, vm => vm.LevelAchievementsList);
            pool.Connect(subListStream);
        }

        private bool _isRequestRecalcCoroutineWorking;
        private void RequestRecalc()
        {
            if (!_isRequestRecalcCoroutineWorking)
                StartCoroutine(RequestRecalcCoroutine());
            else
            {
                // do nothing - coroutine is already works & 'll proceed all new requests made while flag was true.
            }
        }
        // All this stuff with coroutine instead of just simple call _scrollSnap.Recalc inplace is needed to cut the loop of assignment RP in callstack of callback about its change.
        private IEnumerator RequestRecalcCoroutine()
        {
            Debug.Assert(!_isRequestRecalcCoroutineWorking);
            _isRequestRecalcCoroutineWorking = true;

            yield return null; //wait 1 frame to cut the loop of assignment RP in callstack of callback about its change.

            // if (DbgLog.Enabled) DbgLog.Log($"RequestRecalcCoroutine.");

            _scrollSnap.Recalc();
            _isRequestRecalcCoroutineWorking = false;
        }

        [UsedImplicitly]
        public void OnClickPlayButton()
        {
            try
            {
                if (HasAchievedPacksToConsume)
                    Vmodel.Value.ConsumeAllAchievedRewardsPacks();
                else
                    Vmodel.Value.OnClickPlayButton?.Invoke();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message("Error Back Button").Exception(e).Write();
            }
        }
        
        [UsedImplicitly]
        public void OnBackButton()
        {
            try
            {
                Vmodel.Value?.StartGameNode.ExitToLobby();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message("Error Back Button").Exception(e).Write();
            }
        }

        [UsedImplicitly]
        public void ChoseMaleGender() //OnClick
        {
            if (Vmodel.HasValue)
                Vmodel.Value?.ChoseGender(_genderMale.Target);
        }
        [UsedImplicitly]
        public void ChoseFemaleGender() //OnClick
        {
            if (Vmodel.HasValue)
                Vmodel.Value?.ChoseGender(_genderFemale.Target);
        }

    }
}
