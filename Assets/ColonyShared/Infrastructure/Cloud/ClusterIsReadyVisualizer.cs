﻿using System;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.Infrastructure.Cloud
{
    public static class ClusterIsReadyVisualizer
    {
        private static readonly NLog.Logger ConsoleLogger = NLog.LogManager.GetLogger("Console");

        public static void PrintClusterIsReadyToConsole()
        {
            var random = new Random((int)DateTime.Now.Ticks);
            switch (random.Next() % 30)
            {
                case 0:
                {
                    ConsoleLogger.IfInfo()?.Message(@"
Population Zero Z
Can you survive? - Zombies, Dinosaurus, Неподъемные бонусы, Корь !

          ████████_____██████
         █░░░░░░░░██_██░░░░░░█
        █░░░░░░░░░░░█░░░░░░░░░█
       █░░░░░░░███░░░█░░░░░░░░░█
       █░░░░███░░░███░█░░░████░█
      █░░░██░░░░░░░░███░██░░░░██
     █░░░░░░░░░░░░░░░░░█░░░░░░░░███
    █░░░░░░░░░░░░░██████░░░░░████░░█
    █░░░░░░░░░█████░░░████░░██░░██░░█
   ██░░░░░░░███░░░░░░░░░░█░░░░░░░░███
  █░░░░░░░░░░░░░░█████████░░█████████
 █░░░░░░░░░░█████ ████   ████ █████  █
 █░░░░░░░░░░█     █ ███  █     ███  ██
█░░░░░░░░░░░░█   ████ ████    ██ ██████
░░░░░░░░░░░░░█████████░░░████████░░░█
░░░░░░░░░░░░░░░░█░░░░░█░░░░░░░░░░░░█
░░░░░░░░░░░░░░░░░░░░██░░░░█░░░░░░██
░░░░░░░░░░░░░░░░░░██░░░░░░░███████
░░░░░░░░░░░░░░░░██░░░░░░░░░░█░░░░░█
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░█
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░█
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░█
░░░░░░░░░░░█████████░░░░░░░░░░░░░░██
░░░░░░░░░░█▒▒▒▒▒▒▒▒███████████████▒▒█
░░░░░░░░░█▒▒███████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█
░░░░░░░░░█▒▒▒▒▒▒▒▒▒█████████████████
░░░░░░░░░░████████▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█
░░░░░░░░░░░░░░░░░░██████████████████
░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░█
██░░░░░░░░░░░░░░░░░░░░░░░░░░░██
▓██░░░░░░░░░░░░░░░░░░░░░░░░██
▓▓▓███░░░░░░░░░░░░░░░░░░░░█
▓▓▓▓▓▓███░░░░░░░░░░░░░░░██
▓▓▓▓▓▓▓▓▓███████████████▓▓█
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█
(Meanwhile, cluster is ready)
").Write();
                }
                    break;
                case 1:
                {
                    ConsoleLogger.IfInfo()?.Message(@"
░░░░░░░░░░░░░░░░░░░░
░░░░░░░░░░░░░░░░░░░░
░░░░░▄▀▀▀▄░░░░░░░░░
▄███▀░◐░░░▌░░░░░░░░░
░░░░▌░░░░░▐░░░░░░░░░
░░░░▐░░░░░▐░░░░░░░░░
░░░░▌░░░░░▐▄▄░░░░░░░
░░░░▌░░░░▄▀▒▒▀▀▀▀▄
░░░▐░░░░▐▒▒▒▒▒▒▒▒▀▀▄
░░░▐░░░░▐▄▒▒▒▒▒▒▒▒▒▒▀▄
░░░░▀▄░░░░▀▄▒▒▒▒▒▒▒▒▒▒▀▄
░░░░░░▀▄▄▄▄▄█▄▄▄▄▄▄▄▄▄▄▄▀▄
░░░░░░░░░░░▌▌░▌▌░░░░░
░░░░░░░░░░░▌▌░▌▌░░░░░
░░░░░░░░░▄▄▌▌▄▌▌░░░░░
(Meanwhile, cluster is ready)
").Write();
                }
                    break;
                default:
                {
                    ConsoleLogger.IfInfo()?.Message("========================").Write();
                    ConsoleLogger.IfInfo()?.Message("=== Cluster is ready ===").Write();
                    ConsoleLogger.IfInfo()?.Message("========================").Write();
                    break;
                }
            }
        }
    }
}
