using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Regions;
using GeneratedCode.DeltaObjects;
using SharedCode.Entities;

namespace Assets.Src.Arithmetic
{
	public static class CalcerRegionsHelper
	{
		public static ValueTask<ICollection<IRegion>> GetCurrentRegions(CalcerContext ctx)
		{
			var positionedEntity = PositionedObjectHelper.GetPositioned(ctx.EntityContainer, ctx.EntityRef.TypeId, ctx.EntityRef.Guid);
			if (positionedEntity == null)
				return new ValueTask<ICollection<IRegion>>(Array.Empty<IRegion>());

			var ownerScene = ctx.Repository.GetSceneForEntity(ctx.EntityRefGeneric);
			if (ownerScene == default)
				return new ValueTask<ICollection<IRegion>>(Array.Empty<IRegion>());

			var rootRegion = RegionsHolder.GetRegionByGuid(ownerScene);
			var regions = new List<IRegion>();
			rootRegion?.GetAllContainingRegionsNonAlloc(regions, positionedEntity.Position);
			return new ValueTask<ICollection<IRegion>>(regions);
		}
	}
}