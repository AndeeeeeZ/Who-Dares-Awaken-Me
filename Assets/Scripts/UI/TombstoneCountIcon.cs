using UnityEngine;

public class TombstoneCountIcon : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image outlineImage;
    [SerializeField] private UnityEngine.UI.Image spriteImage;

    public bool IsAssigned => outlineImage != null && spriteImage != null;

    public void SetAppearance(Sprite outlineSprite, Sprite iconSprite, Color iconColor)
    {
        outlineImage.sprite = outlineSprite;
        spriteImage.sprite = iconSprite;
        spriteImage.color = iconColor;
    }
}
