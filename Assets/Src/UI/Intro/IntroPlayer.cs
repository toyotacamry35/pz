using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using L10n;
using ReactivePropsNs;
using ShareCode.Threading;
using SharedCode.Logging;
using Uins.Sound;
using UnityEngine;
using UnityEngine.Video;
using UnityAwaiters = UnityThreading.Awaiters;

namespace Uins.Intro
{
    public class IntroPlayer : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const float _Timeout = 5;

        [SerializeField]
        public AudioSource _audioSource;

        [SerializeField]
        public VideoPlayer _videoPlayer;

        [SerializeField]
        public Video[] _videos;

        // public event Action IntroStarted;
        // public event Action IntroFinished;
        public event Action<string> VideoStarted;
        public event Action<string, float> VideoPlaying;
        public event Action<string> VideoFinished;

        private CancellationTokenSource _skipVideo;
        private readonly DisposableComposite _disposables = new DisposableComposite();

        public ReactiveProperty<bool> IsIntroPlayingRp { get; } = new ReactiveProperty<bool>() {Value = false};

        public ReactiveProperty<bool> CanSkipVideoRp { get; } = new ReactiveProperty<bool>() {Value = false};

        void Awake()
        {
            _videoPlayer.errorReceived += OnError;
            if (!_videoPlayer.targetCamera)
                _videoPlayer.targetCamera = Camera.main ? Camera.main : Camera.allCameras[0];
            _audioSource.volume = SoundControl.SavedMasterVolume / 100;
        }

        void OnDestroy()
        {
            _disposables.Dispose();
        }

        public Task Play(CancellationToken ct)
        {
            return PlayScenario(ct);
        }

        public void SkipCurrentVideo()
        {
            if (_skipVideo != null && !_skipVideo.IsCancellationRequested)
                _skipVideo.Cancel();
        }

        async Task PlayScenario(CancellationToken ct)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Intro started").Write();
            UnityQueueHelper.RunInUnityThread(() => IsIntroPlayingRp.Value = true);
            var localizationCode = await UnityQueueHelper.RunInUnityThread(() => LocalizationInit.CurrentLocalizationCode);

            for (var index = 0; index < _videos.Length && !ct.IsCancellationRequested; index++)
            {
                var video = _videos[index];
                using (_skipVideo = video.AllowSkip ? new CancellationTokenSource() : null)
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(ct, _skipVideo != null ? _skipVideo.Token : CancellationToken.None))
                {
                    if (CanSkipVideoRp.Value != video.AllowSkip)
                        UnityQueueHelper.RunInUnityThreadNoWait(() => { CanSkipVideoRp.Value = video.AllowSkip; });
                    await PlayVideo(video.VideoFile, SelectAudioTrack(video.AudioTracks, localizationCode), cts.Token);
                }

                _skipVideo = null;
            }

            UnityQueueHelper.RunInUnityThreadNoWait(
                () =>
                {
                    IsIntroPlayingRp.Value = false;
                    CanSkipVideoRp.Value = false;
                });
            
            Logger.IfDebug()?.Message($"Intro finished").Write();
        }

        async Task PlayVideo(string video, (int track, int tracksCount) audio, CancellationToken ct)
        {
            try
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Video Player | Video:{video} AudioTrack:{audio.track} AudioTracksCount:{audio.tracksCount}").Write();

                await UnityAwaiters.UnityThread;
                VideoStarted?.Invoke(video);

                ct.ThrowIfCancellationRequested();

                _videoPlayer.source = VideoSource.Url;
                _videoPlayer.url = $"file://{Application.streamingAssetsPath}/Video/{video}";
                _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                _videoPlayer.controlledAudioTrackCount = (ushort) audio.tracksCount;
                for (ushort audioTrack = 0; audioTrack < audio.tracksCount; ++audioTrack)
                {
                    if (Logger.IsDebugEnabled && audioTrack == audio.track) Logger.IfDebug()?.Message($"Enable audio track {audio.track}").Write();
                    _videoPlayer.EnableAudioTrack(audioTrack, audioTrack == audio.track);
                    _videoPlayer.SetTargetAudioSource(audioTrack, audioTrack == audio.track ? _audioSource : null);
                }

                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Prepare video | Video:{video}").Write();
                
                _videoPlayer.Prepare();

                ct.ThrowIfCancellationRequested();

                for (float time = 0; !_videoPlayer.isPrepared && time < _Timeout; time += Time.unscaledDeltaTime)
                {
                    await Awaiters.ThreadPool;
                    await UnityAwaiters.UnityThread;
                    ct.ThrowIfCancellationRequested();
                }

                if (!_videoPlayer.isPrepared)
                {
                    Logger.Error($"Can't prepare video {_videoPlayer.url}");
                    return;
                }

                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Play video | Video:{video}").Write();
                
                Log.StartupStopwatch.Milestone("Intro Play");
                _videoPlayer.Play();
                if (audio.track >= 0 && audio.track < audio.tracksCount)
                    _audioSource.Play();

                for (float time = 0; !_videoPlayer.isPlaying && time < _Timeout; time += Time.unscaledDeltaTime)
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Waiting for video playing | Video:{video}").Write();
                    await Awaiters.ThreadPool;
                    await UnityAwaiters.UnityThread;
                    ct.ThrowIfCancellationRequested();
                }

                while (_videoPlayer.isPlaying)
                {
                    if (Logger.IsTraceEnabled) Logger.Trace($"Video playing | Video:{video} Time:{_videoPlayer.time:F3}");
                    VideoPlaying?.Invoke(video, (float) _videoPlayer.time);
                    await Awaiters.ThreadPool;
                    await UnityAwaiters.UnityThread;
                    ct.ThrowIfCancellationRequested();
                }
                
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Video finished | Video:{video}").Write();
            }
            catch (OperationCanceledException)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message("Video cancelled").Write();
            }
            finally
            {
                await UnityAwaiters.UnityThread;
                if (_videoPlayer && _videoPlayer.isPlaying)
                    _videoPlayer.Stop();
                if (_audioSource && _audioSource.isPlaying)
                    _audioSource.Stop();
                VideoFinished?.Invoke(video);
            }
        }

        private void OnError(VideoPlayer source, string message)
        {
            Debug.LogError($"Video player error: {message}");
        }

        private (int track, int tracksCount) SelectAudioTrack(Audio[] audioTracks, string localizationCode)
        {
            var audioTrack = -1;
            var tracksCount = audioTracks?.Length ?? 0;
            if (tracksCount > 0)
            {
                var i = Array.FindIndex(audioTracks, x => x.LocalizationCode.Equals(localizationCode, StringComparison.OrdinalIgnoreCase));
                if (i == -1)
                    i = 0;
                audioTrack = audioTracks[i].TrackIndex;
            }
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"LocalizationCode:{localizationCode} AudioTrack:{audioTrack} TracksCount:{tracksCount}").Write();
            return (audioTrack, tracksCount);
        }

        [Serializable]
        public struct Video
        {
            public string VideoFile;
            public bool AllowSkip;
            public Audio[] AudioTracks;
        }

        [Serializable]
        public struct Audio
        {
            public string LocalizationCode;
            public int TrackIndex;
        }
    }
}