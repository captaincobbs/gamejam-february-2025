using FMODUnity;
using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    [Header("Properties")]
    private bool isConsumed = false;
    [HideInInspector]
    public bool IsConsumed
    {
        get => isConsumed;
        set
        {
            isConsumed = value;
            if (isConsumed)
            {
                gameObject.SetActive(false);
            }
        }
    }

    [Tooltip("If true, the oxygen tank will be consumed on use.")]
    public bool ConsumedOnUse = false;
    [Tooltip("The amount of oxygen to refill.")]
    [SerializeField] uint oxygenRefilled = 5;
    [Tooltip("The maximum value that oxygen can be refilled up to.")]
    [SerializeField] uint refillUpTo = 5;

    [Header("Sound Events")]
    [SerializeField] private EventReference onUse;

    LevelManager LevelManager
    {
        get => LevelManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsConsumed && collider.TryGetComponent(out Player _))
        {
            if (onUse.IsNull)
            {
                Debug.LogWarning("OnUse is null");
            }

            AudioManager.Instance.PlayOneShot(onUse);
            LevelManager.RefillOxygen(oxygenRefilled, refillUpTo);

            if (ConsumedOnUse)
            {
                IsConsumed = true;
            }
        }
    }
}
