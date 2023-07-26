using System;
using System.Collections.Generic;
using Assets.Src.SpatialSystem;
using SharedCode.Aspects.Regions;
using SharedCode.Entities;

namespace Assets.ColonyShared.GeneratedCode.Regions
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class IndexTextureRegion : BaseRectIndexRegion
    {
        private string LogName => $"{GetType().Name}";

        private IndexTextureRegionDef _indexTextureRegionDef;

        private SVO _svo;

        public override void InitRegionWithDef(ARegionDef def, Transform providedTransform = default)
        {
            base.InitRegionWithDef(def, providedTransform);

            _indexTextureRegionDef = def as IndexTextureRegionDef;

            if (_indexTextureRegionDef == null)
                throw new Exception($"Error {LogName} No Region Def");

            _svo = _indexTextureRegionDef.TexData.Target.Value;
        }

        public override short GetIndexFromRectAt(int x, int y)
        {
            return _svo.GetAt(x, y);
        }

        public override IEnumerable<IndexBlock> GetIndexBlocks()
        {
            return _svo.GetIndexBlocks();
        }
    }
}