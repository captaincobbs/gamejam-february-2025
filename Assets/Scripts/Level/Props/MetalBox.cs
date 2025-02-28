using Assets.Scripts;
using FMODUnity;
using UnityEngine;

public class MetalBox : Killable
{
    [SerializeField] private EventReference onDeath;

    public override void Death()
    {
        AudioManager.Instance.PlayOneShot(onDeath);
    }
}
