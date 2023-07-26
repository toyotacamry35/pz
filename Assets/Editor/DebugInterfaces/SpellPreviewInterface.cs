using Assets.Src.ResourceSystem;
using Assets.Src.Wizardry;
using SharedCode.Wizardry;
using System;
using ColonyShared.SharedCode.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.DebugInterfaces.Editor
{
    class SpellPreviewInterface : EditorWindow
    {
        [MenuItem("Debug/SpellPreviewInterface")]
        static void GetSpellPreviewInterface()
        {
            GetWindow<SpellPreviewInterface>();
        }

        private void OnEnable()
        {
            _labelStyle = new GUIStyle() { normal = new GUIStyleState() { textColor = Color.white } };
        }

        SpellDef spellDef;
        private void InternalUpdate()
        {
            spellDef = null;
            var jdb = Selection.activeObject as JdbMetadata;
            if (jdb != null)
            {
                var res = jdb.Get();
                if (res is SpellDef)
                {
                    spellDef = (SpellDef)res;
                }
            }
        }
        private static int _drawDepth;
        private static readonly int _drawShift = 70;
        private static readonly Color[] _drawColor = new Color[] { Color.green, Color.yellow, Color.blue, Color.red, Color.magenta };
        private static GUIStyle _labelStyle;
        private static void DrawSubSpellsGUI(SpellDef spell)
        {
            _drawDepth = 0;
            GUILayout.Label(spell.Name, _labelStyle);
            Rect mainScale = GUILayoutUtility.GetLastRect();
            mainScale.width -= _drawShift;
            mainScale.x += _drawShift;
            GUIExtensions.DrawRectangle(mainScale, _drawColor[_drawDepth % _drawColor.Length]);

            DrawSubspells(spell, spell.SubSpells, 0, SyncTime.FromSeconds(spell.Duration), SyncTime.FromSeconds(spell.Duration));
        }

        private const float MinWidth = 1.0f;
        private static void DrawSubspells(SpellDef spellDef, SubSpell[] subSpells, long startOffest, long calculatedDuration, long overallDuration)
        {
            _drawDepth++;
            if (_drawDepth > 10)
                return;
            if (spellDef.IsInfinite || spellDef.Duration == 0)
                return;

            foreach (var subSpell in subSpells)
            {
                if (subSpell.Spell == null)
                    continue;
                var start = GetStartTimeOffset(subSpell, calculatedDuration) + startOffest;
                var duration = GetDuration(subSpell, calculatedDuration);
                var floatStart = (float)start / (float)overallDuration;
                var floatDuration = (float)duration / (float)overallDuration;
                GUILayout.Label(subSpell.Spell.Target.Name, _labelStyle);
                Rect scale = GUILayoutUtility.GetLastRect();
                scale.width -= _drawShift;
                scale.x += _drawShift;
                var w = Mathf.Max(floatDuration * scale.width, MinWidth);
                GUIExtensions.DrawRectangle(new Rect(scale.x + floatStart * scale.width, scale.y, w, scale.height), _drawColor[_drawDepth % _drawColor.Length]);
                DrawSubspells(subSpell.Spell, subSpell.Spell.Target.SubSpells, start, duration, overallDuration);
            }
        }
        private static long GetDuration(SubSpell spell, long baseSpellDuration)
        {
            return spell.HasOverridenDuration
                ? (spell.OverrideDurationPercent
                    ? (long)Math.Round(spell.OverridenDurationPercent * baseSpellDuration)
                    : SyncTime.FromSeconds(spell.OverridenDuration))
                : SyncTime.FromSeconds(spell.Spell.Target.Duration);
        }

        private static long GetStartTimeOffset(SubSpell spell, long baseSpellDuration)
        {
            return TimelineHelpers.CalcStartOffsetFromSubSpellAndParent(baseSpellDuration, false, spell);
        }
        private void OnGUI()
        {
            GUIExtensions.DrawRectangle(new Rect(0, 0, position.width, position.height), new Color(0.5f, 0.5f, 0.5f));
            if (spellDef != null)
            {
                DrawSubSpellsGUI(spellDef);
            }
            if (UnityEngine.Event.current.type == EventType.Repaint)
                InternalUpdate();
            Repaint();
        }
    }
}
