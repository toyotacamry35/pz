using Assets.Src.Lib.Cheats;
using UnityEngine;

namespace Src.UI.Lobby
{
    /// <summary>
    /// Вешается на объект в сцене Gui_1_LobbyDev, которая убирается из билдов предназначенных для внешнего тестирования.
    /// В результате чего, в этих билдах НЕ активируются клиентские читы. 
    /// </summary>
    public class ClientCheatsEnabler : MonoBehaviour
    {
        [SerializeField] private bool _fly = true;
        [SerializeField] private bool _spectator = true;
        [SerializeField] private bool _debugInfo = true;
        [SerializeField] private bool _timeofday = true;
        [SerializeField] private bool _chat = true;

        void Start()
        {
            ClientCheatsState.Fly = _fly;
            ClientCheatsState.Spectator = _spectator;
            ClientCheatsState.DebugInfo = _debugInfo;
            ClientCheatsState.TimeOfDay = _timeofday;
            ClientCheatsState.Chat = _chat;
        }
    }
}