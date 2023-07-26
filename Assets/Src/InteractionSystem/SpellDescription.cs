using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using EnumerableExtensions;
using JetBrains.Annotations;
using L10n;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.Src.InteractionSystem
{
    public class SpellDescription
    {
        public static readonly ResourceRef<InputActionDef> InteractionAction = new ResourceRef<InputActionDef>(@"/UtilPrefabs/Input/Actions/Interaction");
        public static readonly ResourceRef<InputActionDef> InfoAction = new ResourceRef<InputActionDef>(@"/UtilPrefabs/Input/Actions/Info");
        public static readonly ResourceRef<InputActionDef> AttackAction = new ResourceRef<InputActionDef>(@"/UtilPrefabs/Input/Actions/Attack");

        /// <summary>
        /// Наименование действия в данном контексте
        /// </summary>
        public string ActionDescription;

        public Sprite InteractionSprite;

        public bool DontShowProgress;

        public bool DontShowDetails;

        /// <summary>
        /// Список вариантов взаимодействия с объектом (например, атака режущим, атака колющим, при этом действие всегда атака)
        /// </summary>
        [NotNull]
        public List<ConditionVariant> ConditionVariants = new List<ConditionVariant>();

        public bool HasOverridenInteractionSprite => InteractionSprite != null;

        public bool HasActionDescription => !String.IsNullOrEmpty(ActionDescription);

        public bool IsEmpty => InteractionSprite == null && ConditionVariants.Count == 0;

        public bool HasNoEmptyVariants => ConditionVariants.Any(variant => variant.Items.Any());

        public override string ToString()
        {
            return $"{nameof(SpellDescription)}: {nameof(ActionDescription)}='{ActionDescription}', " +
                   $"{nameof(HasOverridenInteractionSprite)}{HasOverridenInteractionSprite.AsSign()} " +
                   $"{nameof(HasActionDescription)}{HasActionDescription.AsSign()}" +
                   $"{nameof(IsEmpty)}{IsEmpty.AsSign()}" +
                   $"{nameof(HasNoEmptyVariants)}{HasNoEmptyVariants.AsSign()}" +
                   $"{nameof(ConditionVariants)} {ConditionVariants.ItemsToStringByLines()}";
        }

        /// <summary>
        /// Объединение other с текущим. Если в other есть незаполненные у нас поля - заполняем их
        /// </summary>
        public void Merge(SpellDescription other)
        {
            if (ActionDescription == null && other.ActionDescription != null)
                ActionDescription = other.ActionDescription;

            if (InteractionSprite == null && other.InteractionSprite != null)
                InteractionSprite = other.InteractionSprite;

            if (!DontShowProgress && other.DontShowProgress)
                DontShowProgress = other.DontShowProgress;

            if (!DontShowDetails && other.DontShowDetails)
                DontShowDetails = other.DontShowDetails;

            ConditionVariants = ConditionVariants.Concat(other.ConditionVariants).ToList(); // .Where(variant => variant.Items.Count > 0)
        }

        public static SpellDescription GetSpellDescription(SpellDef spell, GameObject target, bool lookForLocalDescription = false)
        {
            var spellDef = spell;
            var spellDescription = new SpellDescription
            {
                InteractionSprite = spellDef?.SpellIcon?.Target,
                ActionDescription = spellDef?.InteractionDescriptionLs.GetText(),
                DontShowProgress = spellDef?.DontShowProgress ?? false
            };
            if (target == null)
                return spellDescription;


            spellDescription.ConditionVariants = new List<ConditionVariant>();
            if (lookForLocalDescription)
            {
                var localCollectingDescription = target ? target.GetComponent<LocalCollectingDescription>() : null;
                if (localCollectingDescription != null && localCollectingDescription.ItemResource.Target != null)
                {
                    var conditionVariant = new ConditionVariant()
                    {
                        Items =
                        {
                            new SharedCode.Entities.Mineable.ProbabilisticItemPack(
                                new SharedCode.Entities.ItemResourcePack(localCollectingDescription.ItemResource.Target, 1),
                                1)
                        }
                    };
                    spellDescription.ConditionVariants.Add(conditionVariant);
                }
            }

            return spellDescription;
        }

        public static SpellDescription GetSpellDescription(SpellVariant spellVariant, GameObject target)
        {
            var spellDescription = GetSpellDescription(spellVariant.Spell, target);
            if (spellDescription.ConditionVariants.Count == 0)
                return spellDescription;

            var firstConditionVariant = spellDescription.ConditionVariants.FirstOrDefault();
            foreach (var svCondition in spellVariant.Conditions)
            {
                if (svCondition is IConditionDescriptable)
                {
                    firstConditionVariant.ConditionMarkers.Add((svCondition as IConditionDescriptable).GetConditionMarker());
                }
            }

            return spellDescription;
        }

        public string GetActionTitle(InputActionDef action)
        {
            if (HasActionDescription)
                return ActionDescription;

            return action.Description;
        }
    }
}