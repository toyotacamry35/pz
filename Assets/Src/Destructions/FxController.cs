using UnityEngine;  //for ParticleSystem
using ColonyHelpers;
using Core.Environment.Logging.Extension;
using UnityEngine.Serialization;
using NLog;

namespace Assets.Src.Destructions
{
    // TODO: use new decal-tool
    // All 3-slashed now comments are about decals (which are tmp-rary absent)
    public class FxController : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [FormerlySerializedAs("HitParticle")] [SerializeField] private ParticleSystem _hitParticle;
        ///public EasyDecal hitDecal;
        // set it to i.e. 80 to get particles rotated +80 or -80 randomly.
        [FormerlySerializedAs("RndAngleAbs")] [SerializeField] private float _rndAngleAbs;

        private bool _createDecal = true;

        // Use this for initialization
        void Start()
        {
            // 1. Off decal & particle children (used only in Editor)

            //#if !UNITY_EDITOR
            if (Application.isPlaying)
            {
                ///var decalEditorTemplate = GetComponentInChildren<EasyDecal>();            
                ///if (decalEditorTemplate)
                ///    decalEditorTemplate.gameObject.SetActive(false);
                ///else
                ///    LogWarn("Didn't find \"Decal\" child");

                var particleEditorTemplate = GetComponentInChildren<ParticleSystem>();
                if (particleEditorTemplate)
                    particleEditorTemplate.gameObject.SetActive(false);
            }
            // else Application.isEditor == true
            //#endif //!UNITY_EDITOR

            // 2. FastFall:

            ///if (!hitDecal || !HitParticle)
            if (!_hitParticle)
            {
                 Logger.IfError()?.Message("!hitDecal  ||  !hitParticle").Write();;
                return;
            }

            /// // for faster execution:
            /// hitDecal.OnlyColliders = true;

            // 3. Call main work:

            PlayEffect();
        }


        public void Init(bool bCreateDecal = true)
        {
            _createDecal = bCreateDecal;
        }

        private const float ParticlesLifeTime = 10f;

        private void PlayEffect()
        {
            Vector3 p = transform.position;
            Vector3 n = transform.rotation * Vector3.forward;

            // 1. Decal:
            if (_createDecal)
            {
                /// //#TODO: calc.rotation - from hitPoint & Player relative pos.:
                /// float angle = Helpers.Rnd(90f);
                /// hitDecal = EasyDecal.Project(hitDecal.gameObject, p, n, angle);
                /// //hitDecal = EasyDecal.ProjectAt(hitDecal.gameObject, transform.gameObject, p, n, 0f/*Hf.rnd(10f)*//*, cactusDecalScale*/); // alt.method
                /// if (hitDecal)
                /// {
                ///     hitDecal.gameObject.SetActive(true);
                ///     hitDecal.transform.SetParent(transform);
                ///     hitDecal.ShowDir = true; // Dbg
                /// }
                /// else
                /// { LogError($"Projecting decal failed. (n== {n}, p== {p})."); }
            }

            // 2. Particles:

            int sign = SharedHelpers.RndBool() ? 1 : -1;
            Quaternion rot = transform.rotation * Quaternion.Euler(0, sign * _rndAngleAbs, 0);
            _hitParticle = Instantiate(_hitParticle, p, rot, transform);

            VLogDrawRainbowLine(p, n);

            // 3. Destroy timers:

            ///float totalLifeTime = hitDecal.Lifetime + hitDecal.FadeoutTime;

            ///Destroy(HitParticle, ParticlesLifeTime);
            ///Destroy(gameObject, totalLifeTime);
            Destroy(gameObject, ParticlesLifeTime);
        }

        private static readonly string LogSubject = VLogsSubjects.Destructing.ToString();
        void VLogDrawRainbowLine(Vector3 start, Vector3 normal)
        {
            VisualLogs.SubjDrawRainbowLine(LogSubject, start, normal, 1f);
        }

    }

}