using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    public class ChanceImagesSet : MonoBehaviour
    {
        private const float MinChanceBorder = .25f;
        private const float MedChanceBorder = .6f;
        private const int ImagesCount = 3;

        /// <summary>
        /// Min, Medium, Max chances image
        /// </summary>
        public Image[] Images;


        //=== Unity ===============================================================

        private void Awake()
        {
            if (!Images.IsNullOrEmptyOrHasNullElements(nameof(Images)) && Images.Length != ImagesCount)
                UI.Logger.Debug($"<{nameof(ChanceImagesSet)}> {nameof(Images)} length isn't equal {nameof(ImagesCount)}={ImagesCount}",
                    gameObject);
        }


        //=== Public ==============================================================

        /// <summary>
        /// Включает
        /// </summary>
        /// <param name="chance">0...1</param>
        public void SwitchImagesByChance(float chance)
        {
            int shownImageIndex = 2;
            if (chance < MedChanceBorder)
                shownImageIndex = chance < MinChanceBorder ? 0 : 1;

            for (int i = 0; i < ImagesCount; i++)
                Images[i].enabled = i == shownImageIndex;
        }

        public void HideAll()
        {
            for (int i = 0; i < ImagesCount; i++)
                Images[i].enabled = false;
        }
    }
}