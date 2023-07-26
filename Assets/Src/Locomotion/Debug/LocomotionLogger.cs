using System;
using System.Collections.Generic;
using Core.Environment.Logging;
using Core.Environment.Logging.Extension;
using NLog;
using static Src.Locomotion.LocomotionDebug;

namespace Src.Locomotion
{
    public readonly struct LocomotionLogger
    {
        private readonly Logger _logger;

        public static readonly LocomotionLogger Default = GetLogger(nameof(Locomotion));

        public static readonly Action<IDictionary<object, object>> _entityIdKeyApplier =
            p => p.Add(EntityIdLayoutRenderer.EntityIdKey, EntityId);

        public static LocomotionLogger GetLogger(string id)
        {
            return new LocomotionLogger (LogManager.GetLogger(id));
        }

        public LocomotionLogger(Logger logger)
        {
            _logger = logger;
        }

        public Level? IfError()
        {
            return _logger.IfError(_entityIdKeyApplier);
        }
        
        public Level? IfWarn()
        {
            return _logger.IfWarn(_entityIdKeyApplier);
        }
        
        public Level? IfInfo()
        {
            return _logger.IfInfo(_entityIdKeyApplier);
        }
        
        public Level? IfDebug()
        {
            return _logger.IfDebug(_entityIdKeyApplier);
        }
        
        public Level? IfTrace()
        {
            return _logger.IfTrace(_entityIdKeyApplier);
        }
        
        public bool IsDebugEnabled => _logger.IsDebugEnabled;
        
        public bool IsTraceEnabled => _logger.IsTraceEnabled;
    }
}