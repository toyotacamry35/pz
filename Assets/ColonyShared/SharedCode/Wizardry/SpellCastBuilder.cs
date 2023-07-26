using System;
using System.Collections.Generic;
using ColonyShared.SharedCode.InputActions;
using ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using ResourceSystem.Utils;
using Scripting;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.Wizardry
{
    [Obsolete]
    public struct SpellCastBuilder
    {
        private SpellDef _spellDef;
        private bool _hasTarget;
        private OuterRef<IEntity> _target;  
        private bool _hasDirection3;
        private Vector3 _direction3;
        private bool _hasDirection2;
        private Vector2 _direction2;
        private bool _hasPosition3;
        private Vector3 _position3;
        private bool _hasLocalPosition3;
        private Vector3 _localPosition3;
        private InputActionDef _inputAction;
        private float _duration;
        private ScriptingContext _context;
        private bool _hasDuration;
        private SpellId _prevSpellId;
        private string _objectName;

        public SpellCastBuilder SetSpell(SpellDef def)
        {
            def.AssertIfNull(nameof(def));
            _spellDef = def;
            return this;
        }

        public SpellCastBuilder SetInputAction(InputActionDef inputAction)
        {
            _inputAction = inputAction;
            return this;
        }

        public SpellCastBuilder SetTarget(OuterRef<IEntity> target)
        {
            _hasTarget = true;
            _target = target;
            return this;
        }

        public SpellCastBuilder SetTarget(OuterRef<IEntityObject> target)
        {
            _hasTarget = true;
            _target = new OuterRef<IEntity>(target.Guid, target.TypeId);
            return this;
        }
        
        public SpellCastBuilder SetTarget(OuterRef target)
        {
            _hasTarget = true;
            _target = new OuterRef<IEntity>(target.Guid, target.TypeId);
            return this;
        }

        public SpellCastBuilder SetTargetIfValid(OuterRef<IEntity> target)
        {
            _hasTarget = target.IsValid;
            _target = target;
            return this;
        }

        public SpellCastBuilder SetTargetIfValid(OuterRef<IEntityObject> target)
        {
            _hasTarget = target.IsValid;
            _target = new OuterRef<IEntity>(target.Guid, target.TypeId);
            return this;
        }
        
        public SpellCastBuilder SetDirection2(Vector2 direction)
        {
            _hasDirection2 = true;
            _direction2 = direction;
            return this;
        }
        
        public SpellCastBuilder SetDirection2IfNotNull(Vector2? direction)
        {
            _hasDirection2 = direction.HasValue;
            if(direction.HasValue)
                _direction2 = direction.Value;
            return this;
        }

        public SpellCastBuilder SetDirection3(Vector3 direction)
        {
            _hasDirection3 = true;
            _direction3 = direction;
            return this;
        }

        public SpellCastBuilder SetDirection3IfNotNull(Vector3? direction)
        {
            _hasDirection3 = direction.HasValue;
            if(direction.HasValue)
                _direction3 = direction.Value;
            return this;
        }

        public SpellCastBuilder SetDirection(Vector3 direction) => SetDirection3(direction);

        public SpellCastBuilder SetDirectionIfNotNull(Vector3? direction) => SetDirection3IfNotNull(direction);

        
        public SpellCastBuilder SetPosition3(Vector3 position)
        {
            _hasPosition3 = true;
            _position3 = position;
            return this;
        }

        public SpellCastBuilder SetPosition3IfNotNull(Vector3? position)
        {
            _hasPosition3 = position.HasValue;
            if(position.HasValue)
                _position3 = position.Value;
            return this;
        }

        public SpellCastBuilder SetLocalPosition3(Vector3 position)
        {
            _hasLocalPosition3 = true;
            _localPosition3 = position;
            return this;
        }

        public SpellCastBuilder SetLocalPosition3IfNotNull(Vector3? position)
        {
            _hasLocalPosition3 = position.HasValue;
            if(position.HasValue)
                _localPosition3 = position.Value;
            return this;
        }
        
        public SpellCastBuilder SetPrevSpellId(SpellId spellId)
        {
            if (!spellId.IsValid) throw new ArgumentException(nameof(spellId));
            _prevSpellId = spellId;
            return this;
        }

        public SpellCastBuilder SetObjectName(string objectName)
        {
            _objectName = objectName;
            return this;
        }

        [Obsolete]
        public SpellCastBuilder SetParameters(IEnumerable<SpellParameterDef> @params, in SetParameterContext ctx)
        {
            if (@params != null)
                foreach (var param in @params)
                {
                    switch(param)
                    {
                        case SpellParameterDirection2Def p:
                            SetDirection2(new Vector2(p.X, p.Y));
                            break;
                        case SpellParameterDirection3Def p:
                            SetDirection3(new Vector3(p.X, p.Y, p.Z));
                            break;
                        case SpellParameterPrevSpellIdDef _:
                            SetPrevSpellId(ctx.PrevSpellId);
                            break;
                        case SpellParameterDurationDef d:
                            SetDuration(d.Duration);
                            break;
                        default:
                            throw new ArgumentException($"Unsupported parameter type {param.GetType()}");
                    }
                }
            return this;
        }

        public SpellCastBuilder SetDuration(float duration)
        {
            _hasDuration = true;
            _duration = duration >= 0 ? duration : throw new ArgumentException($"Invalid duration value: {duration}");
            return this;
        }
        public SpellCastBuilder AddContext(ScriptingContext ctx)
        {
            _context = ctx;
            return this;
        }

        [System.Diagnostics.Contracts.Pure]
        public SpellCast Build()
        {
            _spellDef.AssertIfNull(nameof(_spellDef));

            List<SpellCastParameter> @params = new List<SpellCastParameter>();
            if (_hasDuration)
                @params.Add(new SpellCastParameterDuration {DurationSeconds = _duration});
            if (_inputAction != null)
                @params.Add(new SpellCastParameterInputAction {InputAction = _inputAction});
            if (_hasDirection2)
                @params.Add(new SpellCastParameterDirection2 {Direction = _direction2});
            if (_hasPosition3)
                @params.Add(new SpellCastParameterPosition {Position = _position3});
            if (_hasLocalPosition3)
                @params.Add(new SpellCastParameterLocalPosition {Position = _localPosition3});
            if (_prevSpellId.IsValid)
                @params.Add(new SpellCastParameterPrevSpellId {SpellId = _prevSpellId});
            if (_objectName != null)
                @params.Add(new SpellCastParameterObjectName { Name = _objectName });
            
            // пока оставлено так для совместимости
            if(_hasTarget && _hasDirection3)
                return new SpellCastWithTargetAndDirection(@params) { Def = _spellDef, Target = _target, Direction = _direction3 };
            if(_hasTarget)
                return new SpellCastWithTarget(@params) { Def = _spellDef, Target = _target };
            if(_hasDirection3)
                return new SpellCastWithDirection(@params) { Def = _spellDef, Direction = _direction3 };
            
            return @params.Count > 0 ? new SpellCastWithParameters { Def = _spellDef, Parameters = @params.ToArray(), Context = _context } : new SpellCast { Def = _spellDef, Context = _context };
        }

        public override string ToString()
        {
            var sb = StringBuildersPool.Get;
            sb.Append("Spell:").Append(_spellDef?.____GetDebugAddress() ?? "null");
            if (_hasTarget)
                sb.Append(" Target:").Append(_target);
            if (_hasDirection3)
                sb.Append(" Dir:").Append(_direction3);
            if (_inputAction != null)
                sb.Append(" Action:").Append(_inputAction.ActionToString());
            if (_hasDuration)
                sb.Append(" Duration:").Append(_duration.ToString("F2"));
            return sb.ToStringAndReturn();
        }

        public readonly struct SetParameterContext
        {
            public readonly SpellId PrevSpellId;

            public SetParameterContext(SpellId prevSpellId)
            {
                PrevSpellId = prevSpellId;
            }
        }
    }
}