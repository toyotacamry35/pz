using Assets.Src.Aspects.SpatialObjects;
using Assets.Src.SpawnSystem;
using Assets.Src.Wizardry;
using NLog;
using SharedCode.Aspects.Item.Templates;
using System;
using System.Collections;
using Core.Environment.Logging.Extension;
using SharedCode.Wizardry;
using UnityEngine;
using SVector3 = SharedCode.Utils.Vector3;

namespace Assets.Src.Aspects.VisualMarkers
{
    public class VisualMarkerSelector : EntityGameObjectComponent
    {
        protected static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public float Radius = Constants.WorldConstants.SelectionRadius;

        public TargetHolder TargetHolder { get; private set; }

        public SVector3 Position => (SVector3)transform.position;

        public ISpellDoer SpellDoer { get; private set; }

        private const float _spatialMarkersUpdateDelay = 0.25f;

        public void Init(ISpellDoer spellDoer, TargetHolder targetHolder)
        {
            SpellDoer = spellDoer;
            TargetHolder = targetHolder;
        }

        private Coroutine _coroutine;

        void Start()
        {
            var coroutineDelay = new WaitForSeconds(_spatialMarkersUpdateDelay);
            _coroutine = this.StartInstrumentedCoroutineLiteWeight(UpdateSpatialMarkers(coroutineDelay));
        }

        private IEnumerator UpdateSpatialMarkers(WaitForSeconds delay)
        {
            while (true)
            {
                yield return delay;
                try
                {
                    AABBTriggeredCoordinator.Instance.SetPosition(transform.position);
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Exception(e).Write();
                }
            }
        }

        protected override void DestroyInternal()
        {
            this.StopCoroutineIfNotNull(ref _coroutine);
        }
    }
}
