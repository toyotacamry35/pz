using System;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Aspects.Locomotion;
using JetBrains.Annotations;
using static Src.Locomotion.DebugTag;
using static Src.Locomotion.LocomotionDebug;

namespace Src.Locomotion
{
    public class LocomotionDestuckerNode : ILocomotionPipelinePassNode
    {
        private readonly ISettings _settings;
        private readonly ILocomotionBody _body;

        public LocomotionDestuckerNode(ISettings settings, ILocomotionBody body)
        {
            _settings = settings;
            _body = body ?? throw new ArgumentNullException(nameof(body));
        }
        
        public bool IsReady => true;
        
        public LocomotionVariables Pass(LocomotionVariables vars, float dt)
        {
            if (!vars.Flags.Any(LocomotionFlags.Teleport))
            {
                var distanceSq = (vars.Position - _body.Position).SqrMagnitude;
                if (distanceSq > SharedHelpers.Sqr(_settings.Threshold))
                {
                    vars.Flags |= LocomotionFlags.Teleport;
                    DebugAgent.Set(Destucker, 1);
                }
            }
            return vars;
        }
        
        public interface ISettings
        {
            float Threshold { get; }
        }
    }
}