using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TOD;
using System;
using System.Reflection;



namespace TOD
{

    [System.Serializable]
    public class ASkyLightingContext
    {
        [System.Serializable]
        public class ASkyLightingNightItem
        {
            public bool isOpen;

            public float atmosphereThickness;
            public float skyElevation;
            public float exposure;

            

            [ColorUsage(true, true)]
            public Color topSky;
            [ColorUsage(true, true)]
            public Color horizonSky;
            [ColorUsage(true, true)]
            public Color starsColor;
            public float startIntensity;
            public float fxAmbient;
            public float fxAmbient02;
            public float fxAmbient03;

            public float ambientIntensity;
            [ColorUsage(true, true)]
            public Color ambientGround;
            [ColorUsage(true, true)]
            public Color ambientEquator;
            [ColorUsage(true, true)]
            public Color ambientSky;
            [ColorUsage(true, true)]
            public Color groundColor;

            [ColorUsage(true, true)]
            public Color cloudSkyColor;
            [ColorUsage(true, true)]
            public Color cloudSunHighlight;
            [ColorUsage(true, true)]
            public Color cloudMoonHighlight;
            public float cloudLightIntensity;

            [ColorUsage(true, true)]
            public Color eclipseSunColor;

            public float eclipseSunPower;
            [ColorUsage(true, true)]
            public Color eclipseTopSkyColor;

            

            public void CopyFrom(ASkyLightingNightItem other)
            {
                if (other == null)
                {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                    Debug.LogError("TimeOfDayContext:CopyFrom - null context passed!");
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
            }
        }

        public ColorGradient topSky;
        public ColorGradient horizonSky;
        public ColorGradient sunColor;
        public ColorGradient starsColor;
        public ColorGradient ambientColor;
        public ColorGradient ambientEquator;
        public ColorGradient ambientSky;
        public ColorGradient ambientGround;
        public ColorGradient groundColor;

        public FloatCurve skyElevation;
        public FloatCurve atmosphereThickness;
        public FloatCurve sunLightIntensity;
        public FloatCurve sunLightShadowStrenght;
        public FloatCurve sunLightShadowBias;
        public FloatCurve starsIntensity;
        public FloatCurve exposure;
        public FloatCurve ambientIntensity;
        public FloatCurve fxAmbient;
        public FloatCurve fxAmbient02;
        public FloatCurve fxAmbient03;

        public bool isEclipse = true;
        public bool useHorizonFade = true;
        public bool useClouds = true;
        public bool useCelestials = true;
        public bool useStars = true;

        public float worldLongitude;
        public float worldLatitude;
        public float nightTimePercentage;
        public float sunSize;
        public float sunSpotSize;
        public FloatCurve starsLightIntensity;
        public float starsLightShadowStrenght;
        public float starsLightShadowBias;

      

        [ColorUsage(false, true)]
        public Color starsLightColor;

        public Flare sunFlare;

        
        public List<Celestial> sattellites;
        public Vector3 starsOffsets = Vector3.zero;
        public int cloudRenderingLayer = 31;
        public UnityEngine.Rendering.AmbientMode ambientMode;
        public AWeather weather = null;
        public ASkyLightingNightItem nightItem;
        

        public ASkyLightingContextAsset GetContextAsset()
        {
            ASkyLightingContextAsset cxt = ScriptableObject.CreateInstance<ASkyLightingContextAsset>();

            cxt.context.CopyFrom(this);
            return cxt;
        }

        public void CopyFrom(ASkyLightingContext other)
        {
#if UNITY_EDITOR
            if (other == null)
            {
                Debug.LogError("Context:CopyFrom - null context passed!");
                return;
            }

            Type thisType = GetType();
            Type otherType = other.GetType();

            
            foreach (FieldInfo field in thisType.GetFields())
            {
                FieldInfo otherField = otherType.GetField(field.Name);
                field.SetValue(this, otherField.GetValue(other));
            }


            topSky.GetGradient(other.topSky);
            horizonSky.GetGradient(other.horizonSky);
            skyElevation.GetCurve(other.skyElevation);
            atmosphereThickness.GetCurve(other.atmosphereThickness);
            sunColor.GetGradient(other.sunColor);
            sunLightIntensity.GetCurve(other.sunLightIntensity);
            sunLightShadowStrenght.GetCurve(other.sunLightShadowStrenght);
            sunLightShadowBias.GetCurve(other.sunLightShadowBias);
                        
            ambientColor.GetGradient(other.ambientColor);
            ambientEquator.GetGradient(other.ambientEquator);
            ambientSky.GetGradient(other.ambientSky);
            ambientGround.GetGradient(other.ambientGround);
            groundColor.GetGradient(other.groundColor);

            fxAmbient.GetCurve(other.fxAmbient);
            fxAmbient02.GetCurve(other.fxAmbient02);
            fxAmbient03.GetCurve(other.fxAmbient03);

            starsColor.GetGradient(other.starsColor);
            starsIntensity.GetCurve(other.starsIntensity);
            starsLightIntensity.GetCurve(other.starsLightIntensity);
            exposure.GetCurve(other.exposure);
            ambientIntensity.GetCurve(other.ambientIntensity);
            
            sattellites = new List<Celestial>();

            for (int i = 0; i < other.sattellites.Count; i++)
            {
                sattellites.Add(new Celestial());
                sattellites[i].CopyFrom(other.sattellites[i]);
            }

            nightItem.CopyFrom(other.nightItem);
            weather.CopyFrom(other.weather);
#endif           
        }

        public void CreateSplitData(DayPart[] dayParts)
        {
#if UNITY_EDITOR

            topSky.SplitGradientToParts(dayParts);
            horizonSky.SplitGradientToParts(dayParts);
            sunColor.SplitGradientToParts(dayParts);
            starsColor.SplitGradientToParts(dayParts);
            ambientColor.SplitGradientToParts(dayParts);
            ambientEquator.SplitGradientToParts(dayParts);
            ambientSky.SplitGradientToParts(dayParts);
            ambientGround.SplitGradientToParts(dayParts);
            groundColor.SplitGradientToParts(dayParts);
            weather.cloudSystem.cloudsSettings.skyColor.SplitGradientToParts(dayParts);
            weather.cloudSystem.cloudsSettings.sunHighlight.SplitGradientToParts(dayParts);
            weather.cloudSystem.cloudsSettings.moonHighlight.SplitGradientToParts(dayParts);

            skyElevation.SplitCurveToParts(dayParts);
            atmosphereThickness.SplitCurveToParts(dayParts);
            sunLightIntensity.SplitCurveToParts(dayParts);
            sunLightShadowStrenght.SplitCurveToParts(dayParts);
            sunLightShadowBias.SplitCurveToParts(dayParts);
            starsIntensity.SplitCurveToParts(dayParts);
            starsLightIntensity.SplitCurveToParts(dayParts);
            exposure.SplitCurveToParts(dayParts);
            ambientIntensity.SplitCurveToParts(dayParts);
            fxAmbient.SplitCurveToParts(dayParts);
            fxAmbient02.SplitCurveToParts(dayParts);
            fxAmbient03.SplitCurveToParts(dayParts);
            weather.cloudSystem.cloudsSettings.lightIntensity.SplitCurveToParts(dayParts);
#endif 
        }

        public void MergeSplitData(DayPart[] dayParts)
        {
#if UNITY_EDITOR
            if (topSky.parts == null)
                return;

            topSky.MergePartsToGradient(dayParts);
            horizonSky.MergePartsToGradient(dayParts);
            sunColor.MergePartsToGradient(dayParts);
            starsColor.MergePartsToGradient(dayParts);
            ambientColor.MergePartsToGradient(dayParts);
            ambientEquator.MergePartsToGradient(dayParts);
            ambientSky.MergePartsToGradient(dayParts);
            ambientGround.MergePartsToGradient(dayParts);
            groundColor.MergePartsToGradient(dayParts);
            weather.cloudSystem.cloudsSettings.skyColor.MergePartsToGradient(dayParts);
            weather.cloudSystem.cloudsSettings.sunHighlight.MergePartsToGradient(dayParts);
            weather.cloudSystem.cloudsSettings.moonHighlight.MergePartsToGradient(dayParts);

            skyElevation.MergePartsToCurve(dayParts);
            atmosphereThickness.MergePartsToCurve(dayParts);
            sunLightIntensity.MergePartsToCurve(dayParts);
            sunLightShadowStrenght.MergePartsToCurve(dayParts);
            sunLightShadowBias.MergePartsToCurve(dayParts);
            starsIntensity.MergePartsToCurve(dayParts);
            starsLightIntensity.MergePartsToCurve(dayParts);
            exposure.MergePartsToCurve(dayParts);
            ambientIntensity.MergePartsToCurve(dayParts);
            fxAmbient.MergePartsToCurve(dayParts);
            fxAmbient02.MergePartsToCurve(dayParts);
            fxAmbient03.MergePartsToCurve(dayParts);
            weather.cloudSystem.cloudsSettings.lightIntensity.MergePartsToCurve(dayParts);
#endif 
        }


    }    
}