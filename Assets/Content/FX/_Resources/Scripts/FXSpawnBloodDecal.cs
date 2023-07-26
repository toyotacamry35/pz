using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeEyedGames {
	public class FXSpawnBloodDecal : MonoBehaviour {

		private ParticleSystem bloodSplash;    		
		public Decal bloodDecal;		
		public int maxNumOfDecals;
		List<ParticleCollisionEvent> collisionEvents;
		[HideInInspector]
		public int decalIndex;

		void Start () 
		{
			bloodSplash = GetComponent<ParticleSystem>();
			collisionEvents = new List<ParticleCollisionEvent> ();
			decalIndex = 0; //resets on effect spawn in FXPlayEffect script
		}

		void OnParticleCollision(GameObject other)
		{			
			int numCollisionEvents = ParticlePhysicsExtensions.GetCollisionEvents (bloodSplash, other, collisionEvents);		
			
			if (decalIndex < maxNumOfDecals)
			{
				Debug.Log("decalspawner");			
				var location = collisionEvents[0].intersection;
				location.y -= 0.5f;
				var rotation = Quaternion.identity; //TODO - rotate according to collision normal
				rotation *= Quaternion.Euler(Vector3.up * Random.Range (0,360)); // random decal rotation
				Instantiate(bloodDecal, location, rotation); 
				decalIndex++;
			}
		}
		
		//TODO - destroy decal after fadeout animation		
	}
}