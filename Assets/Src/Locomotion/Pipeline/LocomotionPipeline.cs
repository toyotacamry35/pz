using System;
using JetBrains.Annotations;

namespace Src.Locomotion
{
    public class LocomotionPipeline : ILocomotionComposablePipeline, ILocomotionPipelineCommitNode
    {
        static readonly LocomotionLogger Logger = LocomotionLogger.GetLogger(nameof(LocomotionEngine));
        private const int SizeIncrement = 4;

        private ILocomotionPipelineCommitNode[] _nodes = null;
        private int _nodesCount = 0;
        private bool _ready;

        public ILocomotionComposablePipeline AddPass(ILocomotionPipelinePassNode node, Action<ILocomotionComposablePipeline> builder)
        {
            //Logger.IfDebug()?.Message("Add pass node: {0}", node?.GetType()).Write();
            if (node != null)
            {
                // a) Cre. subpipeline with `node` as the only `_pass` node:
                var pp = new PassComposablePipeline(node);
                // b) Compose cre-ed subpipeline by passed instruction:
                builder.Invoke(pp);
                // c) Add cre-ed & composed subpipeline to curr.root:
                AddNode(pp);
            }
            return this;
        }

        public ILocomotionComposablePipeline AddPassIf(bool predicate, ILocomotionPipelinePassNode node, Action<ILocomotionComposablePipeline> builder)
        {
            if (predicate)
                AddPass(node, builder);
            else
                builder.Invoke(this);
            return this;
        }

        public ILocomotionComposablePipeline AddCommit(ILocomotionPipelineCommitNode node)
        {
            //Logger.IfDebug()?.Message("Add commit node: {0}", node?.GetType()).Write();
            if(node != null && (_nodes == null || Array.IndexOf(_nodes, node)==-1))
                AddNode(node);
            return this;
        }

        public ILocomotionComposablePipeline AddCommitIf(bool prdicate, ILocomotionPipelineCommitNode node)
        {
            //Logger.IfDebug()?.Message("Add commit node: {0}", node?.GetType()).Write();
            if (prdicate)
                AddCommit(node);
            return this;
        }
        
        bool ILocomotionPipelineCommitNode.IsReady
        {
            get
            {
                if (!_ready)
                {
                    for (int i = 0; i < _nodesCount; ++i)
                        if (!_nodes[i].IsReady)
                            return false;
                    _ready = true;
                }
                return true;
            }
        }

        //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
        // So DO NOT change `inVars`!
        void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt)
        {
            //#Note: no-need, but there was 2.5msec time at profiler at some step of mutual calls
            // if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample("## Loco Commit: f)LocoPipeline");

            for (int i = 0; i < _nodesCount; ++i)
                _nodes[i].Commit(ref inVars, dt);

            // LocomotionProfiler.EndSample();
        }
        

        public void Clear()
        {
            for (int i = 0; i < _nodesCount; ++i)
                (_nodes[i] as IDisposable)?.Dispose();
            _nodes = null;
            _nodesCount = 0;
            _ready = false;
        }
 
        public void Dispose()
        {
            Clear();
        }

        private void AddNode(ILocomotionPipelineCommitNode node)
        {
            ++_nodesCount;
            if (_nodes == null || _nodesCount > _nodes.Length)
                Array.Resize(ref _nodes, (_nodesCount / SizeIncrement + 1) * SizeIncrement);
            _nodes[_nodesCount - 1] = node;
        }
 
        private class PassComposablePipeline : ILocomotionComposablePipeline, ILocomotionPipelineCommitNode
        {
            private readonly ILocomotionPipelinePassNode _pass;
            // Subpipeline as commit-node:
            private readonly ILocomotionPipelineCommitNode _commit;
            // Subpipeline as ComposablePipeline:
            private readonly LocomotionPipeline _pipeline;
            
            //#Dbg:
            private readonly string _Dbg_beginSampleString;

            public PassComposablePipeline([NotNull] ILocomotionPipelinePassNode pass)
            {
                if (pass == null) throw new ArgumentNullException(nameof(pass));
                _pipeline = new LocomotionPipeline();
                _commit = _pipeline;
                _pass = pass;
                _Dbg_beginSampleString = $"## Loco Commit: g)PassPipeline [[`{_pass.GetType().Name}`]]";
            }

            ILocomotionComposablePipeline ILocomotionComposablePipeline.AddPass(ILocomotionPipelinePassNode node, Action<ILocomotionComposablePipeline> builder)
            {
                _pipeline.AddPass(node, builder);
                return this;
            }

            ILocomotionComposablePipeline ILocomotionComposablePipeline.AddPassIf(bool predicate, ILocomotionPipelinePassNode node, Action<ILocomotionComposablePipeline> builder)
            {
                _pipeline.AddPassIf(predicate, node, builder);
                return this;
            }
            
            ILocomotionComposablePipeline ILocomotionComposablePipeline.AddCommit(ILocomotionPipelineCommitNode node)
            {
                if (node != null)
                    _pipeline.AddCommit(node);
                return this;
            }

            ILocomotionComposablePipeline ILocomotionComposablePipeline.AddCommitIf(bool predicate, ILocomotionPipelineCommitNode node)
            {
                _pipeline.AddCommitIf(predicate, node);
                return this;
            }
            
            bool ILocomotionPipelineCommitNode.IsReady => _pass.IsReady && _commit.IsReady;

            //#Important!: @param `inVars`: 'ref' === 'in' here - i.e. "const-ref" (only to avoid struct copying)
            // So DO NOT change `inVars`!
            void ILocomotionPipelineCommitNode.Commit(ref LocomotionVariables inVars, float dt) ///=> _commit.Commit(_pass.Pass(vars, dt), dt);
            {
                //if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample($"## Loco Commit: g)PassPipeline [[`{_Dbg_passTypeName}`]]");
                if (LocomotionProfiler.EnableProfile) LocomotionProfiler.BeginSample(_Dbg_beginSampleString);

                var varsCopy = inVars;
                var modifiedVars = _pass.Pass(varsCopy, dt);
                _commit.Commit(ref modifiedVars, dt);

                LocomotionProfiler.EndSample();
            }

            void IDisposable.Dispose() => _pipeline.Dispose();
        }
    }
}