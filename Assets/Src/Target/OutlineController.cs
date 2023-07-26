using System;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.Destructions;
using Assets.Src.InteractionSystem;
using Assets.Tools;
using OutlineEffect;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;
using UnityEngine;
using SharedCode.Serializers;

namespace Assets.Src.Target
{
    public class OutlineController : MonoBehaviour
    {
        TargetHolder _targetHolder;
        ISpellDoer _spellDoer;

        private readonly List<Outline> _outlines = new List<Outline>();
        DestructController _destructController;
        Interactive _interactive;

        public void Init([NotNull] TargetHolder holder, [NotNull] ISpellDoer spellDoer)
        {
            if (holder == null) throw new ArgumentNullException(nameof(holder));
            if (spellDoer == null) throw new ArgumentNullException(nameof(spellDoer));
            _targetHolder = holder;
            _spellDoer = spellDoer;

            _targetHolder.TargetChanged += OnTargetChanged;
            OnTargetChanged(null, _targetHolder.CurrentTarget.Value, _targetHolder.HasAutority);
        }

        private void OnDestroy()
        {
            if (_targetHolder)
                _targetHolder.TargetChanged -= OnTargetChanged;

            OnTargetChanged(null, null, false);
        }

        private void OnTargetChanged(GameObject oldObject, GameObject newObject, bool hasAutority)
        {
            for (int i = 0; i < _outlines.Count; i++)
            {
                if (_outlines[i] != null)
                {
                    Destroy(_outlines[i]);
                }
            }

            _outlines.Clear();
            if (_destructController != null)
            {
                _destructController.InstantiateFracturedEvent -= ChangeOutlinedObject;
                _destructController.DetachFracturedChunkEvent -= OnDetachFracturedChunk;
            }

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            if (newObject != null)
            {
                _coroutine = this.StartInstrumentedCoroutine(UpdateColorCoroutine());

                _interactive = newObject.GetComponent<Interactive>();
                if (_interactive != null)
                {
                    if (hasAutority)
                        ChangeOutlinedObject(newObject);
                    _destructController = newObject.GetComponent<DestructController>();
                    if (_destructController != null)
                    {
                        _destructController.InstantiateFracturedEvent += ChangeOutlinedObject;
                        _destructController.DetachFracturedChunkEvent += OnDetachFracturedChunk;
                    }
                }
            }
        }

        private void OnDetachFracturedChunk(FracturedChunk fracturedChunk)
        {
            if (_outlines.AssertIfNull(nameof(_outlines)) ||
                fracturedChunk.AssertIfNull(nameof(fracturedChunk)))
                return;

            var outline = _outlines.FirstOrDefault(outl => outl.transform == fracturedChunk.transform);
            _outlines.Remove(outline);
            Destroy(outline);
        }

        private void ChangeOutlinedObject(GameObject newObject)
        {
            Renderer[] renders = newObject.GetComponentsInChildren<Renderer>();
            if (renders != null)
            {
                for (int i=0; i<renders.Length; i++)
                {
                    if (i == 0)
                    {
                        if (renders[i].CompareTag("noOutline"))
                            continue;

                        var fracturedChunk = renders[i].transform.GetComponent<FracturedChunk>();
                        if (fracturedChunk != null && fracturedChunk.IsDetachedChunk)
                            continue;

                        AddOutlineToRenderer(renders[i]);
                    }
                }
            }
        }

        void AddOutlineToRenderer(Renderer renderer)
        {
            if (_interactive == null || _spellDoer == null || renderer == null || !Constants.WorldConstants.EnableOutline)
            {
                return;
            }

            Outline outline = renderer.gameObject.GetOrAddComponent<Outline>();
            outline.ColorDelegate = () => { return lastColor; };

            _outlines.Add(outline);
        }

        private OutlineColor lastColor = OutlineColor.Default;
        private Coroutine _coroutine;
        private static readonly YieldInstruction Point7Second = new WaitForSeconds(0.7f);

        private IEnumerator UpdateColorCoroutine()
        {
            while (true)
            {
                var task = AsyncUtils.RunAsyncTask(UpdateColorAsync, ClusterCommands.Repository);

                while (!task.IsCompleted)
                    yield return null;

                lastColor = task.Result;

                yield return Point7Second;
            }
        }

        private async Task<OutlineColor> UpdateColorAsync()
        {
            //if (_doingChooseSpell != null && _doingChooseSpell.IsDoingSpell())

            if (_interactive != null)
            {
                var spell = await _interactive.ChooseSpell(_spellDoer, SpellDescription.InteractionAction);
                if (spell != null)
                {
                    return OutlineHelper.OutlineColorFromIndex(spell.OutlineColorIndex);
                }

                spell = await _interactive.ChooseSpell(_spellDoer, SpellDescription.AttackAction);
                if (spell != null)
                {
                    return OutlineHelper.OutlineColorFromIndex(spell.OutlineColorIndex);
                }
            }

            return OutlineColor.Blue;
        }
    }
}