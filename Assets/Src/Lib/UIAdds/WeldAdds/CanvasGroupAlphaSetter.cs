using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Костыль для Weld, который не умеет напрямую работать с CanvasGroup
/// </summary>
public class CanvasGroupAlphaSetter : MonoBehaviour
{
    [SerializeField, UsedImplicitly]
    private CanvasGroup _canvasGroup;

    private bool _isAfterAwake;


    private float _amount;

    public float Amount
    {
        get => _amount;
        set
        {
            if (!Mathf.Approximately(_amount, value))
            {
                _amount = value;
                if (_isAfterAwake)
                    SyncIfWoken();
            }
        }
    }

    private void Awake()
    {
        if (_canvasGroup.AssertIfNull(nameof(_canvasGroup)))
            return;

        _isAfterAwake = true;
        SyncIfWoken();
    }


    //=== Private =============================================================

    private void SyncIfWoken()
    {
        _canvasGroup.alpha = Amount;
    }
}