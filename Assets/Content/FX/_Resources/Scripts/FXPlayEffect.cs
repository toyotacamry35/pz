using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeEyedGames {
	public class FXPlayEffect : MonoBehaviour {
		
		private ParticleSystem effect;
		private FXSpawnBloodDecal decalSpawner;
		
		void Start () {
			effect = GetComponent<ParticleSystem>();
			decalSpawner = GetComponentInChildren<FXSpawnBloodDecal>();
		}
		
		void Update () {
			if (Input.GetMouseButtonDown(0))
			{
				effect.Play(true);
				if (decalSpawner != null) {
					decalSpawner.decalIndex = 0;
				}
			}		
		}
	}
}
