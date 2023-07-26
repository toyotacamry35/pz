using UnityEngine;
using System;
using System.Reflection;
using TOD;

namespace DeepSky.Haze
{
    [Serializable, AddComponentMenu("")]
    public class DS_HazeContextItem
    {

        #region FIELDS
		[SerializeField]
		public DayName m_DayName;
        [SerializeField]
        public float nightTimePercentBaked = 0.38f;

        [SerializeField] public FloatCurve airScattering; //0, 8.0f)
        [SerializeField] public FloatCurve airHeightFalloff; //(0.0001f, 0.1f)]

        [SerializeField] public FloatCurve hazeScattering; //(0.0001f, 0.1f)]
        [SerializeField] public FloatCurve hazeHeightFalloff; //(0.0001f, 0.1f)]
        [SerializeField] public FloatCurve hazeScatteringDir; //-0.99f, 0.99f)]
        [SerializeField] public FloatCurve hazeScatteringRatio; //(0, 1)]

        [SerializeField] public FloatCurve fogOpacity02; //(0, 1)]
        [SerializeField] public FloatCurve fogScattering;// (0, 8.0f)]
        [SerializeField] public FloatCurve fogExtinction; //Range(0, 8.0f
        
        [SerializeField] public FloatCurve fogHeightFalloff; //Range(0.0001f, 1.0f)]
        [SerializeField] public FloatCurve fogDistance; //0, 1)]
        [SerializeField] public FloatCurve fogHeight;//-200,200
        [SerializeField] public FloatCurve fogScatteringDir;//-90,90

        
        [SerializeField] public ColorGradient fogAmbient;
        [SerializeField] public ColorGradient fogLight;
        [SerializeField] public ColorGradient fogHorizon;

        [SerializeField] public ColorGradient fogColor;
        [SerializeField] public FloatCurve fogDensity; //Range(0.01f, 0.0001f)]
        [SerializeField] public FloatCurve fogAlpha; //0, 1f

        [SerializeField] public bool isCave = false;
        [SerializeField] public FloatCurve caveAmbient;
        #endregion


        public DS_HazeContextItem()
        {
            airScattering = new FloatCurve();
            airHeightFalloff = new FloatCurve();

            hazeHeightFalloff = new FloatCurve();
            hazeScattering = new FloatCurve();
            hazeScatteringDir = new FloatCurve();
            hazeScatteringRatio = new FloatCurve();

            fogOpacity02 = new FloatCurve();
            fogScattering = new FloatCurve();
            fogExtinction = new FloatCurve();

            fogHeightFalloff = new FloatCurve();
            fogDistance = new FloatCurve();
            fogHeight = new FloatCurve();
            fogScatteringDir = new FloatCurve();

            fogAmbient = new ColorGradient();
            fogLight = new ColorGradient();
            fogHorizon = new ColorGradient();

            fogColor = new ColorGradient();
            fogDensity = new FloatCurve();
            fogAlpha = new FloatCurve();

            caveAmbient = new FloatCurve();
        }

        
        public void UpdateCurvesFull()
        {
            float currentTime = ASkyLighting.CGTime;

            airScattering.Evaluate(currentTime);
            airHeightFalloff.Evaluate(currentTime);
            hazeScattering.Evaluate(currentTime);
            hazeHeightFalloff.Evaluate(currentTime);
            hazeScatteringDir.Evaluate(currentTime);
            hazeScatteringRatio.Evaluate(currentTime);
            fogOpacity02.Evaluate(currentTime);
            fogScattering.Evaluate(currentTime);
            fogExtinction.Evaluate(currentTime);
            fogHeightFalloff.Evaluate(currentTime);
            fogDistance.Evaluate(currentTime);
            fogScatteringDir.Evaluate(currentTime);
            fogHeight.Evaluate(currentTime);
            fogAmbient.Evaluate(currentTime);
            fogLight.Evaluate(currentTime);
            fogHorizon.Evaluate(currentTime);
            fogColor.Evaluate(currentTime);
            fogAlpha.Evaluate(currentTime);
            fogDensity.Evaluate(currentTime);
            caveAmbient.Evaluate(currentTime);

        }
        public void Lerp(DS_HazeContextItem other, float dt)
        {
            if (other == null)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogError("DeepSky::DS_HazeContextItem:Lerp - null context passed!");
#endif
                return;
            }

            dt = Mathf.Clamp01(dt);

            float dTinv = 1.0f - dt;
            
            airScattering.value = airScattering.value * dTinv + other.airScattering.value * dt;
            airHeightFalloff.value = airHeightFalloff.value * dTinv + other.airHeightFalloff.value * dt;
            hazeScattering.value = hazeScattering.value * dTinv + other.hazeScattering.value * dt;
            hazeHeightFalloff.value = hazeHeightFalloff.value * dTinv + other.hazeHeightFalloff.value * dt;
            hazeScatteringDir.value = hazeScatteringDir.value * dTinv + other.hazeScatteringDir.value * dt;
            hazeScatteringRatio.value = hazeScatteringRatio.value * dTinv + other.hazeScatteringRatio.value * dt;
            fogOpacity02.value = fogOpacity02.value * dTinv + other.fogOpacity02.value * dt;
            fogScattering.value = fogScattering.value * dTinv + other.fogScattering.value * dt;
            fogExtinction.value = fogExtinction.value * dTinv + other.fogExtinction.value * dt;
            fogHeightFalloff.value = fogHeightFalloff.value * dTinv + other.fogHeightFalloff.value * dt;
            fogDistance.value = fogDistance.value * dTinv + other.fogDistance.value * dt;
            fogScatteringDir.value = fogScatteringDir.value * dTinv + other.fogScatteringDir.value * dt;
            fogHeight.value = fogHeight.value * dTinv + other.fogHeight.value * dt;
            fogAmbient.color = fogAmbient.color * dTinv + other.fogAmbient.color * dt;
            fogLight.color = fogLight.color * dTinv + other.fogLight.color * dt;
            fogHorizon.color = fogHorizon.color * dTinv + other.fogHorizon.color * dt;
            fogColor.color = fogColor.color * dTinv + other.fogColor.color * dt;
            fogAlpha.value = fogAlpha.value * dTinv + other.fogAlpha.value * dt;
            fogDensity.value = fogDensity.value * dTinv + other.fogDensity.value * dt;
            caveAmbient.value = caveAmbient.value * dTinv + other.caveAmbient.value * dt;
            isCave = (other.isCave) ? true : isCave;
        }

        public HazeItemEclipse MergeWithEclipse(HazeItemEclipse nightItem)
        {
            HazeItemEclipse current = ASkyLighting._instance.hazeCore.current;
            current.airScattering = Mathf.Lerp(airScattering.value, nightItem.airScattering, ASkyLighting.eclipsePower);
            current.airHeightFalloff = Mathf.Lerp(airHeightFalloff.value, nightItem.airHeightFalloff, ASkyLighting.eclipsePower);
            current.hazeScattering = Mathf.Lerp(hazeScattering.value, nightItem.hazeScattering, ASkyLighting.eclipsePower);
            current.hazeHeightFalloff = Mathf.Lerp(hazeHeightFalloff.value, nightItem.hazeHeightFalloff, ASkyLighting.eclipsePower);
            current.hazeScatteringDir = Mathf.Lerp(hazeScatteringDir.value, nightItem.hazeScatteringDir, ASkyLighting.eclipsePower);
            current.hazeScatteringRatio = Mathf.Lerp(hazeScatteringRatio.value, nightItem.hazeScatteringRatio, ASkyLighting.eclipsePower);
            current.fogOpacity02 = Mathf.Lerp(fogOpacity02.value, nightItem.fogOpacity02, ASkyLighting.eclipsePower);
            current.fogScattering = Mathf.Lerp(fogScattering.value, nightItem.fogScattering, ASkyLighting.eclipsePower);
            current.fogExtinction = Mathf.Lerp(fogExtinction.value, nightItem.fogExtinction, ASkyLighting.eclipsePower);
            current.fogHeightFalloff = Mathf.Lerp(fogHeightFalloff.value, nightItem.fogHeightFalloff, ASkyLighting.eclipsePower);
            current.fogDistance = Mathf.Lerp(fogDistance.value, nightItem.fogDistance, ASkyLighting.eclipsePower);
            current.fogScatteringDir = Mathf.Lerp(fogScatteringDir.value, nightItem.fogScatteringDir, ASkyLighting.eclipsePower);
            current.fogHeight = Mathf.Lerp(fogHeight.value, nightItem.fogHeight, ASkyLighting.eclipsePower);
            current.fogAlpha = Mathf.Lerp(fogAlpha.value, nightItem.fogAlpha, ASkyLighting.eclipsePower);
            current.fogDensity = Mathf.Lerp(fogDensity.value, nightItem.fogDensity, ASkyLighting.eclipsePower);
            current.fogAmbient = Color.Lerp(fogAmbient.color, nightItem.fogAmbient, ASkyLighting.eclipsePower);
            current.fogLight = Color.Lerp(fogLight.color, nightItem.fogLight, ASkyLighting.eclipsePower);
            current.fogColor = Color.Lerp(fogColor.color, nightItem.fogColor, ASkyLighting.eclipsePower);
            current.fogHorizon = Color.Lerp(fogHorizon.color, nightItem.fogHorizon, ASkyLighting.eclipsePower);
            current.caveAmbient = Mathf.Lerp(caveAmbient.value, nightItem.caveAmbient, ASkyLighting.eclipsePower);
            return current;
        }
        /*
        public DS_HazeContextItem MergeWithEclipse(DS_HazeContextItem nightItem)
        {
            DS_HazeContextItem current = new DS_HazeContextItem();
            current.airScattering.value = Mathf.Lerp(airScattering.value, nightItem.airScattering.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.airHeightFalloff.value = Mathf.Lerp(airHeightFalloff.value, nightItem.airHeightFalloff.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.hazeScattering.value = Mathf.Lerp(hazeScattering.value, nightItem.hazeScattering.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.hazeHeightFalloff.value = Mathf.Lerp(hazeHeightFalloff.value, nightItem.hazeHeightFalloff.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.hazeScatteringDir.value = Mathf.Lerp(hazeScatteringDir.value, nightItem.hazeScatteringDir.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.hazeScatteringRatio.value = Mathf.Lerp(hazeScatteringRatio.value, nightItem.hazeScatteringRatio.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogOpacity02.value = Mathf.Lerp(fogOpacity02.value, nightItem.fogOpacity02.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogScattering.value = Mathf.Lerp(fogScattering.value, nightItem.fogScattering.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogExtinction.value = Mathf.Lerp(fogExtinction.value, nightItem.fogExtinction.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogHeightFalloff.value = Mathf.Lerp(fogHeightFalloff.value, nightItem.fogHeightFalloff.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogDistance.value = Mathf.Lerp(fogDistance.value, nightItem.fogDistance.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogScatteringDir.value = Mathf.Lerp(fogScatteringDir.value, nightItem.fogScatteringDir.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogHeight.value = Mathf.Lerp(fogHeight.value, nightItem.fogHeight.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogAlpha.value = Mathf.Lerp(fogAlpha.value, nightItem.fogAlpha.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogDensity.value = Mathf.Lerp(fogDensity.value, nightItem.fogDensity.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogAmbient.color = Color.Lerp(fogAmbient.color, nightItem.fogAmbient.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogLight.color = Color.Lerp(fogLight.color, nightItem.fogLight.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogColor.color = Color.Lerp(fogColor.color, nightItem.fogColor.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.fogHorizon.color = Color.Lerp(fogHorizon.color, nightItem.fogHorizon.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            current.caveAmbient.value = Mathf.Lerp(caveAmbient.value, nightItem.caveAmbient.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
            return current;
        }
        */
        float MergeParamWithNightItem(float param, DS_HazeContextItem nightItem)
        {
            return Mathf.Lerp(param, nightItem.airHeightFalloff.EvaluateF(ASkyLighting.nightEclipseTimeHaze), ASkyLighting.eclipsePower);
        }

        public void CopyFrom(DS_HazeContextItem other)
        {
            if (other == null)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogError("DeepSky::DS_HazeContextItem:CopyFrom - null context passed!");
#endif
                return;
            }

            Type thisType = GetType();
            Type otherType = other.GetType();

            foreach (FieldInfo field in thisType.GetFields())
            {
                FieldInfo otherField = otherType.GetField(field.Name);
                field.SetValue(this, otherField.GetValue(other));
            }

            airScattering.GetCurve(other.airScattering);
            airHeightFalloff.GetCurve(other.airHeightFalloff);

            hazeScattering.GetCurve(other.hazeScattering);
            hazeHeightFalloff.GetCurve(other.hazeHeightFalloff);
            hazeScatteringDir.GetCurve(other.hazeScatteringDir);
            hazeScatteringRatio.GetCurve(other.hazeScatteringRatio);

            fogOpacity02.GetCurve(other.fogOpacity02); 
            fogScattering.GetCurve(other.fogScattering);
            fogExtinction.GetCurve(other.fogExtinction);

            fogHeightFalloff.GetCurve(other.fogHeightFalloff);
            fogDistance.GetCurve(other.fogDistance);
            fogHeight.GetCurve(other.fogHeight);
            fogScatteringDir.GetCurve(other.fogScatteringDir);

            fogAmbient.GetGradient(other.fogAmbient);
            fogLight.GetGradient(other.fogLight);
            fogHorizon.GetGradient(other.fogHorizon);
            fogColor.GetGradient(other.fogColor);
            fogAlpha.GetCurve(other.fogAlpha);
            fogDensity.GetCurve(other.fogDensity);
            caveAmbient.GetCurve(other.caveAmbient);


        }

    }
}
