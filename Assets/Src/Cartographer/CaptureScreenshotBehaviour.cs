using UnityEngine;

namespace Assets.Src.Cartographer
{
    public class CaptureScreenshotBehaviour : MonoBehaviour
    {
        public static bool CreategameScreenshot = false;

        void Update()
        {
            if (CreategameScreenshot || Input.GetKeyDown(KeyCode.F12))
            {
                CreategameScreenshot = false;
                StartCoroutine(CaptureScreenshotUtils.MakeGameScreenshotCoroutine());
            }
        }
    }
}