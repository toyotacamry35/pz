using System.Threading.Tasks;

namespace SharedCode.EntitySystem
{
    public interface IHookOnInit
    {
        Task OnInit();
    }

    public interface IHookOnDatabaseLoad
    {
        Task OnDatabaseLoad();
    }

    public interface IHookOnStart
    {
        Task OnStart();
    }


    public interface IHookOnUnload
    {
        Task OnUnload();
    }

    public interface IHookOnDestroy
    {
        Task OnDestroy();
    }


    public interface IHookOnReplicationLevelChanged
    {
        void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask);
    }

}
