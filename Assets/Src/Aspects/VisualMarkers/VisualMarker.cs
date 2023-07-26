using Assets.Src.Aspects.Impl;
using SharedCode.AI;

namespace Assets.Src.Aspects.VisualMarkers
{
    public class VisualMarker : AVisualMarker
    {
        protected override void GotClient()
        {
            ShowMarker = GetShowMarkerFlag();

            OnceInit();
            IsNearRp.Value = true;
        }

        protected override void LostClient()
        {
            IsNearRp.Value = false;
        }

        protected override void DestroyInternal()
        {
            IsNearRp.Value = false;
        }

        protected override bool GetIsOurPlayer()
        {
            if (Ego.AssertIfNull(nameof(Ego), gameObject) ||
                GameState.Instance.AssertIfNull(nameof(GameState), gameObject))
                return false;

            return Ego.EntityId == GameState.Instance.CharacterRuntimeData.CharacterId;
        }

        private bool GetShowMarkerFlag()
        {
            var interactiveFlag = GetComponent<Interactive>()?.Def?.ShowMarker ?? true; //1) Если есть запрет на Interactive
            if (!interactiveFlag)
                return false;

            if (Ego.EntityDef is LegionaryEntityDef legionaryEntityDef)
                return legionaryEntityDef.HasNpcMarker; //2) Флаг LegionaryEntityDef 

            return true; //3) Всем остальным можно
        }
    }
}