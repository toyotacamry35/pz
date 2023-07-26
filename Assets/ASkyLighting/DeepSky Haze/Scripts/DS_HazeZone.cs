using UnityEngine;
using TOD;

namespace DeepSky.Haze
{
    [ExecuteInEditMode, AddComponentMenu("DeepSky Haze/Zone", 52)]
    public class DS_HazeZone : MonoBehaviour
    {
        [SerializeField]
        private DS_HazeContext m_Context = new DS_HazeContext();
        public DS_HazeContextAsset m_WaitingToLoad = null;
        public DS_HazeContextAsset defaultPreset;
        

        public void LoadPresetDefault()
        {
            Context.CopyFrom(defaultPreset.Context);
            LoadFromContextPreset(defaultPreset);  
        }

        public void LoadFromContextPreset(DS_HazeContextAsset ctx)
        {
            Context.CopyFrom(ctx.Context);
            
            DayPart[] dayParts = new DayPart[ASkyLighting._instance.dayParts.Length];
            
            for (int i=0; i< ASkyLighting._instance.dayParts.Length; i++)
            {

                dayParts[i] = new DayPart(ASkyLighting._instance.dayParts[i].percent, ASkyLighting._instance.dayParts[i].defaultPercent, 
                    ASkyLighting._instance.dayParts[i].color, ASkyLighting._instance.dayParts[i].state, ASkyLighting._instance.dayParts[i].reflections);
            }
            ASkyLighting.StretchDayPart(ref dayParts, Context.m_ComplexItem.nightTimePercentBaked);

            CreateSplit(dayParts);
            Merge(ASkyLighting._instance.dayParts);
            defaultPreset = ctx;
        }
        
        public DS_HazeContext Context
        {
            get { return m_Context; }
        }
       
        public void CreateSplit(DayPart[] dayParts)
        {
            m_Context.m_ComplexItem.airScattering.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.airHeightFalloff.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.fogHorizon.SplitGradientToParts(dayParts);

            m_Context.m_ComplexItem.hazeScattering.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.hazeHeightFalloff.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.hazeScatteringDir.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.hazeScatteringRatio.SplitCurveToParts(dayParts);

            m_Context.m_ComplexItem.fogOpacity02.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.fogScattering.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.fogExtinction.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.fogHeightFalloff.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.fogDistance.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.fogHeight.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.fogScatteringDir.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.fogAmbient.SplitGradientToParts(dayParts);
            m_Context.m_ComplexItem.fogLight.SplitGradientToParts(dayParts);

            m_Context.m_ComplexItem.fogColor.SplitGradientToParts(dayParts);
            m_Context.m_ComplexItem.fogAlpha.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.fogDensity.SplitCurveToParts(dayParts);
            m_Context.m_ComplexItem.caveAmbient.SplitCurveToParts(dayParts);
        }

        
        public void Merge(DayPart[] dayParts)
        {
            m_Context.m_ComplexItem.airScattering.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.airHeightFalloff.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.fogHorizon.MergePartsToGradient(dayParts);

            m_Context.m_ComplexItem.hazeScattering.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.hazeHeightFalloff.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.hazeScatteringDir.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.hazeScatteringRatio.MergePartsToCurve(dayParts);

            m_Context.m_ComplexItem.fogOpacity02.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.fogScattering.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.fogExtinction.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.fogHeightFalloff.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.fogDistance.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.fogHeight.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.fogScatteringDir.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.fogAmbient.MergePartsToGradient(dayParts);
            m_Context.m_ComplexItem.fogLight.MergePartsToGradient(dayParts);

            m_Context.m_ComplexItem.fogColor.MergePartsToGradient(dayParts);
            m_Context.m_ComplexItem.fogAlpha.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.fogDensity.MergePartsToCurve(dayParts);
            m_Context.m_ComplexItem.caveAmbient.MergePartsToCurve(dayParts);
        }

        public void SetupCurves()
        {
            m_Context.m_ComplexItem.UpdateCurvesFull();
        }

    }
}