namespace Src.Locomotion
{
    public interface ILocomotionNetworkSend 
    {
        void Acquire();
        
        void Release();
        
        bool IsReady { get; }
        /// <summary>
        /// Передать фрейм перемещения
        /// </summary>
        void SendFrame(LocomotionVariables frame, bool important, ReasonForSend reason);
    }

    public interface ILocomotionNetworkReceive
    {
        /// <summary>
        /// Подписаться на получение данных
        /// </summary>
        void Acquire(LocomotionNetworkReceiveDelegate @delegate);
        
        void Release(LocomotionNetworkReceiveDelegate @delegate);
        
        bool IsReady { get; }
    }

    public delegate void LocomotionNetworkReceiveDelegate(LocomotionVariables frame);
}
