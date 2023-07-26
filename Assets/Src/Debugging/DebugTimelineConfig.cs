using UnityEngine;

namespace Src.Debugging
{
    [CreateAssetMenu(menuName = "Debug/Timeline Config")]
    public class DebugTimelineConfig : ScriptableObject
    {
        public ulong TimeRange = 5 * 1000;
        public ulong TimeMarkPeriod = 1 * 1000;
        public uint TimeSubMarks = 10;
        public float TimeRewindSpeed = 0.01f;
        public Rect Area = new Rect(1f / 20, 1f / 20, 18f / 20, 2f / 10);
        public Color BackColor = new Color(0, 0, 0, 0.1f);
        public Color MarkColor = new Color(1, 1, 1, 0.1f);
        public Color SubMarkColor = new Color(1, 1, 1, 0.05f);
        public Color IntervalBgnColor = new Color(0, 0, 0, 0.1f);
        public Color TextColor = new Color(0, 0, 0, 1);
        public float TextRatio = 1f;  
        public float TextSpacing = 2f;  
        public float TextPadding = 1f;  
        public float BarsAlpha = 1f;  
        public DebugTimeline.Filter GlobalFilter;
        public DebugTimeline.Rule[] Rules;
    }
}