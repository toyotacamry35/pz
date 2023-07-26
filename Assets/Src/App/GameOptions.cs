using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/GameOptions")]
public class GameOptions : ScriptableObject
{
    [SerializeField, UsedImplicitly]
    private bool _showWatermarks;

    public bool ShowWatermarks => _showWatermarks;
}