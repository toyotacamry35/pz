using UnityEngine;
using Assets.Src.ResourceSystem;

namespace Uins
{
    [CreateAssetMenu(menuName = "AimInput/AimInputSettingsMapping")]
    public class AimInputSettingsMapping : ScriptableObject
    {
        public JdbMetadata[] InputActions; // InputActionDef
    }
}