using UnityEngine;
using UnityEngine.UI;

public class CrossButton : MonoBehaviour
{
    public Sprite SafeSprite;
    public Sprite DangerSprite;
    public Sprite NoPilotSprite;

    public Image ButtonImage;

    public void SetToClickable()
    {
        SetToSafe();
    }

    public void SetToUnClickable()
    {
        ButtonImage.sprite = NoPilotSprite;
    }

    public void SetToSafe()
    {
        ButtonImage.sprite = SafeSprite;
    }

    public void SetToDanger()
    {
        ButtonImage.sprite = DangerSprite;
    }
}
