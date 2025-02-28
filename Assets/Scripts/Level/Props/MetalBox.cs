using Assets.Scripts;
using FMODUnity;
using UnityEngine;

public class MetalBox : Killable
{
    [SerializeField] private EventReference onDeath;

    public override void Death()
    {
        if (onDeath.IsNull)
        {
            Debug.LogWarning("OnDeath is null");
        }

        AudioManager.Instance.PlayOneShot(onDeath);
    }
}
