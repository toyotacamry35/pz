using UnityEditor;

namespace Assets.Src.Cartographer.Editor
{
    public interface ICartographerProgressCallback
    {
        string Title { get; }
        string Message { get; }
        float Progress { get; }

        bool OnBegin(string title, string message);
        void OnEnd();
        bool OnProgress(string title, string message, float progress);
    }

    public class CartographerProgressCallback
    {
        public static DefaultCartographerProgressCallback Default { get; private set; } = new DefaultCartographerProgressCallback();

        public class DefaultCartographerProgressCallback : ICartographerProgressCallback
        {
            public string Title { get; private set; } = string.Empty;
            public string Message { get; private set; } = string.Empty;
            public float Progress { get; private set; } = 0.0f;

            public bool OnBegin(string title, string message)
            {
                EditorUtility.DisplayProgressBar(title, message, 0.0f);
                Title = title;
                Message = message;
                Progress = 0.0f;
                return true;
            }

            public void OnEnd()
            {
                EditorUtility.ClearProgressBar();
                Title = string.Empty;
                Message = string.Empty;
                Progress = 0.0f;
            }

            public bool OnProgress(string title, string message, float progress)
            {
                EditorUtility.DisplayProgressBar(title, message, progress);
                Title = title;
                Message = message;
                Progress = progress;
                return true;
            }
        }
    }
};