using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameObjectBrush
{
    public interface IGOBrushPreviewImageGiver
    {
        Texture2D GetPreview { get; }
    }


    public interface IBrushExtender
    {
        void AfterBrushPlacement();
    }


}

