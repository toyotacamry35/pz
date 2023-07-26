using UnityEngine;
using UnityEngine.UI;

public class RandomImageSetter : MonoBehaviour
{
    public Sprite[] Sprites;
    public Image TargetImage;


    void Awake()
    {
        if (TargetImage.AssertIfNull(nameof(TargetImage)) ||
            Sprites.IsNullOrEmptyOrHasNullElements(nameof(Sprites)))
            return;

        SetRandomImage();
    }

    public void SetRandomImage()
    {
        if (!TargetImage.IsDestroyed())
            TargetImage.sprite = Sprites[Random.Range(0, Sprites.Length)];
    }
}