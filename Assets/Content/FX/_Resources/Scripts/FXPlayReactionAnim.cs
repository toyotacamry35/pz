using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPlayReactionAnim : MonoBehaviour {

	private Animator myAnimator;
	
	void Start () {
		myAnimator = GetComponent<Animator>();
	}	
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			myAnimator.SetTrigger("hit");
		}		
	}
}
