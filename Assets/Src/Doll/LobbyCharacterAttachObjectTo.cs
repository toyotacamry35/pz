using UnityEngine;

namespace Assets.Src.Doll
{
    public class LobbyCharacterAttachObjectTo : MonoBehaviour
    {
        [SerializeField] private Transform _slot;
        [SerializeField] private bool _resetPosition;
        
        void Start()
        {
            transform.parent = _slot;
            if (_resetPosition)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}