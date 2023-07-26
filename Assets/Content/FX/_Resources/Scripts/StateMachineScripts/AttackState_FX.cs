using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState_FX : StateMachineBehaviour
{

    //parent object of axe collider
    public GameObject weaponCollider;
    //collider script
    private AxeCollision_FX axeBox;
    private FeetCollision_FX feetBox;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        

        //permit sparks instantiation
        axeBox = weaponCollider.GetComponent<AxeCollision_FX>();
        feetBox = weaponCollider.GetComponent<FeetCollision_FX>();
        if (axeBox)
        {
            axeBox.isAttacking = true;
            axeBox.isSpawned = true;

        }
        if (feetBox)
        {
            feetBox.isAttacking = true;
            feetBox.isSpawned = true;
        }
         
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        //supress sparks instantiation 
        if (axeBox)
        {
            axeBox.isAttacking = false;
        }
        if (feetBox)
        {
            feetBox.isAttacking = false;
        }
  
    }
}
    
