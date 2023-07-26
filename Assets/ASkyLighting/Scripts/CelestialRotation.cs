using UnityEngine;
using System;
using System.Collections;
using TOD;

namespace CelestialMechanics {
	public class CelestialRotation : MonoBehaviour {
		const double Deg2Rad = Math.PI/180.0;

		#region Fields
		[SerializeField] float _rightAscension = 0.0f; //[deg]
		public float rightAscension {
			get {return _rightAscension;}
			set {
				_rightAscension = value;
				axis = Kepler.ComputeAxis(_rightAscension, _declination);
			}
		}

		[SerializeField] float _declination = 0.0f; //[deg]
		public float declination {
			get {return _declination;}
			set {
				_declination = value;
				axis = Kepler.ComputeAxis(_rightAscension, _declination);
			}
		}

        public float direction = 1;
		public float meanAngle = 0.0f; //[deg]

		[SerializeField] double _period = 10.0; //[s]
		public double period {
			get {return _period;}
			set {
				_period = value;
				rate = Kepler.ComputeRate(_period, -Math.PI, Math.PI);
			}
		}

		public double timeScale = 1.0;

		public double startEpoch = 0.0; //[s]
		#endregion

		#region Properties
		//static properties
		public Quaternion axis {get; private set;}
		public double rate {get; private set;} //[rad/s]

		//dynamic properties
		public double angle {get; private set;} //[rad]
		public Quaternion rotation {get; private set;}
        public float CG;
        #endregion

        
        void OnEnable() {
			ComputeStaticProperties();
			ComputeDynamicProperties(angle);
			transform.localRotation = rotation;
		}

		void OnValidate() {
			OnEnable();
		}
        
		public void ComputeStaticProperties() {
			axis = Kepler.ComputeAxis(_rightAscension, _declination);
			rate = Kepler.ComputeRate(_period, -Math.PI, Math.PI);
		}

		public void ComputeDynamicProperties(double angle) {
			rotation = Kepler.ComputeRotation(axis, angle/Deg2Rad);
		}

        public void UpdateSimulation()
        {
            angle = (startEpoch + CG * direction) * 2 * Math.PI;
			ComputeDynamicProperties(angle);
			transform.localRotation = rotation;
		}

	}
}