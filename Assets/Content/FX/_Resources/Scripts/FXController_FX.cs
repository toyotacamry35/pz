using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PigeonCoopToolkit.Effects.Trails;

public class FXController_FX : MonoBehaviour {

    public ParticleSystem foot_effect;
    public Transform leftFoot;
    public Transform rightFoot;

    public GameObject decal;

    public GameObject weaponTrail;
    private SmoothTrail trail;

    private void Start()
    {
        trail = weaponTrail.GetComponent<SmoothTrail>();
    }

    void TrailOn()
    {
        trail.Emit = true;

    }

    void TrailOff()
    {
        trail.Emit = false;
    }
    
  
    void RightFootFX()
    {
        Instantiate(foot_effect, rightFoot.position, Quaternion.Euler(-90,0,0));
        //Instantiate(decal, rightFoot.position, Quaternion.identity);
        //Debug.Log("RIGHT");
    }

    void LeftFootFX()
    {
        Instantiate(foot_effect, leftFoot.position, Quaternion.Euler(-90, 0, 0));
        //Instantiate(decal, leftFoot.position, Quaternion.identity);
        //Debug.Log("LEFT");
    }

    private void Update()
    {
    }


}
