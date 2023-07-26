using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Assets.Src.Aspects.SpatialObjects
{
    public class AABBTriggeredCoordinator
    {
        private static AABBTriggeredCoordinator instance;

        public static AABBTriggeredCoordinator Instance {
            get
            {
                if (instance == null)
                    instance = new AABBTriggeredCoordinator();
                return instance;
            }
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private List<IndexOfNoEntityInteractiveObjects> indexes = new List<IndexOfNoEntityInteractiveObjects>();

        public void AddIndex(IndexOfNoEntityInteractiveObjects index)
        {
            if (!indexes.Contains(index))
                indexes.Add(index);
            else
                Logger.IfWarn()?.Message("{0} already has index '{1}'", nameof(AABBTriggeredCoordinator), index).Write();
        }
        public void RemoveIndex(IndexOfNoEntityInteractiveObjects index)
        {
            if (!indexes.Contains(index))
                Logger.IfWarn()?.Message("{0} does not have index '{1}'", nameof(AABBTriggeredCoordinator), index).Write();
            else
                indexes.Remove(index);
        }

        public void SetPosition(Vector3 position)
        {
            foreach (var index in indexes)
                index.SetTriggeredForNearestCells(position);
        }
    }
}