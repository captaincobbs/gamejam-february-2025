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
    void Start()
    {
        imageComponent = GetComponent<Image>();
    }

    public void SetOxygenDisplayLevel(uint level)
    {
        if (OxygenLevels != null && OxygenLevels.Length > 0)
        {
            if (level < OxygenLevels.Length - 1)
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
