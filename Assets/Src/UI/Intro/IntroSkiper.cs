using System.Collections;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Uins.Intro
{
    public class IntroSkiper : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        [SerializeField] private IntroPlayer _introPlayer;
        [SerializeField] private float _skipDelay;

        void Awake()
        {
            _introPlayer.VideoStarted += OnVideoStarted;
            _introPlayer.VideoFinished += OnVideoFinished;
        }

        void OnDestroy()
        {
            _introPlayer.VideoStarted -= OnVideoStarted;
            _introPlayer.VideoFinished -= OnVideoFinished;
        }

        private void OnVideoStarted(string obj) => StartCoroutine(Routine());

        private void OnVideoFinished(string obj) => StopCoroutine(Routine());

        IEnumerator Routine()
        {
            while (Input.anyKey)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Wait for skip key released").Write();
                yield return null;
            }

            float skip = 0;
            while (true)
            {
                if (Input.GetKey(KeyCode.Escape))
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Skip key pressed").Write();
                    skip += Time.unscaledDeltaTime;
                }
                else
                    skip = 0;

                if (skip > _skipDelay)
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Skip").Write();
                    _introPlayer.SkipCurrentVideo();
                }

                yield return null;
            }
        }
    }
}