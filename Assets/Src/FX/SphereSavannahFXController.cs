using System;
using System.Collections;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.ColonyShared.GeneratedCode.Time;
using SharedCode.Aspects.Sessions;
using TOD;
using UnityEngine;

public class SphereSavannahFXController : MonoBehaviour
{
    [SerializeField]
    private bool controlByTime;

    [SerializeField]
    private Material sphereMaterial;

    //private RealmRulesDef _def;
    //private long _startTime;

    protected const float k_DayDuration = 24f;

    public static float StartupTime = InGameTime.StartupHours.Hours + InGameTime.StartupHours.Minutes / 60.0f + InGameTime.StartupHours.Seconds / 60.0f / 60.0f;
    public static float DurationTime = InGameTime.DayDuration.Days * 24 +InGameTime.DayDuration.Hours + InGameTime.DayDuration.Minutes / 60.0f + InGameTime.DayDuration.Seconds / 60.0f / 60.0f;

    private float GetNormalizedTime()
    {
        return ASkyLighting.CGTime;
    }

    private void Start()
    {
        /*AsyncUtils.RunAsyncTask(
            async () =>
            {
                await GetRealm();
                await UnityQueueHelper.RunInUnityThread(() => this.StartInstrumentedCoroutine(UpdateTime()));
            });*/
        // this.StartInstrumentedCoroutine(UpdateTime());
    }

    /*private async Task GetRealm()
    {
        OuterRef<IEntity> realmRef;
        using (var wrapper = await GameState.Instance.ClientClusterNode.Get<IAccountEntityClientFull>(GameState.Instance.AccountId))
        {
            var entity = wrapper?.Get<IAccountEntityClientFull>(GameState.Instance.AccountId);
            if (entity != null)
            {
                realmRef = entity.CharRealmData.CurrentRealm;
            }
            else
                throw new Exception(string.Format($"[{GetType()}] AccountEntity not found"));
        }

        if (!realmRef.Equals(OuterRef<IEntity>.Invalid))
        {
            long entityStartTime = 0;
            int defExistenceHours = -1;
            using (var wrapper = await GameState.Instance.ClientClusterNode.Get(realmRef.TypeId, realmRef.Guid))
            {
                var entity = wrapper?.Get<IRealmEntityClientFull>(realmRef.TypeId, realmRef.Guid, ReplicationLevel.ClientFull);
                if (entity != null)
                {
                    _startTime = entity.StartTime;
                    _def = entity.Def;
                }
                else
                {
                    throw new Exception("Realm does not exist on map!");
                }
            }
        }
    }*/

    IEnumerator UpdateTime()
    {
        if (controlByTime)
        {
            while (true)
            {
                sphereMaterial.SetFloat("_Opacity", GetNormalizedTime());
                yield return new WaitForSeconds(1);
            }
        }
    }
}