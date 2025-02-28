using Assets.Scripts;
using Assets.Scripts.Level.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFaceDisplay : MonoBehaviour
{
    public List<UIntWithSprite> FaceSprites;
    private Image imageComponent;


    public void SetDisplay(bool enabled)
    {
        if (imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
        }

        imageComponent.enabled = enabled;
    }

    public void SetDisplayLevel(uint oxygenLevel)
    {
        if (imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
        }

        foreach (UIntWithSprite sprite in FaceSprites)
        {
            if (oxygenLevel > sprite.Value)
            {
                continue;
            }

            imageComponent.sprite = sprite.Sprite;
            return;
        }
    }
}
