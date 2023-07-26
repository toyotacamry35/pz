using NLog;
using SharedCode.EntitySystem;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class BakenCharacterEntity
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public ValueTask<bool> BakenCanBeActivatedImpl(OuterRef<IEntity> bakenRef)
        {
            return new ValueTask<bool>(ActiveBaken != bakenRef && RegisteredBakens.ContainsKey(bakenRef));
        }

        public ValueTask<bool> ActivateBakenImpl(OuterRef<IEntity> bakenRef)
        {
            if (RegisteredBakens.ContainsKey(bakenRef))
            {
                ActiveBaken = bakenRef;
                return new ValueTask<bool>(true);
            }
            else
                Logger.IfError()?.Message("User {0} has no registered baken {1}", CharacterId, bakenRef).Write();

            return new ValueTask<bool>(false);
        }

        public async ValueTask RegisterBakenImpl(OuterRef<IEntity> bakenRef, bool loaded)
        {
            RegisteredBakens[bakenRef] = loaded;

            if (loaded && ActiveBaken == default)
                await ActivateBaken(bakenRef);
        }

        public ValueTask SetCharacterLoadedImpl(bool loaded)
        {
            CharacterLoaded = loaded;
            return new ValueTask();
        }

        public ValueTask SetLoginImpl(bool logined)
        {
            Logined = logined;
            return new ValueTask();
        }

        public ValueTask BakenIsDestroyedImpl(OuterRef<IEntity> bakenRef)
        {
            if(!RegisteredBakens.Remove(bakenRef))
                Logger.IfError()?.Message("User {0} has no registered baken {1}", CharacterId, bakenRef).Write();

            if (ActiveBaken == bakenRef)
                ActiveBaken = default;

            return new ValueTask();
        }


        public ValueTask<bool> CanBeUnloadedImpl()
        {
            return new ValueTask<bool>(!Logined && !CharacterLoaded && RegisteredBakens.Values.All(v => !v));
        }
    }
}
