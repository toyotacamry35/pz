using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.Aspects;
using Assets.Src.Inventory;
using Assets.Src.ContainerApis;
using UnityEngine;
using OutlineEffect;
using Assets.ColonyShared.SharedCode.Aspects.Item;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using NLog;
using SharedCode.Aspects.Item.Templates;
using Assets.Src.Character.Events;
using Assets.Src.Doll;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using Core.Environment.Logging.Extension;
using ReactivePropsNs;

// It's comp. of view bone. & view should be created on `GotClient` & be destroyed on `LostCl` (see `CharClView`: `.InitClientU` / `.CleanClient`).
public class VisualSlot : MonoBehaviour
{
    protected static readonly NLog.Logger Logger = LogManager.GetLogger("VisualSlot");

    public SlotDefRef SlotDefRef;
    public Transform ActiveSlotInUseTransform;
    private ISubjectWithDollView _view;
    private (BaseItemResource, Guid) _item;
    private EntityApiWrapper<HasDollBroadcastApi> _hasDollBroadcastApiWrapper;
    private readonly List<GameObject> _reparentedClothObjects = new List<GameObject>();
    private List<object> _slotHideCausers; 


    //=== Props ===============================================================

    private VisualDoll Doll => _view?.Doll;

    public GameObject AttachedObj { get; private set; }

    public event Action<VisualSlot,GameObject> AttachedObjChanged;
    
    public bool IsInUse { get; private set; }

    public ResourceIDFull SlotResourceIDFull => GameResourcesHolder.Instance.GetID(SlotDefRef.Target);

    
    //=== Unity ===============================================================

    //Don't move it to `Awake`, 'cos it causes access to not fully constructed object (awake is not finised)
    //(see `SlotListenersCollection.OnSubscribeRequest`)
    private void Start()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }


    //=== Protected ===========================================================

    public void Subscribe()
    {
        if (SlotDefRef.Target.AssertIfNull(nameof(SlotDefRef), gameObject))
            return;

        _view = transform.root.GetComponentInChildren<ISubjectWithDollView>();
        if (_view.AssertIfNull(nameof(_view), gameObject) ||
            Doll.AssertIfNull(nameof(Doll), gameObject))
            return;

        if (Logger.IsDebugEnabled) 
            Logger.IfDebug()?.Message($"_doll.InstID:{Doll.GetInstanceID()}").Write();

        var ego = GetComponentInParent<EntityGameObject>();
        if (ego.AssertIfNull(nameof(ego), gameObject))
            return;

        _hasDollBroadcastApiWrapper = EntityApi.GetWrapper<HasDollBroadcastApi>(ego.OuterRef);

        _hasDollBroadcastApiWrapper.EntityApi.SubscribeToSlot(SlotDefRef.Target.SlotId, OnSlotItemChanged);
        _hasDollBroadcastApiWrapper.EntityApi.SubscribeToUsedSlotsChanged(OnUsedSlotsChanged);
    }

    public void Unsubscribe()
    {
        if (_hasDollBroadcastApiWrapper == null)
            return;

        _hasDollBroadcastApiWrapper.EntityApi.UnsubscribeFromSlot(SlotDefRef.Target.SlotId, OnSlotItemChanged);
        _hasDollBroadcastApiWrapper.EntityApi.UnsubscribeFromUsedSlotsChanged(OnUsedSlotsChanged);

        _hasDollBroadcastApiWrapper.Dispose();
        _hasDollBroadcastApiWrapper = null;
    }

    public void Hide(object causer)
    {
        if (causer == null) throw new ArgumentNullException(nameof(causer));

        _slotHideCausers = _slotHideCausers ?? new List<object>();
        bool hide = _slotHideCausers.Count == 0;
        if (!_slotHideCausers.Contains(causer))
            _slotHideCausers.Add(causer);
        if (hide && AttachedObj)
            AttachedObj.SetActive(false);
    }
    
    public void Unhide(object causer)
    {
        if (causer == null) throw new ArgumentNullException(nameof(causer));
        if (_slotHideCausers != null && _slotHideCausers.Remove(causer) && _slotHideCausers.Count == 0 && AttachedObj)
            AttachedObj.SetActive(true);
    }

    public bool IsHidden => _slotHideCausers?.Count > 0;
    
    //=== Private =============================================================

    private void OnSlotItemChanged(int slotIndex, SlotItem slotItem, int stackDelta)
    {
        var unequipedItemResource = _item;
        _item = (slotItem.ItemResource, slotItem.ItemGuid);

        if (unequipedItemResource.Item1 != null)
            UnequipVisual(unequipedItemResource);

        if (_item.Item1 != null)
            EquipVisual(_item);
        else
            IsInUse = false;
    }

    private void OnUsedSlotsChanged(IList<ResourceIDFull> newActiveSlotsIndices)
    {
        if (IsInUse)
        {
            if (!newActiveSlotsIndices.Contains(SlotResourceIDFull))
            {
                IsInUse = false;
                if (_item.Item1 != null)
                    EquipVisual(_item);
            }
        }
        else
        {
            if (newActiveSlotsIndices.Contains(SlotResourceIDFull))
            {
                IsInUse = true;
                if (_item.Item1 != null)
                    EquipVisual(_item);
            }
        }
    }

    private bool ShouldBeShownVisual(BaseItemResource itemResource)
    {
        if (itemResource.AssertIfNull(nameof(itemResource)))
            return false;

        if (GetVisual(itemResource) == null)
            return false;

        return !itemResource.ItemType.Target?.HideVisualAtSlot ?? true;
    }

    private GameObject GetVisual(BaseItemResource resource)
    {
        if (resource is ItemResource itemResource)
        {
            if (itemResource.Visuals != null && _view.DollDef != null && itemResource.Visuals.TryGetValue(_view.DollDef, out var v))
                return v.Target;
            if (itemResource.Visual != null)
                return itemResource.Visual.Target; 
            return itemResource.Visuals?.Values.FirstOrDefault()?.Target;
        }
        return null;
    }
    
    private void EquipVisual((BaseItemResource, Guid) item)
    {
        if (item.Item1.AssertIfNull(nameof(item)))
            return;

        //Is needed for logic on `IsInUse`(soon 'll be renamed to ~`IsInHand`) value changed (weapon moves from belt-slot into hand & back)
        UnequipVisual(item);

        if (!ShouldBeShownVisual(item.Item1))
            return;
        
        var itemResource = item.Item1 as ItemResource;
        
        var visual = GetVisual(itemResource);
        AttachedObj = Instantiate(visual, ActiveSlotInUseTransform != null && IsInUse ? ActiveSlotInUseTransform : transform);

        if (ActiveSlotInUseTransform != null)
        {
            GameObject pivot = AttachedObj.transform.Find(IsInUse ? "hand" : "spine")?.gameObject;
            if (pivot != null)
            {
                AttachedObj.transform.localPosition = pivot.transform.localPosition;
                AttachedObj.transform.localRotation = pivot.transform.localRotation;
            }

            if (IsInUse)
            {
                if (itemResource != null && itemResource.CorrectionInHandRequired)
                {
                    var correction = ActiveSlotInUseTransform.GetComponent<InHandPositionCorrection>();
                    if (correction != null)
                    {
                        AttachedObj.transform.localPosition += correction.Offset;
                        AttachedObj.transform.localRotation *= Quaternion.Euler(correction.Rotation);
                    }
                }

                ActiveSlotInUseTransform.GetComponent<AttackEventSubscriptionHandler>()?
                    .SetupColliders(AttachedObj.GetComponentsInChildren<HitColliderHandler>(), 
                        (item.Item1 as IHasStatsResource).SpecificStats.Target?.DamageType);
                _view?.OnPutItemInHand(item);
            }
            else
            {
                ActiveSlotInUseTransform.GetComponent<AttackEventSubscriptionHandler>()
                    ?.DisposeColliders(AttachedObj.GetComponentsInChildren<HitColliderHandler>());
                _view?.OnRemoveItemFromHand(item);
            }
        }

        //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"_doll?:{_doll != null}, .View?:{_doll?.View != null}, .HasAuthority?:{_doll?.View?.HasAuthority}").Write();

        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"_doll?:{Doll != null}, (_doll.InstID:{Doll?.GetInstanceID()})").Write();
        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"(continue)..  .View?:{_view != null}, ").Write();
        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"(continue)..  .HasAuthority?:{_view?.HasAuthority}").Write();

        if (_view != null && _view.HasAuthority)
        {
            foreach(var lodGroup in AttachedObj.GetComponentsInChildren<LODGroup>())
                lodGroup.ForceLOD(0);
            AddOutline(AttachedObj);
        }

        VisualSlotMesh[] visualSlotMesh = AttachedObj.GetComponentsInChildren<VisualSlotMesh>();
        if (visualSlotMesh != null)
        {
            for (int i = 0; i < visualSlotMesh.Length; i++)
            {
                Transform t = Doll.GetBoneByName(visualSlotMesh[i].boneName);
                if (t)
                {
                    visualSlotMesh[i].transform.parent = t;
                    visualSlotMesh[i].transform.localPosition = Vector3.zero;
                    visualSlotMesh[i].transform.localRotation = Quaternion.identity;
                }
                else
                    Logger.IfWarn()?.Message($"Unknown bone {visualSlotMesh[i].boneName} in VisualSlotMesh {visualSlotMesh[i].name}").Write();
            }
        }

        if (!ItemHelper.IsAChildOf(item.Item1.ItemType, Consts.BaseWeaponItemType))
        {
            var skinnedMeshes = AttachedObj.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshes != null)
            {
                foreach (var skinnedMeshRenderer in skinnedMeshes)
                    RetargedSkinnedMeshRenderer(skinnedMeshRenderer);

                EquipClothes();
            }
        }

        // Hide naked Body parts:
        if (itemResource?.HideBodyParts != null)
        {
            foreach (var hidePart in itemResource?.HideBodyParts)
            {
                if (Doll.BodyPartsWithHideState.TryGetValue(hidePart, out var partHideState))
                    partHideState.Add(gameObject);
            }
        }
        
        if (itemResource?.HideVisualSlots != null && Doll != null)
            foreach (var slotDef in itemResource?.HideVisualSlots)
                if (Doll.TryGetVisualSlot(slotDef, out var visualSlot))
                    visualSlot.Hide(gameObject);
        
        if (IsHidden)
            AttachedObj.SetActive(false);
        
        AttachedObjChanged?.Invoke(this, AttachedObj);
    }

    private void RetargedSkinnedMeshRenderer(SkinnedMeshRenderer skinnedRenderer)
    {
        var equipmentBones = skinnedRenderer.bones;
        Transform[] retargetedBones = new Transform[equipmentBones.Length];

        for (int i = 0; i < equipmentBones.Length; ++i)
        {
            var equipmentBone = equipmentBones[i];
            Transform dollBone = Doll.GetBoneByName(equipmentBone.gameObject.name);
            if (!dollBone)
                dollBone = Doll.GetBoneByName(equipmentBone.gameObject.name.TrimStart('_')); //TODO: в некоторых костюмах накосячили с неймингом, перевыгружать некогда было
            if (!dollBone)
            {
                Debug.LogWarning($"Can't find bone \"{equipmentBone.gameObject.name}\" in doll");
                if (!TrySetAbsentBoneByParentModelBone(equipmentBone, out dollBone))
                    // Exception case handling (plug, but can't do better):
                    dollBone = Doll.transform;
            }

            retargetedBones[i] = dollBone;
        }

        skinnedRenderer.bones = retargetedBones;
    }

    // E.g.: if jacket have wings & model doesn't 've appropr. bones, We'll bind them to spine model bone
    private bool TrySetAbsentBoneByParentModelBone(Transform equipmentBone, out Transform outBone)
    {
        var parentGo = equipmentBone.parent.gameObject;
        while (parentGo != null)
        {
            Transform dollParentBone = Doll.GetBoneByName(parentGo.name);
            if (dollParentBone)
            {
                outBone = dollParentBone;
                return true;
            }

            parentGo = parentGo.transform?.parent?.gameObject;
        }

        outBone = null;
        return false;
    }

    private void UnequipVisual((BaseItemResource, Guid) item)
    {
        var itemResource = item.Item1 as ItemResource;

        _view?.OnRemoveItemFromHand(item);

        if (itemResource?.HideVisualSlots != null && Doll != null)
            foreach (var slotDef in itemResource?.HideVisualSlots)
                if (Doll.TryGetVisualSlot(slotDef, out var visualSlot))
                    visualSlot.Unhide(gameObject);

        UnequipClothes();

        // Unhide naked Body parts:
        if (itemResource?.HideBodyParts != null)
        {
            try
            {
                foreach (var hidePart in itemResource.HideBodyParts)
                {
                    if (Doll.BodyPartsWithHideState.TryGetValue(hidePart /*.Target*/, out var partHideState))
                        partHideState.Remove(gameObject); // <- string #237
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message($"Got an exception: \"{e}\".  `{nameof(item)}` == {item.Item1}. this?:{this != null}").Write();
                //#causes Exception: if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"#Exception: .. g.o.?:{this?.gameObject != null}").Write();
            }
        }

        if (AttachedObj == null)
            return;

        if (ActiveSlotInUseTransform)
            ActiveSlotInUseTransform.GetComponent<AttackEventSubscriptionHandler>()
                ?.DisposeColliders(AttachedObj.GetComponentsInChildren<HitColliderHandler>());

        Destroy(AttachedObj);
        AttachedObj = null;
    }

    private void EquipClothes()
    {
        Cloth[] oldClothes = transform.GetComponentsInChildren<Cloth>(true);
        for (int i = 0; i < oldClothes.Length; i++)
            ReparentClothes(oldClothes[i]);
    }

    private void ReparentClothes(Cloth cloth)
    {
        Transform clothTransform = cloth.transform;
        Vector3 localPos = clothTransform.localPosition;
        Quaternion localRot = clothTransform.localRotation;

        string boneName = clothTransform.GetComponent<SkinnedMeshRenderer>()?.rootBone?.name;
        if (string.IsNullOrEmpty(boneName))
        {
            Logger.IfWarn()?.Message($"Can't get cloth bone name (no SkinnedMeshRenderer?) | Cloth:{clothTransform.FullName(transform)} Entity:{Doll.transform.root}").Write();
            return;
        }

        var newParent = Doll.GetBoneByName(boneName);

        if (newParent)
        {
            clothTransform.parent = newParent;
            clothTransform.localPosition = localPos;
            clothTransform.localRotation = localRot;
            _reparentedClothObjects.Add(clothTransform.gameObject);

            foreach (var clothCollider in cloth.capsuleColliders)
            {
                var clothColliderTransform = clothCollider.transform;
                if (!clothColliderTransform.IsChildOf(transform))
                    continue;

                string clothColliderBoneName = clothColliderTransform.parent.name;
                Vector3 clothColliderLocalPos = clothColliderTransform.localPosition;
                Quaternion clothColliderLocalRot = clothColliderTransform.localRotation;
                Transform newColliderParent = Doll.GetBoneByName(clothColliderBoneName);
                if (newColliderParent)
                {
                    clothColliderTransform.parent = newColliderParent;
                    clothColliderTransform.localPosition = clothColliderLocalPos;
                    clothColliderTransform.localRotation = clothColliderLocalRot;
                    _reparentedClothObjects.Add(clothColliderTransform.gameObject);
                }
                else
                    Logger.IfWarn()?.Message($"Can't reparent cloth collider | Cloth:{clothTransform.FullName(transform)} Collider:{clothColliderTransform.FullName(transform)} Bone:{clothColliderBoneName} Entity:{Doll.transform.root}").Write();
            }
        }
        else
            Logger.IfWarn()?.Message($"Can't reparent cloth | Cloth:{clothTransform.FullName(transform)} Bone:{boneName} Entity:{Doll.transform.root}").Write();
    }

    private void AddOutline(GameObject attachedObj)
    {
        if (!Constants.WorldConstants.EnableOutline)
            return;
        
        Renderer[] renderers = attachedObj.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (i == 0)
            {
                Outline outlineEraser = renderers[i].gameObject.AddComponent<Outline>();
                outlineEraser.eraseRenderer = true;
                outlineEraser.color = OutlineColor.Erase;
            }
        }
    }

    private void UnequipClothes()
    {
        if (Doll == null)
            return;

        foreach (var reparentedClothObject in _reparentedClothObjects)
            if (reparentedClothObject)
                Destroy(reparentedClothObject);
        _reparentedClothObjects.Clear();
    }
}