using Assets.Src.Camera;
using JetBrains.Annotations;
using NLog;
using Src.Aspects.Doings;
using Src.Input;
using System;
using System.Threading.Tasks;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.Src.Aspects.Doings
{
    internal class PlayerBrain : CharacterBrain, IDisposableCharacterBrain
    {
        // ReSharper disable once UnusedMember.Local
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("Brain.Character");

        public event Action OnFrameStart;
        public event Action OnFrameEnd;

        public GameObject GameObject => this != null ? gameObject : null;

        public bool IsBot => false;
        
        internal async Task<PlayerBrain> Init(
            ISpellDoer doer,
            OuterRef entityRef,
            IEntitiesRepository repository,
            TargetHolder targetHolder,
            CameraGuideController.ISettings guideControllerSettingsProvider)
        {
            UnityQueueHelper.AssertInUnityThread();
            if (doer == null) throw new ArgumentNullException(nameof(doer)); 
            
            await base.Init(
                entityRef: entityRef,
                repository: repository, 
                spellDoer: doer, 
                targetHolder: targetHolder, 
                cameraController: new CameraGuideController(guideControllerSettingsProvider), 
                inputProvider: InputManager.Instance);
            
            return this;
        }

        private void Update()
        {
#if UNITY_EDITOR // чтобы когда хочется что-то подправить в редакторе в рантайме, и для этого "отцепляешь" мышь нажав ctrl, камера не крутилась вслед за мышью     
            if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
#endif
                UpdateCamera();
        }
   }
}