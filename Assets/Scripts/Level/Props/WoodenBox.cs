using FMODUnity;
using UnityEngine;

public class WoodenBox : Entity
{
    [Header("Sound Events")]
    [SerializeField] private EventReference onMove;
    [SerializeField] private EventReference onDeath;
    public override void Move()
    {
        if (onMove.IsNull)
        {
            Debug.LogWarning("OnMove is null");
        }

        AudioManager.Instance.PlayOneShot(onMove);
    }

    public override void Death()
    {
        if (onDeath.IsNull)
        {
            Debug.LogWarning("OnDeath is null");
        }

        AudioManager.Instance.PlayOneShot(onDeath);
    }
}
