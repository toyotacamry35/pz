using System;
using Assets.ASkyLighting.SSShadows;
using Assets.Src.Instancenator;
using Core.Environment.Logging.Extension;
using NLog;
using Trive.Rendering;

namespace Assets.Src.App.QualitySettings
{
    public enum QualityLevels
    {
        ///#IMPORTANT: !!! SHOULD BE MANUALLY SYNCRONIZED WITH LocalizationKeysDef resource jdb: `L10n_Settings_QualityLvl_enum.jdb` !!!

        Minimal = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Ultra = 4
    }

    public static class QualitySimpleSetter
    {
        
        public enum SSRQuality
        {
            None, 
            Stohastic
        }
        
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        internal static QualityLevels? DBG_LastSetQLvl = null;
        public static bool SetQuality(QualityLevels level)
        {
            bool ssshadows;
            SSRQuality ssr;
            float vegetationBias;
            
            DBG_LastSetQLvl = level;

            switch (level)
            {
                case QualityLevels.Minimal:
                    ssshadows = false;
                    vegetationBias = 0.2f;
                    ssr = SSRQuality.None;
                    break;
                case QualityLevels.Low:
                    ssshadows = false;
                    vegetationBias = 0.5f;
                    ssr = SSRQuality.None;
                    break;
                case QualityLevels.Medium:
                    ssshadows = true;
                    vegetationBias = 0.8f;
                    ssr = SSRQuality.None;
                    break;
                case QualityLevels.High:
                    ssshadows = true;
                    vegetationBias = 1.0f;
                    ssr = SSRQuality.None;
                    break;
                case QualityLevels.Ultra:
                    vegetationBias = 1.4f;
                    ssshadows = true;
                    ssr = SSRQuality.Stohastic;
                    break;
                default:
                     Logger.IfInfo()?.Message("No actions defined for quality level {0}",  level).Write();
                    return false;
            }

            UnityEngine.QualitySettings.SetQualityLevel((int)level, true);
            InstanceCompositionDistanceManager.SetVegetationLodBias(vegetationBias, false);
            SSShadowsControl.SSShadowsEnabled = ssshadows;
            switch (ssr)
            {
                case SSRQuality.None:
                    if(PostProcessGlobalVolumeController.Instance!=null)
                        PostProcessGlobalVolumeController.Instance.SetSSROnAll(false);
                    break;
                case SSRQuality.Stohastic:
                    if(PostProcessGlobalVolumeController.Instance!=null)
                        PostProcessGlobalVolumeController.Instance.SetSSROnAll(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return true;
        }
    }

}
