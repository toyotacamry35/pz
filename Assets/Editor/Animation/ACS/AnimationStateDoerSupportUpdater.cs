using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Src.Animation.ACS;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Editor.Animation.ACS
{
    [UsedImplicitly]
    public class AnimationStateDoerSupportUpdater : AnimationStateComponentUpdater<AnimationStateDoerSupport>
    {
        protected override void Update(AnimationStateComponentUpdaterContext ctx, AnimationStateDoerSupport comp, AnimatorState state, AnimatorStateMachine machine, int layer)
        {
            var speedParameter = CreateSpeedParameterIfNeed(ctx.AnimationController, state, layer, comp.SpeedParameter);
            if(comp.SetSpeedParameter(speedParameter))
                comp.Dirty();

            var runParameter = CreateRunParameterIfNeed(ctx.AnimationController, state, layer, comp.RunParameter);
            if(comp.SetRunParameter(runParameter))
                comp.Dirty();

            var stayParameter = CreateStayParameterIfNeed(ctx.AnimationController, state, layer, comp.StayParameter);
            if(comp.SetStayParameter(stayParameter))
                comp.Dirty();

            var defaultState = machine.defaultState.GetOrCreateComponent<AnimationStateProfile>();
            if(comp.SetDefaultState(defaultState.Header))
                comp.Dirty();

            CreateTransitions(ctx.AnimationController, layer, state, runParameter, stayParameter);
        }
        
        private static string CreateSpeedParameterIfNeed(AnimatorController ac, AnimatorState state, int layer, string oldParamName)
        {
            var paramName = $"@{ac.layers[layer].name}.{state.name} #speed";

            var oldParam = ac.parameters.FirstOrDefault(x => x.name == oldParamName);
            if (oldParam != null && oldParamName != paramName)
            {
                ac.RemoveParameter(oldParam);
                ac.Dirty();
            }
            
            var paramIdx = Array.FindIndex(ac.parameters, x => x.name == paramName);
            if (paramIdx == -1 || ac.parameters[paramIdx].type != AnimatorControllerParameterType.Float)
            {
                if (paramIdx != -1)
                    paramName = ac.MakeUniqueParameterName(paramName);
                ac.AddParameter(new AnimatorControllerParameter{ name = paramName, type = AnimatorControllerParameterType.Float, defaultFloat = 1});
                ac.Dirty();
            }

            paramIdx = Array.FindIndex(ac.parameters, x => x.name == paramName);
            if (!Mathf.Approximately(ac.parameters[paramIdx].defaultFloat, 1))
            {
                var newParams = ac.parameters.ToArray(); 
                newParams[paramIdx].defaultFloat = 1;
                ac.parameters = newParams;
                ac.Dirty();
            }

            if (!state.speedParameterActive)
            {
                state.speedParameterActive = true;
                ac.Dirty();
            }

            if (state.speedParameter != paramName)
            {
                state.speedParameter = paramName;
                ac.Dirty();
            }

            return paramName;
        }
        
        private static string CreateRunParameterIfNeed(AnimatorController ac, AnimatorState state, int layer, string oldParamName)
        {
            var paramName = $"@{ac.layers[layer].name}.{state.name} #run";

            var oldParam = ac.parameters.FirstOrDefault(x => x.name == oldParamName);
            if (oldParam != null && oldParamName != paramName)
            {
                ac.RemoveParameter(oldParam);
                ac.Dirty();
            }
            
            var paramIdx = Array.FindIndex(ac.parameters, x => x.name == paramName);
            if (paramIdx == -1 || ac.parameters[paramIdx].type != AnimatorControllerParameterType.Trigger)
            {
                if (paramIdx != -1)
                    paramName = ac.MakeUniqueParameterName(paramName);
                ac.AddParameter(new AnimatorControllerParameter{ name = paramName, type = AnimatorControllerParameterType.Trigger });
                ac.Dirty();
            }

            return paramName;
        }
        
        private static string CreateStayParameterIfNeed(AnimatorController ac, AnimatorState state, int layer, string oldParamName)
        {
            var paramName = $"@{ac.layers[layer].name}.{state.name} #stay";

            var oldParam = ac.parameters.FirstOrDefault(x => x.name == oldParamName);
            if (oldParam != null && oldParamName != paramName)
            {
                ac.RemoveParameter(oldParam);
                ac.Dirty();
            }
            
            var paramIdx = Array.FindIndex(ac.parameters, x => x.name == paramName);
            if (paramIdx == -1 || ac.parameters[paramIdx].type != AnimatorControllerParameterType.Bool)
            {
                if (paramIdx != -1)
                    paramName = ac.MakeUniqueParameterName(paramName);
                ac.AddParameter(new AnimatorControllerParameter{ name = paramName, type = AnimatorControllerParameterType.Bool, defaultBool = false});
                ac.Dirty();
            }

            paramIdx = Array.FindIndex(ac.parameters, x => x.name == paramName);
            if (ac.parameters[paramIdx].defaultBool != false)
            {
                var newParams = ac.parameters.ToArray(); 
                newParams[paramIdx].defaultBool = false;
                ac.parameters = newParams;
                ac.Dirty();
            }

            return paramName;
        }

        private static void CreateTransitions(AnimatorController ac, int layerIdx, AnimatorState state, string runParam, string stayParam)
        {
            if (ac == null) throw new ArgumentNullException(nameof(ac));
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (string.IsNullOrEmpty(runParam)) throw new ArgumentNullException(nameof(runParam));
            if (string.IsNullOrEmpty(stayParam)) throw new ArgumentNullException(nameof(stayParam));

            const float duration = 0.1f;
            
            // enter from anystate
            if (!Array.Exists(ac.layers[layerIdx].stateMachine.anyStateTransitions, x => x.destinationState == state))
                CreateTransition(ac, layerIdx, null, state, duration, null, new []{ new AnimatorCondition{ parameter = runParam, mode = AnimatorConditionMode.If } });

            // exit from state
            if (!Array.Exists(state.transitions, x => x.destinationState == null))
                CreateTransition(ac, layerIdx, state, null, duration, null, new []{ new AnimatorCondition{ parameter = stayParam, mode = AnimatorConditionMode.IfNot } });

            // enter from prev state
            var (baseName, num) = ParseStateName(state.name);
            var prevState = ac.layers[layerIdx].stateMachine.states
                .Select(x => (x.state, ParseStateName(x.state.name)))
                .Where(x => x.Item2.Item1 == baseName && x.Item2.Item2 < num)
                .OrderBy(x => x.Item2.Item2)
                .LastOrDefault();
            if (prevState.state != null)
                if (!Array.Exists(prevState.state.transitions, x => x.destinationState == state))
                    CreateTransition(ac, layerIdx, prevState.state, state, duration, null, new []{ new AnimatorCondition{ parameter = runParam, mode = AnimatorConditionMode.If } });
        }

        private static void CreateTransition(AnimatorController ac, int layerIdx, AnimatorState stateFrom, AnimatorState stateTo, float duration, float? exitTime, AnimatorCondition[] conditions)
        {
            Debug.Log($"Transition '{stateFrom?.name ?? "AnyState"}' -> '{stateTo?.name ?? "Exit"}' Duration:{duration} ExitTime:{exitTime} Conditions:[{string.Join(", ",conditions.Select(x => $"{x.mode} {x.parameter}"))}]");

            AnimatorStateTransition tr;
            if (stateFrom != null && stateTo != null)
                tr = stateFrom.AddTransition(stateTo);
            else if (stateFrom != null && stateTo == null)
                tr = stateFrom.AddExitTransition();
            else if (stateFrom == null && stateTo != null)
                tr = ac.layers[layerIdx].stateMachine.AddAnyStateTransition(stateTo);
            else
                throw new ArgumentNullException(nameof(stateFrom));
            tr.duration = duration;
            tr.hasExitTime = exitTime.HasValue;
            if (exitTime.HasValue)
                tr.exitTime = exitTime.Value;
            tr.canTransitionToSelf = true;
            if (conditions != null)
                tr.conditions = conditions;
            stateFrom?.Dirty();
            stateTo?.Dirty();
            ac.Dirty();
        }

        private static (string, int) ParseStateName(string name)
        {
            var match = Regex.Match(name, "^(.*[^0-9])([0-9]+?)$");
            string baseName = name;
            int number = 0;
            if (match.Success)
            {
                baseName = match.Groups[1].Value.TrimEnd(' ');
                number = int.Parse(match.Groups[2].Value);
            }
            return (baseName, number);
        }
    }
}