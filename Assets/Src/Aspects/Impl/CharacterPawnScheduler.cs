using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.Aspects.Impl;
using Assets.Src.Camera;
using SharedCode.Aspects.Item.Templates;
using UnityEngine;
using UnityUpdate;
using Random = UnityEngine.Random;

namespace Src.Aspects.Impl
{
    public class CharacterPawnScheduler
    {
        private readonly List<Tuple> _characters = new List<Tuple>();

        public void RegisterCharacter(CharacterPawn character)
        {
            if (!character) throw new ArgumentException(nameof(character));
            if (_characters.All(x => x.Character != character))
            {
                _characters.Add(new Tuple(character) { Throttle = CalculateThrottle(character) * Random.value });
                if (_characters.Count == 1)
                    Activate();
            }
        }

        public void UnregisterCharacter(CharacterPawn character)
        {
            if (!character) throw new ArgumentException(nameof(character));
            if (_characters.RemoveAll(x => x.Character == character) > 0 && _characters.Count == 0)
                Deactivate();
        }

        private void Activate()
        {
            UnityUpdateDelegate.OnUpdate += Update;
        }

        private void Deactivate()
        {
            UnityUpdateDelegate.OnUpdate -= Update;
        }

        
        private void Update()
        {
            for (var i = _characters.Count - 1; i >= 0; --i)
            {
                var tuple = _characters[i];
                if (!tuple.Character)
                {
                    _characters.RemoveAt(i);
                    continue;
                };

                if (tuple.Throttle < Time.deltaTime)
                {
                    tuple.Character.DoUpdate(Time.time - tuple.LastUpdateTime);
                    tuple.Throttle += CalculateThrottle(tuple.Character);
                    tuple.LastUpdateTime = Time.time;
                }
                else
                    tuple.Throttle -= Time.deltaTime;
            }
            
            if (_characters.Count == 0) 
                Deactivate();
        }

        private float CalculateThrottle(CharacterPawn pawn)
        {
            if (pawn.HasAuthority)
                return 0;
            var settings = Constants.WorldConstants.CharacterPawnScheduler.Target;
            var distance = CalculateObserverDistance(pawn.transform);
            var delayByDistance = Mathf.Lerp(0, settings.ThrottleOnFarDistance,   Mathf.InverseLerp(settings.NearDistance, settings.FarDistance, distance));
            var delayByQuantity = Mathf.Lerp(0, settings.ThrottleOnMaxCharacters, Mathf.InverseLerp(settings.MinCharacters, settings.MaxCharacters, _characters.Count));
            return delayByDistance * delayByQuantity;
        }
        
        private float CalculateObserverDistance(Transform xform)
        {
            return GameCamera.Camera ? Vector3.Distance(GameCamera.Camera.transform.position, xform.position) : 0;
        }
        
        class Tuple
        {
            public readonly CharacterPawn Character;
            public float Throttle;
            public float LastUpdateTime;

            public Tuple(CharacterPawn character)
            {
                Character = character;
            }
        }
    }
}