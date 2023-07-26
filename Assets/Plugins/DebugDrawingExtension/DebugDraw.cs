using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static DebugExtension;

namespace Plugins.DebugDrawingExtension
{
    public partial class DebugDraw
    {
        private readonly string _facility;
        private DebugDrawAdapter _adapter;
        private DebugDrawLevel _level;

        public bool IsDebugEnabled => _level >= DebugDrawLevel.Debug && DebugExtension.Draw;

        public bool IsTraceEnabled => _level >= DebugDrawLevel.Trace && DebugExtension.Draw;

        public DebugDrawAdapter Debug => IsDebugEnabled ? _adapter : null;

        public DebugDrawAdapter Trace => IsTraceEnabled ? _adapter : null;
        
        private DebugDraw(string facility, Dictionary<Regex, DebugDrawConfig.Rule> rules)
        {
            _facility = facility ?? throw new ArgumentNullException(nameof(facility));
            ApplyRules(rules);
        }
        
        private void ApplyRules(Dictionary<Regex, DebugDrawConfig.Rule> rules)
        {
            _level = DebugDrawLevel.None;
            _adapter = null;
            if (rules != null)
                foreach (var rule in rules)
                    if (rule.Key.IsMatch(_facility))
                    {
                        _level = rule.Value.Level;
                        _adapter = new DebugDrawAdapter(rule.Value);
                        break;
                    }
        }
    }

    public class DebugDrawAdapter
    {
        private readonly DebugDrawConfig.Rule _config;

        internal DebugDrawAdapter(DebugDrawConfig.Rule config)
        {
            _config = config;
        }
        
        public DebugDrawAdapter Point(Vector3 position, Color color, float scale = 1.0f, float duration = -1)
        {
            DebugPoint(position, color, scale, Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Point(Vector3 position, float scale = 1.0f, float duration = -1)
        {
            DebugPoint(position, scale,Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Line(Vector3 bgn, Vector3 end, Color color, float duration = -1)
        {
            DebugLine(bgn, end, color, Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Arrow(Vector3 bgn, Vector3 end, Color color, float duration = -1)
        {
            DebugArrow(bgn, end, color, Duration(duration), _config.DepthTest);
            return this;
        }
        
        public DebugDrawAdapter Circle(Vector3 center, float radius, Color color, float duration = -1)
        {
            DebugCircle(center, color, radius, Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Circle(Vector3 center, Vector3 up, float radius, Color color, float duration = -1)
        {
            DebugCircle(center, up, color, radius, Duration(duration), _config.DepthTest);
            return this;
        }
        
        public DebugDrawAdapter Bounds(Bounds bounds, float duration = -1)
        {
            DebugBounds(bounds, Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Bounds(Bounds bounds, Color color, float duration = -1)
        {
            DebugBounds(bounds, color, Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Box(Vector3 center, Vector3 extends, Quaternion rotation, Color color, float duration = -1)
        {
            DebugBox(center, extends, rotation, color, Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Sphere(Vector3 position, float radius, float duration = -1)
        {
            DebugWireSphere(position, radius, Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Sphere(Vector3 position, float radius, Color color, float duration = -1)
        {
            DebugWireSphere(position, color, radius, Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Capsule(Vector3 start, Vector3 end, float radius, Color color, float duration = -1)
        {
            DebugCapsule(start, end, color, radius, Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Collider(Collider collider, Vector3 position, Quaternion rotation, Color color, float duration = -1)
        {
            DrawCollider(collider, position, rotation, color, Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Collider(Collider collider, Vector3 position, Quaternion rotation, Color color, bool positionIsCenter, float duration = -1)
        {
            DrawCollider(collider, position, rotation, color, positionIsCenter, Duration(duration), _config.DepthTest);
            return this;
        }

        public DebugDrawAdapter Collider(Collider collider, Vector3 position, Quaternion rotation, Vector3 scale, Color color, bool positionIsCenter = false, float duration = -1)
        {
            DrawCollider(collider, position, rotation, scale, color, positionIsCenter, Duration(duration), _config.DepthTest);
            return this;
        }
        
        private float Duration(float overrideDuration) => overrideDuration > 0 ? overrideDuration : _config.Duration;
    }
}