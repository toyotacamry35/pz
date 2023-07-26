using System.Linq;
using UnityEngine;

namespace Assets.Src.Doll
{
    public class LobbyCharacterSkinnedMeshRetargeter : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        
        void Awake()
        {
            var allBones = _root.GetComponentsInChildren<Transform>().ToDictionary(x => x.name, x => x);
            foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var newBones = skinnedMeshRenderer.bones
                    .Where(x => x)
                    .Select(oldBone => allBones.TryGetValue(oldBone.name, out var newBone) ? newBone : null)
                    .Where(x => x)
                    .ToArray();
                skinnedMeshRenderer.bones = newBones;
            }
        }
    }
}