using System.Collections;
using UnityEngine;

public class GODestroyer : MonoBehaviour {

	public float LifeTime = 5.0f;
     
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(LifeTime);
		Destroy(gameObject);
	}
}
