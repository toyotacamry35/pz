using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquiperArt : MonoBehaviour {

    public Assets.Src.Aspects.VisualDoll _doll;
	// Use this for initialization
	void Start ()
    {

        var skinnedMeshes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshes != null)
        {
            foreach (var skinnedMeshRenderer in skinnedMeshes)
                RetargedSkinnedMeshRenderer(skinnedMeshRenderer);
        }
    }

    private void RetargedSkinnedMeshRenderer(SkinnedMeshRenderer skinnedRenderer)
    {
        var equipmentBones = skinnedRenderer.bones;
        Transform[] retargetedBones = new Transform[equipmentBones.Length];

        for (int i = 0; i < equipmentBones.Length; ++i)
        {
            var equipmentBone = equipmentBones[i];
            Transform dollBone = _doll.GetBoneByName(equipmentBone.gameObject.name);
            if (!dollBone)
            {
                Debug.LogWarning($"Can't find bone \"{equipmentBone.gameObject.name}\" in doll");
                if (!TrySetAbsentBoneByParentModelBone(equipmentBone, out dollBone))
                    // Exception case handling (plug, but can't do better):
                    dollBone = _doll.transform;
            }

            retargetedBones[i] = dollBone;
        }

        skinnedRenderer.bones = retargetedBones;
    }

    private bool TrySetAbsentBoneByParentModelBone(Transform equipmentBone, out Transform outBone)
    {
        var parentGo = equipmentBone.parent.gameObject;
        while (parentGo != null)
        {
            Transform dollParentBone = _doll.GetBoneByName(parentGo.name);
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

}
