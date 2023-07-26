using Assets.Src.SpawnSystem;

namespace Uins
{
    //#Tmp: Пока UI не пересядет с EntityApi на EGO, это временное решение.
    //.. Этот комп.должен быть на любом обэекте с компонентом `HasOwnerEntityNotifier`. (К сожалению, нельзя добавить EGOComponent в run-time).
    public class HasOwnerEntityNotifierEgoComponent : EntityGameObjectComponent
    {
        private HasOwnerEntityNotifier _notifier;
        private HasOwnerEntityNotifier GetNotifier => (_notifier != null) ? _notifier : _notifier = GetComponent<HasOwnerEntityNotifier>();

        protected override void GotClient()
        {
            GetNotifier?.SubscribeClient(EntityId);
        }

        protected override void LostClient()
        {
            GetNotifier?.UnsubscribeClient();
        }
    }

}
