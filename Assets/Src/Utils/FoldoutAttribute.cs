using UnityEngine;

namespace Assets.Src.Utils
{
    public class FoldoutAttribute : PropertyAttribute
    {
        public string Text { get; }
        public bool Default { get; }

        public FoldoutAttribute(string text, bool @default = true)
        {
            Text = text;
            Default = @default;
        }
    }
}