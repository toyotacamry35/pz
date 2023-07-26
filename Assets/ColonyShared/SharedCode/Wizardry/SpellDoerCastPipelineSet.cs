using System.Collections.Generic;

namespace SharedCode.Wizardry
{
    public class SpellDoerCastPipelineSet
    {
        private readonly List<ISpellDoerCastPipeline> _pipelines = new List<ISpellDoerCastPipeline>();

        public void Add(ISpellDoerCastPipeline pipeline)
        {
            _pipelines.Add(pipeline);
        }

        public void StopCast()
        {
            foreach (var pipeline in _pipelines)
                pipeline.StopCast();
            _pipelines.Clear();
        }
    }
}