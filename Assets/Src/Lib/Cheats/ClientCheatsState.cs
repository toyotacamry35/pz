namespace Assets.Src.Lib.Cheats
{
    public static class ClientCheatsState
    {
        /// <summary>
        /// Режим читерского полёта активируемгого по Num-0
        /// </summary>
        public static bool Fly { get; set; }
        
        /// <summary>
        /// Режим свободной камеры активируемгого по F5
        /// </summary>
        public static bool Spectator { get; set; }

        /// <summary>
        /// Возможность подкручивать время у освещения
        /// </summary>
        public static bool TimeOfDay { get; set; }

        /// <summary>
        /// возможность включать отладочную инфу по Shift+Ctrl+'+"
        /// </summary>
        public static bool DebugInfo { get; set; }
        
        /// <summary>
        /// Использование читов из чата
        /// </summary>
        public static bool Chat { get; set; }
    }
}
