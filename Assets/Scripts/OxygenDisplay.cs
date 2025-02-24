using UnityEngine;
using UnityEngine.UI;

public class OxygenDisplay : MonoBehaviour
{
    [Header("Oxygen Levels")]
    public Sprite[] OxygenLevels;

    [Header("Graphics")]
    [Tooltip("This is the default Oxygen sprite used")]
    public Sprite OxygenLevelDefault;

    private Image imageComponent;

    public void SetOxygenDisplayLevel(uint level)
    {
        if (imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
        }

        if (OxygenLevels != null && OxygenLevels.Length > 0)
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
