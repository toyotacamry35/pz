using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class CharacterDissolveController : MonoBehaviour {

    public float cutoff;
    public bool play;

    public PlayableDirector timeline;

    public GameObject deathParticles;    
    public Transform spawnPoint;
    public Vector3 rot;

   //self-explanatory
    void PlayTimeline ()
    {
        timeline.Play();
    }

	void Update () {
        //global shader cutoff param
        Shader.SetGlobalFloat("_DissolveCutoff", cutoff);

        if (play)
        {            
            play = false;
            //launch timeline clip + spawn particles
            PlayTimeline();
            Instantiate(deathParticles, spawnPoint.position, this.transform.rotation);
        }
	}
}
