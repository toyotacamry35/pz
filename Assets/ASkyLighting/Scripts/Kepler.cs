using UnityEngine;
using System;
using System.Collections;

namespace CelestialMechanics {
	public static class Kepler
    {
		//static int maxIterations = 20;
		static double tolerance = 0.0001;

		public static Quaternion ComputeOrientation(float o, float w, float i) {
			return Quaternion.Euler(i, -w, 0f)*Quaternion.Euler(0f, -o, 0f);
		}

		public static double ComputeSemiLatusRectum(double p, double e) {
			//if (e == 0.0) 		
				return p;
			//else
			//	return p*(1.0-e*e)/(1.0-e);

		}

		public static double ComputeSemiMajorAxis(double p, double e)
        {
			//if (e == 0.0) 			
				return p;
			// else
			//	return p/(1-e);
		}

		public static double ComputeRate(double T, double from, double to) {
			if (T == 0) {
				Debug.LogWarning("Period cannot be 0");
				return 0;
			}
			return Math.Abs((from-to)/T);
		}


		public static double ComputeEccentricAnomaly(double M, double e)
        {
			//if (e == 0.0)
			    return M;
            //else
            //    return NewtonElliptical(M, e);
        }
		public static double ComputeTrueAnomaly(double E, double e)
        {
			//if (e == 0.0)
				return E;
			// else
			//	return 2.0*Math.Atan2(Math.Sqrt(1.0+e)*Math.Sin(E/2.0), Math.Sqrt(1.0-e)*Math.Cos(E/2.0));
		}

		public static double ComputeRadius(double l, double e, double v)
        {
			return l/(1.0 + e*Math.Cos(v));
		}
		public static Vector3 ComputePosition(double r, double v)
        {
			return new Vector3((float)(r*Math.Cos(v)),
			                   0f,
			                   (float)(r*Math.Sin(v)));
		}

		public static Vector3 ComputeVelocity(double a, double r, double n, double E, double v, double e) {
			//if (e == 0.0)
            {		
				Vector3 vel = new Vector3((float)(-Math.Sin(E)),
				                          0f,
				                          (float)(Math.Cos(E)));
				return (float)(a * n) * vel;

			}
            /*
                else
            {	
				Vector3 vel = new Vector3((float)(-Math.Sin(E)),
			    	                      0f,
			        	                  (float)(Math.Sqrt(1-e*e)*Math.Cos(E)));
				return (float)((a*a*n)/r) * vel;

			} 
            */
		}

		public static Quaternion ComputeAxis(float alpha, float delta) {
			return Quaternion.Euler(delta, alpha, 0f);
		}

		public static Quaternion ComputeRotation(Quaternion axis, double angle)
        {
			return axis * Quaternion.Euler(0f, -(float)angle, 0f);
		}

        /*
		static double NewtonElliptical(double M, double e) {
			double E0 = M;
			double E = 0;

			for (int i = 0; i < maxIterations; i++)
            {
				E = E0 - (E0 - e*Math.Sin(E) - M)/(1.0 - e*Math.Cos(E));

				if (Math.Abs(E - e*Math.Sin(E) - M) < tolerance)
					return E;
				else
					E0 = E;
			}
            return 0;
        }
        */
	}
}