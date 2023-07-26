using System.Collections.Generic;
using ColonyShared.SharedCode.Utils;

namespace Assets.Src.Detective
{
    public class Archive
    {
        private readonly int _idsCount;
        public Archive(int idsCount)
        {
            _idsCount = idsCount;
        }
        Dictionary<ArchiveId, Investigation> _finishedInvestigations = new Dictionary<ArchiveId, Investigation>();
        Queue<ArchiveId> _ids = new Queue<ArchiveId>();
        public void Put(Investigation inv)
        {
            lock(this)
            {

                if (_finishedInvestigations.Count > _idsCount)
                {
                    var earliestId = _ids.Dequeue();
                    _finishedInvestigations.Remove(earliestId);
                }
                var id = new ArchiveId() { Time = SyncTime.Now };
                _finishedInvestigations[id] = inv;
                _ids.Enqueue(id);
            }
        }

        List<KeyValuePair<ArchiveId, Investigation>> _list = new List<KeyValuePair<ArchiveId, Investigation>>();
        public List<KeyValuePair<ArchiveId, Investigation>> GetInvestigations()
        {
            lock(this)
            {
                _list.Clear();
                foreach (var i in _finishedInvestigations)
                    _list.Add(new KeyValuePair<ArchiveId, Investigation>(i.Key, i.Value));
                _list.Sort((x, y) => -1 * x.Key.Time.CompareTo(y.Key.Time));
                return _list;
            }
        }
    }
    

    public class ArchiveId
    {
        public long Time;
    }

}
