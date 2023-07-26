using Assets.Src.Lib.DOTweenAdds;
using UnityEngine.UI.ProceduralImage;

public class ProceduralImageBlurTweenComponent : TweenComponentBase
{
    public ProceduralImage ProceduralImage;

    protected override float Parameter
    {
        get { return ProceduralImage.FalloffDistance; }
        set { ProceduralImage.FalloffDistance = value; }
    }
}