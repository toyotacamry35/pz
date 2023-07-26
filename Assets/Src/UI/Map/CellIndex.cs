using TMPro;
using UnityEngine;

namespace Uins
{
    public class CellIndex : MonoBehaviour
    {
        public TextMeshProUGUI Title;

        private void Awake()
        {
            Title.AssertIfNull(nameof(Title));
        }
    }
}