using JetBrains.Annotations;
using L10n;
using Uins;
using UnityEngine;

public class NewPlaceInfo : MonoBehaviour
{
    public LocalizationKeysHolder LocalizationKeysHolder;

    /// <summary>
    /// Для тестов: true - сообщение по центру, false - сбоку
    /// </summary>
    public bool ShowAsFirstTimeExplored; //DEBUG

    private void Awake()
    {
        LocalizationKeysHolder.AssertIfNull(nameof(LocalizationKeysHolder), gameObject);
    }

    [UsedImplicitly]
    void OnTriggerEnter(Collider other)
    {
        var newPlacesNotifier = other.GetComponent<NewPlacesNotifier>();
        if (newPlacesNotifier == null)
            return;

        var regionNameLs = LocalizationKeysHolder != null && !LocalizationKeysHolder.Ls1.IsEmpty()
            ? LocalizationKeysHolder.Ls1
            : LsExtensions.EmptyWarning;

        CenterNotificationQueue.Instance.SendNotification(new NewRegionNotificationInfo(regionNameLs, ShowAsFirstTimeExplored));
        Destroy(gameObject);
    }
}