using Assets.Src.AI.DamageIndication;
using Assets.Src.Aspects.Impl.EntityGameObjectComponents;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using Assets.Src.Wizardry;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;
using Src.Aspects.Impl.Stats.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uins;
using UnityEngine;

namespace Assets.Src.Aspects
{
    class DebugHpDrawer : EntityGameObjectComponent
    {
        public TimeStatState State;
        HpIndicationSwitchersController _hpIndicator;
        public JdbMetadata Stat;
        public float OffsetY;
        public Color Color = Color.red;
        public float DrawDistance = 10;
        public Vector2 From;
        public float SizeOnMainChar = 150f;
        public float SizeOnOtherChar = 150f;
        public bool OnlyOnMainChar = false;
        public bool BlackEnvelope = false;
        protected override void GotClient()
        {
            if (!Constants.WorldConstants.ShowHPBars)
                return;
            _hpIndicator = gameObject.AddComponent<HpIndicationSwitchersController>();
            _hpIndicator._hpIndicationShaderSwitchers = Array.Empty<HpIndicationShaderSwitcher>();
            HpIndicationSwitchersController.SubscribeHpIndicatorController(Stat.Get<StatResource>(), TypeId, EntityId, _hpIndicator, ClientRepo, Aspects.SubscribeUnsubscribe.Subscribe);
        }

        protected override void LostClient()
        {
            if (!Constants.WorldConstants.ShowHPBars)
                return;
            HpIndicationSwitchersController.SubscribeHpIndicatorController(Stat.Get<StatResource>(), TypeId, EntityId, _hpIndicator, ClientRepo, Aspects.SubscribeUnsubscribe.Unsubscribe);
            Destroy(_hpIndicator);
        }
        private void OnGUI()
        {
            if (!Constants.WorldConstants.ShowHPBars)
            {

                this.enabled = false;
                return;
            }
            if (OnlyOnMainChar && GameState.Instance.CharacterRuntimeData.CharacterId != EntityId)
                return;
            var ssize = new Vector2(Screen.width * 0.5f, Screen.height);
            if (UnityEngine.Camera.main == null)
                return;
            if ((UnityEngine.Camera.main.transform.position - transform.position).magnitude > DrawDistance)
                return;
            if (_hpIndicator == null)
                return;
            var pos = (transform.position + new Vector3(0, 2.5f, 0)).WorldToGuiPoint();
            if (pos.z < 0)
                return;
            var texRect = new Rect(pos, new Vector2(GameState.Instance.CharacterRuntimeData.CharacterId != EntityId ? SizeOnOtherChar : SizeOnMainChar, 6));
            var rect = texRect;
            rect.position += new Vector2(-rect.width / 2, rect.size.y + OffsetY);
            float fill = _hpIndicator.CurrentValue;
            Color fillColor = Color;
            if(EntityId == GameState.Instance.CharacterRuntimeData.CharacterId)
            {
                rect.position = ssize - From;
                if(BlackEnvelope)
                    GUIExtensions.DrawRectangle(new Rect(rect.position - new Vector2(1, 1), new Vector2(rect.width * fill + 2, rect.height + 2)), Color.black);
                else
                    GUIExtensions.DrawRectangle(new Rect(rect.position - new Vector2(1, 1), new Vector2(rect.width + 2, rect.height + 2)), Color.black);
                GUIExtensions.DrawRectangle(new Rect(rect.position, new Vector2(rect.width * fill, rect.height)), fillColor);
                
            }
            else
            {
                if(BlackEnvelope)
                    GUIExtensions.DrawRectangle(new Rect(rect.position - new Vector2(1, 1), new Vector2(rect.width * fill + 2, rect.height + 2)), Color.black);
                else
                    GUIExtensions.DrawRectangle(new Rect(rect.position - new Vector2(1, 1), new Vector2(rect.width + 2, rect.height + 2)), Color.black);

                GUIExtensions.DrawRectangle(new Rect(rect.position, new Vector2(rect.width * fill, rect.height)), fillColor);

            }
            var textArea = new Rect(new Vector2(rect.xMax, rect.y), new Vector2(150, 25));
        }
    }
}
