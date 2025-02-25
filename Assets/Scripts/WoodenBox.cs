using FMODUnity;
using UnityEngine;

public class WoodenBox : Entity
{
    [Header("Sound Events")]
    [SerializeField] private EventReference onMove;
    [SerializeField] private EventReference onDeath;
    public override void OnMove()
    {
        AudioManager.PlayOneShot(onMove);
    }

    public override void OnDeath()
    {
        AudioManager.PlayOneShot(onDeath);
    }
}
