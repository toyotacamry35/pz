using System;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;

namespace Uins
{
    public class MemorySpender
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static Random rnd = new Random();

        private static MemorySpender instance;

        public long TotalSpended { get; private set; }
        public float lastTime;
        public bool spending = false;
        /// <summary> Количество экземпляров фигни генерящихся в секунду. </summary>
        public int MemorySpendingSpeed = 1000;

        private bool currentLevelCompleted = false;
        private MemorySpender[] children = new MemorySpender[10];
        private long[] payload = new long[1024];

        public MemorySpender()
        {
            if (instance != null)
                instance = this;
        }

        private void Spend()
        {
            if (!currentLevelCompleted)
                for (int i = 0; i < children.Length; i++)
                    if (children[i] == null)
                    {
                        children[i] = new MemorySpender();
                        if (i + 1 == children.Length)
                            currentLevelCompleted = true;
                        return;
                    }
            children[rnd.Next(children.Length)].Spend();
        }

        public void StartSpending(float time)
        {
            lastTime = time;
            spending = true;
        }
        public void StopSpending()
        {
            spending = false;
        }
        public void Spending(float time)
        {
            if (spending)
            {
                int mustSpend = (int)Math.Floor((time - lastTime) * MemorySpendingSpeed);
                if (mustSpend > 0) {
                    lastTime = time;
                    long memoryBefore = GC.GetTotalMemory(false);
                    for (int i = 0; i < mustSpend; i++)
                        Spend();
                    long deltaMemory = GC.GetTotalMemory(false) - memoryBefore;
                    if (deltaMemory >= 0)
                    {
                        TotalSpended += deltaMemory;
                        if (TotalSpended / (10 * 1024 * 1024) != (TotalSpended - deltaMemory) / (10 * 1024 * 1024))
                            Logger.IfError()?.Message($"{GetType().NiceName()} SPEND: {TotalSpended / (1024 * 1024)}Mb").Write();
                    }
                    else
                    {
                        Logger.IfError()?.Message($"Memory was collected during spending").Write();
                    }
                }
            }
        }
    }
}
