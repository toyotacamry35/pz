using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.Aspects.Impl.Factions.Template;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class CharacterPositionInformation
    {
        public Task<bool> SetPositionImpl(Vector3 position)
        {
            Position = position;
            return Task.FromResult(true);
        }

        public Task<bool> SetMutationImpl(MutationStageDef mutation)
        {
            Mutation = mutation;
            return Task.FromResult(true);
        }
    }
}
