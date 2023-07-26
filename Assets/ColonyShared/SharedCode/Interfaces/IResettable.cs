
namespace Assets.ColonyShared.SharedCode.Interfaces
{
    public interface IResettable
    {
        void Reset();
    }

    public interface IStopAndRestartable
    {
        void Stop();
        void Restart();
    }
}
