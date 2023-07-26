using JetBrains.Annotations;
using Uins;
using UnityEngine;

public class NewPlacesNotifier : MonoBehaviour
{
    [UsedImplicitly]
    void OnTriggerEnter(Collider other)
    {
        //UI.CallerLog($"From {other} to {transform}"); //DEBUG
        //        var newPlaceInfo = other.GetComponent<NewPlaceInfo>();
        //        if (newPlaceInfo == null)
        //            return;
        //
        //        var regionNameLs = newPlaceInfo.LocalizationKeysHolder != null && !newPlaceInfo.LocalizationKeysHolder.Ls1.IsEmpty()
        //            ? newPlaceInfo.LocalizationKeysHolder.Ls1
        //            : LsExtensions.EmptyWarning;
        //
        //        CenterNotificationQueue.Instance.SendNotification(new NewRegionNotificationInfo(regionNameLs, newPlaceInfo.ShowAsFirstTimeExplored));
        //        Destroy(other.gameObject);
    }
}