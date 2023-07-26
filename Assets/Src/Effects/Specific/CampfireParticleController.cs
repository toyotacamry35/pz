using Assets.Src.Aspects;
using System.Threading.Tasks;
using UnityEngine;

public class CampfireParticleController : IsActiveEntityListener
{
    public GameObject SoundEffect = null;
    public GameObject StartSequence = null;
    public GameObject LoopSequence = null;
    public GameObject EndSequence = null;

    private ParticleSystem[] loopers = null;

    protected override bool CanBeOnChildObject => true;
    
    protected override void GotClient()
    {
       //  Logger.IfInfo()?.Message("Awake").Write();;
        base.GotClient();

        if (StartSequence)
            StartSequence.SetActive(false);
        if (LoopSequence)
            LoopSequence.SetActive(false);
        if (EndSequence)
            EndSequence.SetActive(false);

        // get all emitters in main fire gameobject and re-enable looping
        if (LoopSequence)
        {
            loopers = LoopSequence.GetComponentsInChildren<ParticleSystem>();
            if (loopers != null)
            {
                foreach (var pSystem in loopers)
                {
                    var main = pSystem.main;
                    main.loop = true;
                }
            }
        }
    }

    protected override async Task SetActive(bool isActive, bool isClient)
    {
        //Logger.IfInfo()?.Message($"isActive = {isActive}, isClient = {isClient}").Write();
        await UnityQueueHelper.RunInUnityThread(() => {
            //if (isClient)
            {
                //Logger.IfWarn()?.Message($"SoundEffect.activeInHierarchy = {SoundEffect.activeInHierarchy}, isActive = {isActive}").Write();
                if (SoundEffect.activeInHierarchy != isActive)
                {
                    SoundEffect.SetActive(isActive);
                }
                    

                if (isActive)
                {
                    if (StartSequence)
                        StartSequence.SetActive(true);
                    if (LoopSequence)
                        LoopSequence.SetActive(true);
                    if (EndSequence)
                        EndSequence.SetActive(false);

                    if (loopers != null)
                    {
                        foreach (var pSystem in loopers)
                        {
                            var main = pSystem.main;
                            main.loop = true;
                        }
                    }
                }
                else
                {
                    if (EndSequence && ((StartSequence && StartSequence.activeInHierarchy) || (LoopSequence && LoopSequence.activeInHierarchy)))
                        EndSequence.SetActive(true);
                    if (StartSequence)
                        StartSequence.SetActive(false);
                    if (LoopSequence)
                        LoopSequence.SetActive(false);

                    if (loopers != null)
                    {
                        foreach (var pSystem in loopers)
                        {
                            var main = pSystem.main;
                            main.loop = false;
                        }
                    }
                }
            }
        });
    }
}
