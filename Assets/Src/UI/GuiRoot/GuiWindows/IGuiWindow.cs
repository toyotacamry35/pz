using ColonyShared.SharedCode.Input;
using System;
using ReactivePropsNs;

namespace Uins
{
    public interface IGuiWindow
    {
        /// <summary>
        /// уникальный id окна, задается в окне
        /// </summary>
        WindowId Id { get; }

        /// <summary>
        /// id стека, в котором открыто окно, задается в только в WindowsManager
        /// </summary>
        WindowStackId CurrentWindowStack { get; set; }

        /// <summary>
        /// состояние окна, задается в только в WindowsManager
        /// </summary>
        ReactiveProperty<GuiWindowState> State { get; set; }

        /// <summary>
        /// Может ли окно быть закрыто (актуально для окон нижнего уровня, например, HUD)
        /// </summary>
        bool IsUnclosable { get; }

        /// <summary>
        /// Блокировки и/или переопределения привязок клавиш к командам активируемые при открытии данного окна
        /// </summary>    
        InputBindingsDef InputBindings { get; }

        /// <summary>
        /// Callback открытия окна (State Closed -> Open)
        /// </summary>
        void OnOpen(object arg);

        /// <summary>
        /// Callback закрытия окна (State Open -> Closed)
        /// </summary>
        void OnClose();

        /// <summary>
        /// Callback заслонения открытого окна другим открывающимся в этом стеке (State Open -> Faded)
        /// </summary>
        void OnFade();

        /// <summary>
        /// Callback снятия заслонения окна, предыдущее окно закрывается (State Faded -> Open)
        /// </summary>
        void OnUnfade();

        /// <summary>
        /// Update-callback открытого окна
        /// </summary>
        void OpenUpdate();

        /// <summary>
        /// Update-callback не закрытого окна
        /// </summary>
        void NoClosedUpdate();

        /// <summary>
        /// Update-callback закрытого окна - слушать, не должно ли оно быть открыто.
        /// Вызывается не в WindowsManager, а в других смежных окнах, если из них это окно может быть открыто. 
        /// Типовую реализацию см. в InventoryNode
        /// </summary>
        void ClosedHotkeyUpdate(Action additionalAction = null);
    }

    public static class GuiWindowHelper
    {
        public static string GuiWindowToString(IGuiWindow guiWindow)
        {
            return $"{nameof(IGuiWindow)} <{guiWindow.GetType()}> {nameof(guiWindow.Id)}={guiWindow.Id}, " +
                   $"{nameof(guiWindow.CurrentWindowStack)}={guiWindow.CurrentWindowStack}, {nameof(guiWindow.State)}={guiWindow.State}" +
                   (guiWindow.IsUnclosable ? ", " + nameof(guiWindow.IsUnclosable) : "");
        }
    }
}