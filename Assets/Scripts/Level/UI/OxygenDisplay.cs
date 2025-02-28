using UnityEngine;
using UnityEngine.UI;

public class OxygenDisplay : MonoBehaviour
{
    [Header("Oxygen Levels")]
    [Tooltip("The sprites displayed for corresponding oxygen values, the index of the array is the associated level of oxygen that the sprite will display for.")]
    public Sprite[] OxygenLevels;

    [Header("Graphics")]
    [Tooltip("The default oxygen sprite used if the oxygen value is invalid")]
    public Sprite OxygenLevelDefault;
    private Image imageComponent;

    public void SetDisplay(bool enabled)
    {
        if (imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
        }

        imageComponent.enabled = enabled;
    }

    public void SetDisplayLevel(uint level)
    {
        if (imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
        }

        if (OxygenLevels?.Length > 0)
        {
            if (level < OxygenLevels.Length)
            {
                imageComponent.sprite = OxygenLevels[level];
            }
            else
            {
                imageComponent.sprite = OxygenLevelDefault;
                Debug.Log($"No Sprite for Oxygen level of {level}");
            }
        }
        else
        {
            Debug.Log("No Oxygen Level Sprites provided");
        }
    }
}
